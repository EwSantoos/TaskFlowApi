using TaskFlowApi.Application.Interfaces.Demo;
using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Infra.DemoSession
{
    public class DemoSessionData : IDemoSessionContext
    {
        public List<Usuario> Usuarios { get; set; } = [];
        public List<Projeto> Projetos { get; set; } = [];
        public List<Tarefa> Tarefas { get; set; } = [];
    }
}
