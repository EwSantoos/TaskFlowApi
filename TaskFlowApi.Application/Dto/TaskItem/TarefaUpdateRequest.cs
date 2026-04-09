using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Dto.TaskItem
{
    public class TarefaUpdateRequest
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public StatusTarefaEnum? Status { get; set; }
        public DateTime? DataLimite { get; set; }
        public int? ProjetoId { get; set; }
        public int? UsuarioId { get; set; }
    }
}
