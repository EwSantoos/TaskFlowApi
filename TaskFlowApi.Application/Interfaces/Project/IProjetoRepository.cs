using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.Project
{
    public interface IProjetoRepository
    {
        Task<Projeto> AdicionarAsync(Projeto projeto);
        Task<Projeto> AtualizarAsync(Projeto projeto);
        Task<Projeto> ObterPorIdAsync(int id);
        Task RemoverAsync(Projeto projeto);
        Task<List<Projeto>> ListarAsync();
        Task<bool> ValidarNomeExistenteAsync(string nome);
    }
}
