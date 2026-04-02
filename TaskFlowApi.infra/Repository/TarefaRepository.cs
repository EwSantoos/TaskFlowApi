using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Application.Interfaces.WorkItem;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Infra.Data;

namespace TaskFlowApi.infra.Repository
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly AppDbContext _dbContext;

        public TarefaRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tarefa> AdicionarAsync(Tarefa tarefa)
        {
            _dbContext.Tarefas.Add(tarefa);
            await _dbContext.SaveChangesAsync();

            return tarefa;
        }

        public async Task<Tarefa> AtualizarAsync(Tarefa tarefa)
        {
            _dbContext.Tarefas.Update(tarefa);
            await _dbContext.SaveChangesAsync();

            return tarefa;
        }
        public async Task<Tarefa> ObterPorIdAsync(int id)
        {
            return await _dbContext.Tarefas.Include(p => p.Projeto).Include(u => u.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task RemoverAsync(Tarefa tarefa)
        {
            _dbContext.Tarefas.Remove(tarefa);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Tarefa>> ListarAsync()
        {
            return await _dbContext.Tarefas.Include(p => p.Projeto).Include(u => u.Usuario)
                .AsNoTracking().ToListAsync();
        }

        public async Task<bool> ValidarTituloExistenteAsync(string titulo)
        {
            return await _dbContext.Tarefas.AnyAsync(t => t.Titulo.Trim().ToLower() == titulo.Trim().ToLower());
        }
    }
}
