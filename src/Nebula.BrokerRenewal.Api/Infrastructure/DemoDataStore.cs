using Nebula.BrokerRenewal.Api.Domain;

namespace Nebula.BrokerRenewal.Api.Infrastructure;

public sealed class DemoDataStore
{
    public List<Renewal> Renewals { get; } =
    [
        new("R-1001", "tenant-contoso", "broker-100", "Contoso Manufacturing",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)), 125000m, "USD", "Pending", "High"),
        new("R-1002", "tenant-contoso", "broker-100", "Northwind Logistics",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(51)), 87500m, "USD", "Pending", "Medium"),
        new("R-1003", "tenant-contoso", "broker-200", "Fabrikam Retail",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(35)), 64000m, "USD", "Pending", "Low"),
        new("R-2001", "tenant-adventure", "broker-300", "Adventure Works",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(18)), 99000m, "USD", "Pending", "High")
    ];

    public List<OutreachDraft> Drafts { get; } = [];
}
