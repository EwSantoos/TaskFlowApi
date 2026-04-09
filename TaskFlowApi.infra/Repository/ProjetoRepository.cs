using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Infra.Data;

namespace TaskFlowApi.infra.Repository
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly AppDbContext _dbContext;

        public ProjetoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Projeto> AdicionarAsync(Projeto projeto) 
        {
            _dbContext.Projetos.Add(projeto);
            await _dbContext.SaveChangesAsync();

            return projeto;
        }

        public async Task<Projeto> AtualizarAsync(Projeto projeto)
        {
            _dbContext.Projetos.Update(projeto);
            await _dbContext.SaveChangesAsync();

            return projeto;
        }

        public async Task<List<Projeto>> ListarAsync()
        {
            return await _dbContext.Projetos.Include(u => u.UsuarioCriador).AsNoTracking().ToListAsync();
        }

        public async Task<Projeto?> ObterPorIdAsync(int projetoId)
        {
            return await _dbContext.Projetos.Include(u => u.UsuarioCriador).Include(t => t.Tarefas).ThenInclude(t => t.Usuario)
                .FirstOrDefaultAsync(x => x.Id == projetoId);
        }

        public async Task RemoverAsync(Projeto projeto)
        {
            _dbContext.Projetos.Remove(projeto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ValidarNomeExistenteAsync(string nome)
        {
            var nomeNormalizado = nome.Trim().ToLower();

            return await _dbContext.Projetos.AnyAsync(p => p.Nome.ToLower() == nomeNormalizado);
        }
    }
}
