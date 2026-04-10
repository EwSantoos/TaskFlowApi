using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string CriarToken(Usuario usuario, string sessionId);
    }
}
