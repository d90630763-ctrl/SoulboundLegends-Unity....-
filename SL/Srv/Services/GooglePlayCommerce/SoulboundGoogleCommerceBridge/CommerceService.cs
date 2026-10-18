namespace SoulboundGoogleCommerceBridge;

public sealed record GoogleVerifyRequest(
    string RequestId,
    string ProductId,
    string PurchaseToken,
    string PackageName
);

public sealed record CommerceResult(
    bool Accepted,
    bool Pending,
    bool AlreadyProcessed,
    string? TransactionId,
    string Message
);

public sealed class CommerceService
{
    private readonly GooglePlayVerifier verifier;
    private readonly Dictionary<string, CommerceResult> processedTokens = new();
    private readonly object gate = new();

    public CommerceService(GooglePlayVerifier verifier) => this.verifier = verifier;

    public async Task<CommerceResult> VerifyAsync(GoogleVerifyRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ProductId) ||
            string.IsNullOrWhiteSpace(request.PurchaseToken) ||
            string.IsNullOrWhiteSpace(request.PackageName))
            return new(false, false, false, null, "Missing commerce fields.");

        lock (gate)
        {
            if (processedTokens.TryGetValue(request.PurchaseToken, out CommerceResult? previous))
                return previous with { AlreadyProcessed = true };
        }

        string expectedPackage = Environment.GetEnvironmentVariable("GOOGLE_PLAY_PACKAGE_NAME") ?? "";
        if (!string.Equals(expectedPackage, request.PackageName, StringComparison.Ordinal))
            return new(false, false, false, null, "Package name rejected.");

        GooglePurchaseStatus status = await verifier.VerifyAsync(request.ProductId, request.PurchaseToken, request.PackageName, ct);

        if (status.Pending)
            return new(false, true, false, null, "Purchase is pending. No entitlement granted.");

        if (!status.Purchased)
            return new(false, false, false, null, "Google Play did not confirm a completed purchase.");

        // IMPORTANT: This is the integration boundary.
        // In production, call Master Authority here and only persist success after its
        // idempotent entitlement/economy transaction commits successfully.
        string transactionId = Guid.NewGuid().ToString("N");
        CommerceResult result = new(true, false, false, transactionId, "Purchase verified. Master Authority entitlement commit required/connected here.");

        lock (gate)
            processedTokens[request.PurchaseToken] = result;

        return result;
    }

    public Task RecordNotificationAsync(string body, CancellationToken ct)
    {
        // Production: validate Google RTDN envelope, decode Pub/Sub payload,
        // then query Google Play for authoritative purchase state.
        return Task.CompletedTask;
    }
}
