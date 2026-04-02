using System.ComponentModel.DataAnnotations;
using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Dto.User
{
    public class UsuarioUpdateRequest
    {
        public string? Nome { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido!")]
        public string? Email { get; set; }
        public PerfilAcessoEnum? Perfil { get; set; }
    }
}
