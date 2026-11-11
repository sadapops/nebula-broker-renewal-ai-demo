# Nebula Broker Renewal AI Demo

Standalone .NET 8 sample demonstrating an agentic broker-renewal workflow.

## Concepts demonstrated
- Agent orchestration: interpret a request, select a registered tool, execute it, and return a grounded result.
- Tool registry: the model can request only application-registered tools.
- Scope enforcement: tenant and broker filters are applied in business services, never trusted to the prompt.
- Human-in-the-loop: outreach is saved as a draft and is never sent automatically.
- Provider abstraction: deterministic local model keeps the sample runnable without API keys.

## Architecture
Broker -> ASP.NET Core API -> RenewalAgent -> IAgentModel (intent/tool plan) -> ToolRegistry -> Renewal/Outreach business services -> scoped data store.

The model proposes a tool and arguments. The application validates the tool against an allow-list, and business services enforce the caller's data scope.

## Run
Requires the .NET 8 SDK.

    dotnet restore
    dotnet run --project src/Nebula.BrokerRenewal.Api

The API prints its listening URL. The demo identity adapter expects X-Tenant-Id and X-Broker-Id headers. Example:

    curl -X POST http://localhost:5000/agent/chat -H "Content-Type: application/json" -H "X-Tenant-Id: tenant-contoso" -H "X-Broker-Id: broker-100" -d '{"message":"Show renewals needing attention in the next 90 days"}'

Create a draft:

    curl -X POST http://localhost:5000/agent/chat -H "Content-Type: application/json" -H "X-Tenant-Id: tenant-contoso" -H "X-Broker-Id: broker-100" -d '{"message":"Draft an email for renewal R-1001"}'

List drafts with GET /outreach/drafts and the same headers.

## Production hardening
The header-based identity adapter is for local demonstration only. Replace it with validated Microsoft Entra ID/JWT claims. Add persistent storage, audit logging, prompt-injection defenses, rate/cost limits, cancellation/timeouts, structured tool argument validation, and monitoring. Never allow the model to bypass business authorization. Keep human approval before sending customer communications.

## Interview walkthrough
1. Explain how the orchestrator turns a natural-language request into a bounded tool call.
2. Trace the tool registry and the renewal service.
3. Show that another broker's renewal is not returned, even if the caller asks for it by ID.
4. Show that outreach creation produces a Draft only.
5. Explain how IAgentModel can be implemented using Azure OpenAI tool calling.

This is an educational in-memory reference implementation, not a production insurance system. It does not send email or connect to real insurer systems.
