using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Dto.User
{
    public class UsuarioUpdateRequest
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public PerfilAcessoEnum? Perfil { get; set; }
    }
}
