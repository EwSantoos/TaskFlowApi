using TaskFlowApi.Application.Interfaces.Demo;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;
using TaskFlowApi.infra.DemoSession;

namespace TaskFlowApi.Infra.DemoSession;

public class DemoSessionStore : IDemoSessionService
{
    private static readonly Dictionary<string, DemoSessionWrapper> _sessions = new();
    private static readonly object _lock = new();

    private readonly DemoSeedFactory _seedFactory;

    public DemoSessionStore(DemoSeedFactory seedFactory)
    {
        _seedFactory = seedFactory;
    }

    public string CriarSessao()
    {
        var sessionId = Guid.NewGuid().ToString();

        lock (_lock)
        {
            _sessions[sessionId] = new DemoSessionWrapper
            {
                Data = _seedFactory.Criar(),
                CriadoEm = DateTime.UtcNow
            };
        }

        return sessionId;
    }

    public IDemoSessionContext ObterSessao(string sessionId)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out var wrapper))
                throw new DomainException("Sessão não encontrada.", ErrorTypeEnum.Unauthorized);

            var expirado = DateTime.UtcNow - wrapper.CriadoEm > TimeSpan.FromHours(1);

            if (expirado)
            {
                _sessions.Remove(sessionId);
                throw new DomainException("Sessão expirada.", ErrorTypeEnum.Unauthorized);
            }

            return wrapper.Data;
        }
    }

    public bool ExisteSessao(string sessionId)
    {
        lock (_lock)
        {
            return _sessions.ContainsKey(sessionId);
        }
    }

    public void RemoverSessao(string sessionId)
    {
        lock (_lock)
        {
            _sessions.Remove(sessionId);
        }
    }
}