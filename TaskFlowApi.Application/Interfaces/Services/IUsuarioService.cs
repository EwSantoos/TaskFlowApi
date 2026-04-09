using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Dto.Pagination;

namespace TaskFlowApi.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioResponse> CriarUsuarioAsync(UsuarioRequest request);
        Task<UsuarioResponse> AtualizarAsync(int idUsuario, int usuarioLogadoId, UsuarioUpdateRequest request);
        Task<UsuarioResponse> ObterPorIdAsync(int idUsuario);
        Task ExcluirAsync(int idUsuario);
        Task<PaginacaoResponse<UsuarioResponse>> ListarAsync(UsuarioFiltroRequest request);
    }
}
