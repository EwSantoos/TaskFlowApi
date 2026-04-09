using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt);
        bool ValidarSenhaHash(string senha, byte[] senhaHash, byte[] senhaSalt);
    }
}
