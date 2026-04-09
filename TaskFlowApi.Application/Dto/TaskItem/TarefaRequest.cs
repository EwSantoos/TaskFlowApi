using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Dto.TaskItem
{
    public class TarefaRequest
    {
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public StatusTarefaEnum Status { get; set; }
        public DateTime? DataLimite { get; set; }
        public int UsuarioId { get; set; }
        
    }
}
