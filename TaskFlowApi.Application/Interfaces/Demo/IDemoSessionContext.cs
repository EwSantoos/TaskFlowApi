using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Application.Interfaces.Demo
{
    public interface IDemoSessionContext
    {
        List<Usuario> Usuarios { get; }
        List<Projeto> Projetos { get; }
        List<Tarefa> Tarefas { get; }
    }
}
