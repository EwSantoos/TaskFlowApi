using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Infra.Data;

namespace TaskFlowApi.infra.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _dbContext;
        public UsuarioRepository(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario> AdicionarAsync(Usuario usuario)
        {
            _dbContext.Add(usuario);
            await _dbContext.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario> AtualizarAsync(Usuario usuario)
        {
            _dbContext.Usuarios.Update(usuario);
            await _dbContext.SaveChangesAsync();

            return usuario;
        }
      
        public async Task<Usuario?> ObterPorIdAsync(int idUsuario) 
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(a => a.Id == idUsuario);
        }

        public async Task RemoverAsync(Usuario usuario)
        {
            _dbContext.Usuarios.Remove(usuario);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(List<Usuario> Itens, int TotalItens)> ListarAsync(UsuarioFiltro usuariofiltro)
        {
            var query = _dbContext.Usuarios.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(usuariofiltro.Nome))
            {
                var nome = usuariofiltro.Nome.Trim();
                query = query.Where(u => u.Nome.Contains(nome));
            }

            if (!string.IsNullOrWhiteSpace(usuariofiltro.Email))
            {
                var email = usuariofiltro.Email.Trim().ToLower();
                query = query.Where(u => u.Email.Contains(email));
            }

            if (usuariofiltro.Perfil.HasValue)
            {
                query = query.Where(u => u.Perfil == usuariofiltro.Perfil);
            }

            var totalItens = await query.CountAsync();

            var itens = await query
                .OrderByDescending(u => u.Id)
                .Skip((usuariofiltro.PageNumber - 1) * usuariofiltro.PageSize)
                .Take(usuariofiltro.PageSize)
                .ToListAsync();

            return (itens, totalItens);
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            email = email.Trim().ToLowerInvariant();

            return await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistente(string email)
        {
            return await _dbContext.Usuarios.AnyAsync(u => u.Email == email.Trim().ToLowerInvariant());
        }
    }
}
