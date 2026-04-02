using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Dto.WorkItem
{
    public class TarefaResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public StatusTarefaEnum Status { get; set; }
        public int ProjetoId { get; set; }
        public string NomeProjeto { get; set; }
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? DataLimite { get; set; }
    }
}
