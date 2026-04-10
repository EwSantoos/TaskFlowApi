using Microsoft.AspNetCore.Http;
using TaskFlowApi.Application.Interfaces.Demo;

namespace TaskFlowApi.Infra.Security;

public class CurrentSessionService : ICurrentSessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentSessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? ObterSessionId()
    {
        return _httpContextAccessor.HttpContext?
            .User?
            .FindFirst("sessionId")?
            .Value;
    }
}