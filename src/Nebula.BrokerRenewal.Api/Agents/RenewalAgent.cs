using Nebula.BrokerRenewal.Api.Contracts;

namespace Nebula.BrokerRenewal.Api.Agents;

public sealed class RenewalAgent(IAgentModel model, IToolRegistry tools, IConfiguration configuration)
{
    public async Task<AgentResponse> HandleAsync(string message, CancellationToken ct)
    {
        var plan = await model.PlanAsync(message, ct);
        var maxToolCalls = Math.Clamp(configuration.GetValue("Agent:MaxToolCalls", 3), 1, 5);
        if (maxToolCalls < 1) throw new InvalidOperationException("Tool execution budget exhausted.");

        // Only tools in the application's allow-listed registry can execute.
        var tool = tools.GetRequired(plan.ToolName);
        var output = await tool.ExecuteAsync(plan.Arguments, ct);
        return new AgentResponse(
            $"Intent: {plan.Intent}\nTool output: {output}",
            [new ToolExecution(tool.Name, true, "Executed successfully.")],
            "Completed");
    }
}
