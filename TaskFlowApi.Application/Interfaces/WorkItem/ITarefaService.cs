using TaskFlowApi.Application.Dto.WorkItem;

namespace TaskFlowApi.Application.Interfaces.WorkItem
{
    public interface ITarefaService
    {
        Task<TarefaResponse> CriarTarefaAsync(int idUser, TarefaRequest request);
        Task<TarefaResponse> AtualizarTarefaAsync(int id, TarefaRequest request);
        Task<TarefaResponse> BuscarPorIdAsync(int id);
        Task<List<TarefaResponse>> ListarAsync();
        Task ExcluirAsync(int id);
    }
}
