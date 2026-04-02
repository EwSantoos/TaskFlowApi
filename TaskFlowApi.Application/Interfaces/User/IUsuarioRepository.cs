using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.User
{
    public interface IUsuarioRepository
    {
        Task<Usuario> AtualizarAsync(Usuario usuario);
        Task<Usuario> ObterPorIdAsync(int id);
        Task RemoverAsync(Usuario usuario);
        Task<List<Usuario>> ListarAsync();
        Task<bool> ValidarEmailExistente(string email);
    }
}
