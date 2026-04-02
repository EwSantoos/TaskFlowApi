namespace TaskFlowApi.Domain.Entities
{
    public class Projeto
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string? Descricao { get; private set; }
        public List<Tarefa> Tarefas { get; private set; } = new();
        public DateTime CriadoEm { get; private set; } = DateTime.UtcNow;

        public int UsuarioCriadorId { get; private set; }
        public Usuario UsuarioCriador { get; private set; }

        public static Projeto Criar(int idUser, string nome, string? descricao) 
        {
            return new Projeto 
            { 
                Nome = nome, 
                Descricao = descricao,
                UsuarioCriadorId = idUser
            };
        }

        public void Atualizar(string nome, string descricao) 
        {
            if (!string.IsNullOrWhiteSpace(nome)) 
            {
                Nome = nome;
            }

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                Descricao = descricao;
            }
        }
    }
}
