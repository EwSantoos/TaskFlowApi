using TaskFlowApi.Application.Dto.TaskItem;

namespace TaskFlowApi.Application.Dto.Project
{
    public class ProjetoResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public int UsuarioCriadorId { get; set; }
        public string NomeCriador { get; set; }
        public DateTime CriadoEm { get; set; }
        public List<TarefaResponse> Tarefas { get; set; } = new();

    }
}
