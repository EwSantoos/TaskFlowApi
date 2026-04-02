using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Dto.User
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public PerfilAcessoEnum Perfil { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
