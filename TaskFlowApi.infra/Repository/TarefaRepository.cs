using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
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

        public async Task<Tarefa?> ObterPorIdAsync(int tarefaId)
        {
            return await _dbContext.Tarefas.Include(p => p.Projeto).Include(u => u.Usuario)
                .FirstOrDefaultAsync(t => t.Id == tarefaId);
        }

        public async Task RemoverAsync(Tarefa tarefa)
        {
            _dbContext.Tarefas.Remove(tarefa);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(List<Tarefa> Itens, int TotalItems)> ListarAsync(TarefaFiltro tarefaFiltro)
        {
            var query = _dbContext.Tarefas
                .Include(t => t.Projeto)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tarefaFiltro.Nome))
            {
                var nome = tarefaFiltro.Nome.Trim();
                query = query.Where(t => t.Titulo.Contains(nome));
            }

            if (tarefaFiltro.ProjetoId.HasValue)
            {
                query = query.Where(t => t.ProjetoId == tarefaFiltro.ProjetoId.Value);
            }

            if (tarefaFiltro.UsuarioId.HasValue)
            {
                query = query.Where(t => t.UsuarioId == tarefaFiltro.UsuarioId.Value);
            }

            if (tarefaFiltro.Status.HasValue)
            {
                query = query.Where(t => t.Status == tarefaFiltro.Status.Value);
            }

            var totalItems = await query.CountAsync();

            var itens = await query
                .OrderByDescending(t => t.Id)
                .Skip((tarefaFiltro.PageNumber - 1) * tarefaFiltro.PageSize)
                .Take(tarefaFiltro.PageSize)
                .ToListAsync();

            return (itens, totalItems);
        }

        public async Task<bool> ValidarTituloExistenteAsync(int projetoId, string titulo)
        {
            var tituloNormalizado = titulo.Trim().ToLower();

            return await _dbContext.Tarefas.AnyAsync(t =>
            t.ProjetoId == projetoId &&
            t.Titulo.ToLower() == tituloNormalizado &&
            (
                t.Status == StatusTarefaEnum.Pendente ||
                t.Status == StatusTarefaEnum.EmAndamento
            ));
        }
    }
}
