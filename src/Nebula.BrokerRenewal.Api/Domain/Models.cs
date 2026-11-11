namespace Nebula.BrokerRenewal.Api.Domain;

public sealed record UserContext(string TenantId, string BrokerId, string UserId);
public sealed record Renewal(string Id, string TenantId, string BrokerId, string CustomerName,
    DateOnly RenewalDate, decimal AnnualPremium, string Currency, string Status, string RiskLevel);
public sealed record OutreachDraft(string Id, string TenantId, string BrokerId, string RenewalId,
    string Subject, string Body, DateTimeOffset CreatedAt, string Status);
