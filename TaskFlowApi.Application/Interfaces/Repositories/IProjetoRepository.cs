using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.Repositories
{
    public interface IProjetoRepository
    {
        Task<Projeto> AdicionarAsync(Projeto projeto);
        Task<Projeto> AtualizarAsync(Projeto projeto);
        Task<Projeto?> ObterPorIdAsync(int projetoId);
        Task RemoverAsync(Projeto projeto);
        Task<List<Projeto>> ListarAsync();
        Task<bool> ValidarNomeExistenteAsync(string nome);
    }
}
