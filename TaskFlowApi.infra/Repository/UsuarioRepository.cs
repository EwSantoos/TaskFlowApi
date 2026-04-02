using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Application.Interfaces.User;
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

        public async Task<Usuario> AtualizarAsync(Usuario usuario)
        {
            _dbContext.Usuarios.Update(usuario);
            await _dbContext.SaveChangesAsync();

            return usuario;
        }
      
        public async Task<Usuario> ObterPorIdAsync(int id) 
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task RemoverAsync(Usuario usuario)
        {
            _dbContext.Usuarios.Remove(usuario);
            await _dbContext.SaveChangesAsync();
        }        

        public async Task<List<Usuario>> ListarAsync()
        {
            return await _dbContext.Usuarios.AsNoTracking().ToListAsync();
        }

        public async Task<bool> ValidarEmailExistente(string email)
        {
            return await _dbContext.Usuarios.AnyAsync(u => u.Email == email.Trim().ToLowerInvariant());
        }
    }
}
