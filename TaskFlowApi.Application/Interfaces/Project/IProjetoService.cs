using TaskFlowApi.Application.Dto.Project;

namespace TaskFlowApi.Application.Interfaces.Project
{
    public interface IProjetoService
    {
        Task<ProjetoResponse> CriarProjetoAsync(int idUser, ProjetoRequest request);
        Task<ProjetoResponse> AtualizarProjetoAsync(int id, ProjetoRequest request);
        Task<ProjetoResponse> BuscarPorIdAsync(int id);
        Task<List<ProjetoResponse>> ListarAsync();
        Task ExcluirAsync(int id);
    }
}
