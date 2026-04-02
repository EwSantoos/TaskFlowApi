using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public PerfilAcessoEnum Perfil { get; private set; }
        public byte[] PasswordHash { get; private set; }
        public byte[] PasswordSalt { get; private set; }
        public DateTime CriadoEm { get; private set; } = DateTime.UtcNow;

        public static Usuario Criar(string nome, string email, PerfilAcessoEnum perfil, byte[] senhaHash, byte[] senhaSalt) 
        {
            return new Usuario
            {
                Nome = nome.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                Perfil = perfil,
                PasswordHash = senhaHash,
                PasswordSalt = senhaSalt
            };
        }

        public void Atualizar(string? nome, string? email, PerfilAcessoEnum? perfil) 
        {
            if(!string.IsNullOrWhiteSpace(nome))
                Nome = nome.Trim();

            if(!string.IsNullOrWhiteSpace(email))
                Email = email.Trim().ToLowerInvariant();

            Perfil = perfil ?? Perfil;
        }
    }
}
