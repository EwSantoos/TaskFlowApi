using TaskFlowApi.Application.Filters;
using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.Repositories
{
    public interface ITarefaRepository
    {
        Task<Tarefa> AdicionarAsync(Tarefa projeto);
        Task<Tarefa> AtualizarAsync(Tarefa tarefa);
        Task<Tarefa?> ObterPorIdAsync(int id);
        Task RemoverAsync(Tarefa tarefa);
        Task<(List<Tarefa> Itens, int TotalItems)> ListarAsync(TarefaFiltro tarefaFiltro);
        Task<bool> ValidarTituloExistenteAsync(int projetoId, string titulo);
    }
}
