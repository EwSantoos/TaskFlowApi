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

        public static Tarefa Criar(int projetoId, int usuarioId, string titulo, StatusTarefaEnum status, string? descricao, DateTime? dataLimite)
        {
            return new Tarefa
            {
                ProjetoId = projetoId,
                UsuarioId = usuarioId,
                Titulo = titulo.Trim(),
                Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim(),
                Status = status,
                DataLimite = dataLimite

            };
        }

        public void Atualizar(int? projetoId, int? usuarioId, string? titulo, string? descricao, StatusTarefaEnum? status, DateTime? dataLimite)
        {
            if (projetoId.HasValue) 
            {
                ProjetoId = projetoId.Value;
            }

            if (usuarioId.HasValue) 
            {
                UsuarioId = usuarioId.Value;
            }

            if (status.HasValue) 
            {
                Status = status.Value;
            }

            if (titulo is not null)
            {
                Titulo = titulo.Trim();
            }

            if (descricao is not null)
            {
                Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
            }

            if (dataLimite.HasValue) 
            {
                DataLimite = dataLimite;
            }
        }
    }
}
