using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Application.Filters
{
    public class TarefaFiltro
    {
        public string? Nome { get; set; }
        public int? ProjetoId { get; set; }
        public int? UsuarioId { get; set; }
        public StatusTarefaEnum? Status { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
