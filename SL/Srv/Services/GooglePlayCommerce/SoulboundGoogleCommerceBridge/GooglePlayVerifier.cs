namespace SoulboundGoogleCommerceBridge;

public sealed record GooglePurchaseStatus(bool Purchased, bool Pending, string? OrderId);

public sealed class GooglePlayVerifier
{
    public Task<GooglePurchaseStatus> VerifyAsync(string productId, string purchaseToken, string packageName, CancellationToken ct)
    {
        // SECURITY: never treat the client purchase token itself as proof.
        // Production implementation must call Google's authenticated Developer API
        // with a server-only service account and inspect the current purchase state.
        // This sample refuses to pretend verification happened.
        bool allowDevelopmentStub = string.Equals(
            Environment.GetEnvironmentVariable("SBL_ALLOW_COMMERCE_DEV_STUB"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (allowDevelopmentStub)
            return Task.FromResult(new GooglePurchaseStatus(true, false, "DEV-STUB"));

        return Task.FromResult(new GooglePurchaseStatus(false, false, null));
    }
}
