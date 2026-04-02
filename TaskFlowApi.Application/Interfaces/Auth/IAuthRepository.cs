using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.Auth
{
    public interface IAuthRepository
    {
        Task<Usuario> AdicionarAsync(Usuario usuario);
        Task<Usuario?> ObterPorEmailAsync(string email);
    }
}
