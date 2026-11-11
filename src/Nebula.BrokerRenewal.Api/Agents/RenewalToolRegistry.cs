using System.Text.Json;
using Nebula.BrokerRenewal.Api.Infrastructure;

namespace Nebula.BrokerRenewal.Api.Agents;

public sealed class RenewalToolRegistry : IToolRegistry
{
    private readonly IReadOnlyDictionary<string, IAgentTool> _tools;

    public RenewalToolRegistry(IRenewalService renewals, IOutreachService outreach)
    {
        _tools = new Dictionary<string, IAgentTool>(StringComparer.OrdinalIgnoreCase)
        {
            ["renewals.list-needing-attention"] = new ListRenewalsTool(renewals),
            ["outreach.create-draft"] = new CreateOutreachDraftTool(outreach)
        };
    }

    public IAgentTool GetRequired(string name) => _tools.TryGetValue(name, out var tool)
        ? tool : throw new InvalidOperationException($"Tool '{name}' is not registered.");

    public IReadOnlyCollection<string> GetToolNames() => _tools.Keys.ToArray();
}

internal sealed class ListRenewalsTool(IRenewalService service) : IAgentTool
{
    public string Name => "renewals.list-needing-attention";
    public string Description => "List pending renewals within a number of days.";
    public async Task<string> ExecuteAsync(JsonElement args, CancellationToken ct)
    {
        var days = args.TryGetProperty("withinDays", out var value) && value.TryGetInt32(out var n) ? n : 90;
        return JsonSerializer.Serialize(await service.ListNeedingAttentionAsync(days, ct));
    }
}

internal sealed class CreateOutreachDraftTool(IOutreachService service) : IAgentTool
{
    public string Name => "outreach.create-draft";
    public string Description => "Create a draft email for an in-scope renewal. Never sends it.";
    public async Task<string> ExecuteAsync(JsonElement args, CancellationToken ct)
    {
        if (!args.TryGetProperty("renewalId", out var value) || string.IsNullOrWhiteSpace(value.GetString()))
            throw new ArgumentException("renewalId is required.");
        return JsonSerializer.Serialize(await service.CreateDraftAsync(value.GetString()!, ct));
    }
}
