using Nebula.BrokerRenewal.Api.Agents;
using Nebula.BrokerRenewal.Api.Contracts;
using Nebula.BrokerRenewal.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<DemoDataStore>();
builder.Services.AddScoped<IUserContextAccessor, HeaderUserContextAccessor>();
builder.Services.AddScoped<IRenewalService, RenewalService>();
builder.Services.AddScoped<IOutreachService, OutreachService>();
builder.Services.AddSingleton<IAgentModel, DeterministicDemoModel>();
builder.Services.AddScoped<IToolRegistry, RenewalToolRegistry>();
builder.Services.AddScoped<RenewalAgent>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { name = "Nebula Broker Renewal AI Demo", status = "running" }));

app.MapPost("/agent/chat", async (ChatRequest request, RenewalAgent agent, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
        return Results.BadRequest(new { error = "Message is required." });
    return Results.Ok(await agent.HandleAsync(request.Message, ct));
});

app.MapGet("/renewals", async (IRenewalService service, CancellationToken ct) =>
    Results.Ok(await service.ListNeedingAttentionAsync(90, ct)));

app.MapGet("/outreach/drafts", async (IOutreachService service, CancellationToken ct) =>
    Results.Ok(await service.GetDraftsAsync(ct)));

app.Run();

public partial class Program;
