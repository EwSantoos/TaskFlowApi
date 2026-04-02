using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.WorkItem
{
    public interface ITarefaRepository
    {
        Task<Tarefa> AdicionarAsync(Tarefa projeto);
        Task<Tarefa> AtualizarAsync(Tarefa tarefa);
        Task<Tarefa> ObterPorIdAsync(int id);
        Task RemoverAsync(Tarefa tarefa);
        Task<List<Tarefa>> ListarAsync();
        Task<bool> ValidarTituloExistenteAsync(string titulo);
    }
}
