using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Application.Dto.User
{
    public class UsuarioLogin
    {
        [Required(ErrorMessage = "E-mail é obrigatório!"), EmailAddress(ErrorMessage = "E-mail inválido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória!")]
        public string Senha { get; set; }
    }
}
