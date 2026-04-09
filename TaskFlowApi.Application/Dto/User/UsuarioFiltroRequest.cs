using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Dto.User
{
    public class UsuarioFiltroRequest
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public PerfilAcessoEnum? Perfil { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
