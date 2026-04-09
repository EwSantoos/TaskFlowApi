using TaskFlowApi.Application.Dto.Pagination;
using TaskFlowApi.Application.Dto.TaskItem;

namespace TaskFlowApi.Application.Interfaces.Services
{
    public interface ITarefaService
    {
        Task<TarefaResponse> CriarTarefaAsync(int projetoId, TarefaRequest request);
        Task<TarefaResponse> AtualizarTarefaAsync(int tarefaId, TarefaUpdateRequest request);
        Task<TarefaResponse> ObterPorIdAsync(int tarefaId);
        Task<PaginacaoResponse<TarefaResponse>> ListarAsync(TarefaFiltroRequest request);
        Task ExcluirAsync(int tarefaId);
    }
}
