using System.Text.RegularExpressions;
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
            if(nome is not null)
                Nome = nome.Trim();

            if(email is not null)
                Email = email.Trim().ToLowerInvariant();

            Perfil = perfil ?? Perfil;
        }

        public static bool EmailValido(string email)
        {
            // 1. vazio ou nulo
            if (string.IsNullOrWhiteSpace(email))
                return false;

            email = email.Trim();

            //regex base (formato geral)
            var formatoValido = Regex.IsMatch(
                email,
                @"^[a-zA-Z0-9_%+\-]+(\.[a-zA-Z0-9_%+\-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+$"
            );

            if (!formatoValido)
                return false;

            if (email.Contains(".."))
                return false;

            //separa local e domínio
            var partes = email.Split('@');

            var local = partes[0];
            var dominio = partes[1];

            //local não pode começar ou terminar com ponto
            if (local.StartsWith(".") || local.EndsWith("."))
                return false;

            //domínio não pode começar ou terminar com ponto
            if (dominio.StartsWith(".") || dominio.EndsWith("."))
                return false;

            return true;
        }

        public static bool SenhaValida(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return false;

            if (senha.Length < 8)
                return false;

            if (!senha.Any(char.IsLetter))
                return false;

            if (!senha.Any(char.IsDigit))
                return false;

            return true;
        }
    }
}
