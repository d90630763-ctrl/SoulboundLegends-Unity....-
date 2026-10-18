using SoulboundGoogleCommerceBridge;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CommerceService>();
builder.Services.AddSingleton<GooglePlayVerifier>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { service = "Soulbound Legends Google Play Commerce Bridge", status = "ok" }));

app.MapPost("/api/commerce/googleplay/verify", async (GoogleVerifyRequest request, CommerceService commerce, CancellationToken ct) =>
{
    CommerceResult result = await commerce.VerifyAsync(request, ct);
    return result.Accepted || result.Pending || result.AlreadyProcessed
        ? Results.Ok(result)
        : Results.BadRequest(result);
});

app.MapPost("/api/commerce/googleplay/restore", () => Results.Ok(new
{
    accepted = true,
    message = "Restore reconciliation endpoint is ready; connect it to authenticated Google Play purchase reconciliation."
}));

app.MapPost("/api/commerce/googleplay/rtdn", async (HttpRequest request, CommerceService commerce, CancellationToken ct) =>
{
    using var reader = new StreamReader(request.Body);
    string body = await reader.ReadToEndAsync(ct);
    await commerce.RecordNotificationAsync(body, ct);
    return Results.Ok(new { accepted = true });
});

app.Run();
