using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Interfaces.User
{
    public interface IUsuarioService
    {
        Task<UsuarioResponse> AtualizarAsync(int id, PerfilAcessoEnum perfilLogado, UsuarioUpdateRequest request);
        Task<UsuarioResponse> BuscarPorIdAsync(int id);
        Task ExcluirAsync(int id);
        Task<List<UsuarioResponse>> ListarAsync();
    }
}
