using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Domain.Entities
{
    public class Tarefa
    {
        public int Id { get; private set; }
        public string Titulo { get; private set; }
        public string? Descricao { get; private set; }
        public StatusTarefaEnum Status { get; private set; } = StatusTarefaEnum.Pendente;
        public DateTime CriadoEm { get; private set; } = DateTime.UtcNow;
        public DateTime? DataLimite { get; private set; }

        public int ProjetoId { get; private set; }
        public Projeto Projeto { get; private set; }

        public int UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; }

        public static Tarefa Criar(int projetoId, int usuarioId, string titulo, string? descricao, DateTime? dataLimite)
        {
            return new Tarefa
            {
                ProjetoId = projetoId,
                UsuarioId = usuarioId,
                Titulo = titulo,
                Descricao = descricao,
                DataLimite = dataLimite

            };
        }

        public void Atualizar(int usuarioId, string titulo, string? descricao, DateTime? dataLimite)
        {
            UsuarioId = usuarioId;

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                Titulo = titulo;
            }

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                Descricao = descricao;
            }

            if (dataLimite.HasValue) 
            {
                DataLimite = dataLimite;
            }
        }
    }
}
