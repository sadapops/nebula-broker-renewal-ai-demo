using System.Text.Json;

namespace Nebula.BrokerRenewal.Api.Agents;

// Deterministic stand-in for an LLM: no credentials or network call are needed.
// Replace this implementation with Azure OpenAI structured tool/function calling.
public sealed class DeterministicDemoModel : IAgentModel
{
    public Task<AgentPlan> PlanAsync(string message, CancellationToken ct)
    {
        var text = message.Trim();
        AgentPlan plan;

        if (text.Contains("draft", StringComparison.OrdinalIgnoreCase))
        {
            var match = System.Text.RegularExpressions.Regex.Match(text, @"R-[A-Za-z0-9-]+",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!match.Success) throw new ArgumentException("Please include a renewal ID such as R-1001.");
            plan = new AgentPlan("outreach.create-draft",
                JsonSerializer.SerializeToElement(new { renewalId = match.Value }), "create an outreach email draft");
        }
        else
        {
            var days = text.Contains("30") ? 30 : text.Contains("60") ? 60 : 90;
            plan = new AgentPlan("renewals.list-needing-attention",
                JsonSerializer.SerializeToElement(new { withinDays = days }), "list renewals needing attention");
        }

        return Task.FromResult(plan);
    }
}
