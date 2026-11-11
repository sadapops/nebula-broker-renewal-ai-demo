using Nebula.BrokerRenewal.Api.Domain;

namespace Nebula.BrokerRenewal.Api.Infrastructure;

public interface IUserContextAccessor
{
    UserContext GetCurrent();
}

// Local demo only. Production must derive identity from validated JWT/Entra ID claims.
public sealed class HeaderUserContextAccessor(IHttpContextAccessor accessor) : IUserContextAccessor
{
    public UserContext GetCurrent()
    {
        var http = accessor.HttpContext ?? throw new InvalidOperationException("No HTTP context.");
        var tenant = http.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        var broker = http.Request.Headers["X-Broker-Id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenant) || string.IsNullOrWhiteSpace(broker))
            throw new UnauthorizedAccessException("X-Tenant-Id and X-Broker-Id are required for this demo.");
        return new UserContext(tenant, broker, $"{tenant}:{broker}");
    }
}
