using TaskFlowApi.Application.Dto.Token;
using TaskFlowApi.Application.Dto.User;

namespace TaskFlowApi.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<TokenResponse> TokenAsync(UsuarioLogin login);
    }
}
