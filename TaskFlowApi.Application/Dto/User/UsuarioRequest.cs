using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Application.Dto.User
{
    public class UsuarioRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório!")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "E-mail é obrigatório!"), EmailAddress(ErrorMessage = "E-mail inválido!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; }
        [Compare("Senha", ErrorMessage = "Senhas não coincidem!")]
        public string ConfirmaSenha { get; set; }
        [Required(ErrorMessage = "Tipo do perfil é obrigatório!")]
        public string Perfil { get; set; }
    }
}
