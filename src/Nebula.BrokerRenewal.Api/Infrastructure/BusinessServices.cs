using Nebula.BrokerRenewal.Api.Domain;

namespace Nebula.BrokerRenewal.Api.Infrastructure;

public interface IRenewalService
{
    Task<IReadOnlyList<Renewal>> ListNeedingAttentionAsync(int withinDays, CancellationToken ct);
    Task<Renewal?> GetByIdAsync(string renewalId, CancellationToken ct);
}

public interface IOutreachService
{
    Task<OutreachDraft> CreateDraftAsync(string renewalId, CancellationToken ct);
    Task<IReadOnlyList<OutreachDraft>> GetDraftsAsync(CancellationToken ct);
}

public sealed class RenewalService(DemoDataStore store, IUserContextAccessor userAccessor) : IRenewalService
{
    public Task<IReadOnlyList<Renewal>> ListNeedingAttentionAsync(int withinDays, CancellationToken ct)
    {
        if (withinDays is < 1 or > 365) throw new ArgumentOutOfRangeException(nameof(withinDays));
        var user = userAccessor.GetCurrent();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        IReadOnlyList<Renewal> rows = store.Renewals
            .Where(x => x.TenantId == user.TenantId && x.BrokerId == user.BrokerId)
            .Where(x => x.Status == "Pending" && x.RenewalDate >= today && x.RenewalDate <= today.AddDays(withinDays))
            .OrderBy(x => x.RenewalDate).ToArray();
        return Task.FromResult(rows);
    }

    public Task<Renewal?> GetByIdAsync(string renewalId, CancellationToken ct)
    {
        var user = userAccessor.GetCurrent();
        var row = store.Renewals.SingleOrDefault(x =>
            x.Id == renewalId && x.TenantId == user.TenantId && x.BrokerId == user.BrokerId);
        return Task.FromResult(row);
    }
}

public sealed class OutreachService(
    DemoDataStore store, IRenewalService renewalService, IUserContextAccessor userAccessor) : IOutreachService
{
    public async Task<OutreachDraft> CreateDraftAsync(string renewalId, CancellationToken ct)
    {
        var renewal = await renewalService.GetByIdAsync(renewalId, ct)
            ?? throw new KeyNotFoundException("Renewal was not found in the current broker scope.");
        var user = userAccessor.GetCurrent();
        var draft = new OutreachDraft(Guid.NewGuid().ToString("N"), user.TenantId, user.BrokerId,
            renewal.Id, $"Renewal discussion: {renewal.CustomerName}",
            $"Hello {renewal.CustomerName} team,\n\nYour policy is due for renewal on {renewal.RenewalDate:dd MMM yyyy}. " +
            "We would welcome the opportunity to review your coverage before the renewal date.\n\nRegards,\nYour broker",
            DateTimeOffset.UtcNow, "Draft");
        store.Drafts.Add(draft);
        return draft;
    }

    public Task<IReadOnlyList<OutreachDraft>> GetDraftsAsync(CancellationToken ct)
    {
        var user = userAccessor.GetCurrent();
        IReadOnlyList<OutreachDraft> rows = store.Drafts
            .Where(x => x.TenantId == user.TenantId && x.BrokerId == user.BrokerId)
            .OrderByDescending(x => x.CreatedAt).ToArray();
        return Task.FromResult(rows);
    }
}
