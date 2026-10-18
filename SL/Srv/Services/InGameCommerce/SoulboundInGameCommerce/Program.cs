using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<InGameCommerceService>();
var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { ok = true, service = "SoulboundLegends.InGameCommerceAuction" }));

app.MapPost("/api/commerce/shop/buy", [Authorize] (HttpContext ctx, ShopBuyRequest req, InGameCommerceService svc) =>
    Results.Ok(svc.Buy(ctx.User, req)));

app.MapPost("/api/commerce/auction/list", [Authorize] (HttpContext ctx, AuctionListRequest req, InGameCommerceService svc) =>
    Results.Ok(svc.List(ctx.User, req)));

app.MapPost("/api/commerce/auction/bid", [Authorize] (HttpContext ctx, AuctionBidRequest req, InGameCommerceService svc) =>
    Results.Ok(svc.Bid(ctx.User, req)));

app.MapPost("/api/commerce/auction/finalize", [Authorize] (HttpContext ctx, AuctionFinalizeRequest req, InGameCommerceService svc) =>
    Results.Ok(svc.Finalize(ctx.User, req)));

app.MapGet("/api/commerce/auction/listings", [Authorize] (InGameCommerceService svc) => Results.Ok(svc.Listings()));
app.Run();

public record ShopBuyRequest(string MerchantId, string ProductId, int Quantity, string IdempotencyKey);
public record AuctionListRequest(string ItemInstanceId, long StartingBid, long BuyoutPrice, int DurationSeconds, string IdempotencyKey);
public record AuctionBidRequest(string AuctionId, long BidAmount, string IdempotencyKey);
public record AuctionFinalizeRequest(string AuctionId, string IdempotencyKey);

public sealed class InGameCommerceService
{
    private readonly ConcurrentDictionary<string, object> _idempotency = new();
    private readonly ConcurrentDictionary<string, Auction> _auctions = new();

    public object Buy(System.Security.Claims.ClaimsPrincipal user, ShopBuyRequest r)
    {
        RequireValid(r.IdempotencyKey);
        if (r.Quantity <= 0 || r.Quantity > 100) throw new ArgumentException("Invalid quantity.");
        if (string.IsNullOrWhiteSpace(r.MerchantId) || string.IsNullOrWhiteSpace(r.ProductId)) throw new ArgumentException("Merchant/product is required.");
        // Production implementation: Master Authority validates balance, catalog, inventory capacity and atomically commits the debit/grant.
        return new { success = true, type = "shop_purchase", authoritativeCommitRequired = true, r.MerchantId, r.ProductId, r.Quantity };
    }

    public object List(System.Security.Claims.ClaimsPrincipal user, AuctionListRequest r)
    {
        RequireValid(r.IdempotencyKey);
        if (r.StartingBid <= 0 || r.DurationSeconds < 60) throw new ArgumentException("Invalid auction terms.");
        var id = Guid.NewGuid().ToString("N");
        var a = new Auction(id, user.Identity?.Name ?? "authenticated-player", r.ItemInstanceId, r.StartingBid, r.BuyoutPrice, DateTimeOffset.UtcNow.AddSeconds(Math.Min(r.DurationSeconds, 604800)), null, 0);
        _auctions[id] = a;
        // Production implementation: Master Authority must escrow the item before this listing is committed.
        return new { success = true, auctionId = id, a.EndsAt, escrowRequired = true };
    }

    public object Bid(System.Security.Claims.ClaimsPrincipal user, AuctionBidRequest r)
    {
        RequireValid(r.IdempotencyKey);
        if (!_auctions.TryGetValue(r.AuctionId, out var a)) throw new KeyNotFoundException("Auction not found.");
        if (a.EndsAt <= DateTimeOffset.UtcNow) throw new InvalidOperationException("Auction ended.");
        if (r.BidAmount <= a.CurrentBid) throw new InvalidOperationException("Bid must exceed current bid.");
        var bidder = user.Identity?.Name ?? "authenticated-player";
        _auctions[r.AuctionId] = a with { CurrentBid = r.BidAmount, HighestBidder = bidder };
        // Production implementation: reserve funds atomically through Master Authority.
        return new { success = true, auctionId = r.AuctionId, currentBid = r.BidAmount, fundsReservationRequired = true };
    }

    public object Finalize(System.Security.Claims.ClaimsPrincipal user, AuctionFinalizeRequest r)
    {
        RequireValid(r.IdempotencyKey);
        if (!_auctions.TryGetValue(r.AuctionId, out var a)) throw new KeyNotFoundException("Auction not found.");
        if (a.EndsAt > DateTimeOffset.UtcNow) throw new InvalidOperationException("Auction has not ended.");
        if (!string.IsNullOrWhiteSpace(a.HighestBidder))
            return new { success = true, winner = a.HighestBidder, winningBid = a.CurrentBid, settlementRequired = true };
        return new { success = true, unsold = true, returnItemRequired = true };
    }

    public IEnumerable<Auction> Listings() => _auctions.Values.Where(x => x.EndsAt > DateTimeOffset.UtcNow);
    private void RequireValid(string key) { if (string.IsNullOrWhiteSpace(key) || key.Length < 16) throw new ArgumentException("A strong idempotency key is required."); if (!_idempotency.TryAdd(key, new object())) throw new InvalidOperationException("Duplicate request."); }
}

public record Auction(string Id, string SellerId, string ItemInstanceId, long StartingBid, long BuyoutPrice, DateTimeOffset EndsAt, string? HighestBidder, long CurrentBid);
