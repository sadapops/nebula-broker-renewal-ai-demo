namespace Nebula.BrokerRenewal.Api.Contracts;

public sealed record ChatRequest(string Message);
public sealed record AgentResponse(string Message, IReadOnlyList<ToolExecution> ToolExecutions, string Status);
public sealed record ToolExecution(string ToolName, bool Succeeded, string Summary);
