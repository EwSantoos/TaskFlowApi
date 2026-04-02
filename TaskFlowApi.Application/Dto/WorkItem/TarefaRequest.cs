using System.ComponentModel.DataAnnotations;
using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Dto.WorkItem
{
    public class TarefaRequest
    {
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public StatusTarefaEnum Status { get; set; }
        public DateTime? DataLimite { get; set; }

        [Required(ErrorMessage = "UsuarioId é obrigatório!")]
        public int UsuarioId { get; set; }
        
    }
}
