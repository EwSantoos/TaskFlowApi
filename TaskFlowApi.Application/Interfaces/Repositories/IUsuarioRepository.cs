using System.Threading.Tasks;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> AdicionarAsync(Usuario usuario);
        Task<Usuario> AtualizarAsync(Usuario usuario);
        Task<Usuario?> ObterPorIdAsync(int idUsuario);
        Task RemoverAsync(Usuario usuario);
        Task<(List<Usuario> Itens, int TotalItens)> ListarAsync(UsuarioFiltro usuarioFiltro);
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<bool> EmailExistente(string email);
    }
}
