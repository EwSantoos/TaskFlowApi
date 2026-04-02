using TaskFlowApi.Application.Dto.User;

namespace TaskFlowApi.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<UsuarioResponse> CriarUsuarioAsync(UsuarioRequest request);
        Task<string> LoginAsync(UsuarioLogin login);
    }
}
