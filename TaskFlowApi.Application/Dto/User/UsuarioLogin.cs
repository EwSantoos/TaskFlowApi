using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Application.Dto.User
{
    public class UsuarioLogin
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
