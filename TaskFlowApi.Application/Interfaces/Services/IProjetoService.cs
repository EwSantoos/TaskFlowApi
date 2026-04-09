using TaskFlowApi.Application.Dto.Project;

namespace TaskFlowApi.Application.Interfaces.Services
{
    public interface IProjetoService
    {
        Task<ProjetoResponse> CriarProjetoAsync(int usuarioLogadoId, ProjetoRequest request);
        Task<ProjetoResponse> AtualizarProjetoAsync(int projetoId, ProjetoUpdateRequest request);
        Task<ProjetoResponse> ObterPorIdAsync(int projetoId);
        Task<List<ProjetoResponse>> ListarAsync();
        Task ExcluirAsync(int projetoId);
    }
}
