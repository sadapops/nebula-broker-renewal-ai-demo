using System.Text.Json;

namespace Nebula.BrokerRenewal.Api.Agents;

public sealed record AgentPlan(string ToolName, JsonElement Arguments, string Intent);
public interface IAgentModel { Task<AgentPlan> PlanAsync(string message, CancellationToken ct); }
public interface IAgentTool
{
    string Name { get; }
    string Description { get; }
    Task<string> ExecuteAsync(JsonElement arguments, CancellationToken ct);
}
public interface IToolRegistry
{
    IAgentTool GetRequired(string name);
    IReadOnlyCollection<string> GetToolNames();
}
