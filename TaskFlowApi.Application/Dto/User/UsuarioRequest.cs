using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Application.Dto.User
{
    public class UsuarioRequest
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string ConfirmaSenha { get; set; }
        public string Perfil { get; set; }
    }
}
