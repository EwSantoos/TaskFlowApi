using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Application.Interfaces.Auth;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Infra.Data;

namespace TaskFlowApi.infra.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _dbContext;

        public AuthRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario> AdicionarAsync(Usuario usuario)
        {
            _dbContext.Add(usuario);
            await _dbContext.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email) 
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
