using System.Security.Cryptography;
using TaskFlowApi.Application.Interfaces.Auth;

namespace TaskFlowApi.Infra.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt) 
        {
            using (var hmac = new HMACSHA512()) 
            {
                senhaSalt = hmac.Key;
                senhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
            }
        }

        public bool ValidarSenhaHash(string senha, byte[] senhaHash, byte[] senhaSalt) 
        {
            using (var hmac = new HMACSHA512(senhaSalt)) 
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
                return computeHash.SequenceEqual(senhaHash);
            }
        }
    }
}
