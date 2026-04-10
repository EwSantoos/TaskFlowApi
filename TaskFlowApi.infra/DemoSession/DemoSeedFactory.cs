using System.Reflection;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Infra.DemoSession;

public class DemoSeedFactory
{
    public DemoSessionData Criar()
    {
        var session = new DemoSessionData();

        CriarUsuariosBase(session);
        CriarProjetosBase(session);
        CriarTarefasBase(session);

        return session;
    }

    private void CriarUsuariosBase(DemoSessionData session)
    {
        var admin = Usuario.Criar("AdminDemo", "admin@teste.com", PerfilAcessoEnum.Administrador, Array.Empty<byte>(), Array.Empty<byte>());
        DefinirPropriedade(admin, nameof(Usuario.Id), 1);

        var operacional = Usuario.Criar("OperacionalDemo", "operacional@teste.com", PerfilAcessoEnum.Operacional, Array.Empty<byte>(), Array.Empty<byte>());
        DefinirPropriedade(operacional, nameof(Usuario.Id), 2);

        var consulta = Usuario.Criar("ConsultaDemo", "consulta@teste.com", PerfilAcessoEnum.Consulta, Array.Empty<byte>(), Array.Empty<byte>());
        DefinirPropriedade(consulta, nameof(Usuario.Id), 3);

        var usuarioDemo = Usuario.Criar("UsuarioDemo", "usuariodemo@teste.com", PerfilAcessoEnum.Operacional, Array.Empty<byte>(), Array.Empty<byte>());
        DefinirPropriedade(usuarioDemo, nameof(Usuario.Id), 4);

        var testeDemo = Usuario.Criar("TesteDemo", "testedemo@teste.com", PerfilAcessoEnum.Operacional, Array.Empty<byte>(), Array.Empty<byte>());
        DefinirPropriedade(testeDemo, nameof(Usuario.Id), 5);

        session.Usuarios.AddRange([admin, operacional, consulta, usuarioDemo, testeDemo]);
    }

    private void CriarProjetosBase(DemoSessionData session)
    {
        session.Projetos.Add(CriarProjeto(session, 1, 1, "Site do sistema", "Interface web pra gerenciar tarefas."));
        session.Projetos.Add(CriarProjeto(session, 2, 1, "API principal", "Responsável por autenticação e regras do sistema."));
        session.Projetos.Add(CriarProjeto(session, 3, 2, "Painel admin", "Tela pra acompanhar usuários e dados gerais."));
        session.Projetos.Add(CriarProjeto(session, 4, 2, "App mobile", "Versão mobile pra acessar tarefas."));
        session.Projetos.Add(CriarProjeto(session, 5, 3, "Integrações externas", "Conectar com outros serviços."));
        session.Projetos.Add(CriarProjeto(session, 6, 4, "Ajustes backend", "Melhorias internas no código."));
        session.Projetos.Add(CriarProjeto(session, 7, 5, "Monitoramento", "Logs e métricas do sistema."));
        session.Projetos.Add(CriarProjeto(session, 8, 5, "Testes automatizados", "Cobertura de testes da aplicação."));
    }

    private void CriarTarefasBase(DemoSessionData session)
    {
        session.Tarefas.Add(CriarTarefa(session, 1, 1, 1, "Tela de login", StatusTarefaEnum.Concluida, "Criar tela inicial de login.", DateTime.UtcNow.AddDays(3)));
        session.Tarefas.Add(CriarTarefa(session, 2, 1, 2, "Listar usuários", StatusTarefaEnum.EmAndamento, "Mostrar usuários cadastrados.", DateTime.UtcNow.AddDays(5)));
        session.Tarefas.Add(CriarTarefa(session, 3, 2, 1, "CRUD de projetos", StatusTarefaEnum.Pendente, "Criar endpoints de projeto.", DateTime.UtcNow.AddDays(7)));
        session.Tarefas.Add(CriarTarefa(session, 4, 2, 2, "JWT funcionando", StatusTarefaEnum.Concluida, "Proteger rotas com autenticação.", DateTime.UtcNow.AddDays(2)));
        session.Tarefas.Add(CriarTarefa(session, 5, 3, 3, "Dashboard inicial", StatusTarefaEnum.EmAndamento, "Mostrar resumo de dados.", DateTime.UtcNow.AddDays(4)));
        session.Tarefas.Add(CriarTarefa(session, 6, 2, 1, "Organizar services", StatusTarefaEnum.EmAndamento, "Melhorar estrutura dos services.", DateTime.UtcNow.AddDays(6)));
        session.Tarefas.Add(CriarTarefa(session, 7, 2, 2, "Padronizar retorno", StatusTarefaEnum.Pendente, "Ajustar resposta da API.", DateTime.UtcNow.AddDays(8)));
        session.Tarefas.Add(CriarTarefa(session, 8, 3, 3, "Ajustar layout", StatusTarefaEnum.Pendente, "Melhorar visual do painel.", DateTime.UtcNow.AddDays(-2)));
        session.Tarefas.Add(CriarTarefa(session, 9, 1, 1, "Melhorar UI", StatusTarefaEnum.EmAndamento, "Pequenos ajustes visuais.", DateTime.UtcNow.AddDays(3)));
        session.Tarefas.Add(CriarTarefa(session, 10, 5, 2, "Integração externa", StatusTarefaEnum.Concluida, "Consumir API de terceiros.", DateTime.UtcNow.AddDays(1)));
    }   

    private Projeto CriarProjeto(DemoSessionData session, int id, int usuarioCriadorId, string nome, string descricao)
    {
        var projeto = Projeto.Criar(usuarioCriadorId, nome, descricao);

        var usuario = session.Usuarios.First(u => u.Id == usuarioCriadorId);

        DefinirPropriedade(projeto, nameof(Projeto.Id), id);
        DefinirPropriedade(projeto, nameof(Projeto.UsuarioCriador), usuario);

        return projeto;
    }

    private Tarefa CriarTarefa(DemoSessionData session, int id, int projetoId, int usuarioId, string titulo, StatusTarefaEnum status, string? descricao, DateTime? dataLimite)
    {
        var tarefa = Tarefa.Criar(projetoId, usuarioId, titulo, status, descricao, dataLimite);

        var projeto = session.Projetos.First(p => p.Id == projetoId);
        var usuario = session.Usuarios.First(u => u.Id == usuarioId);

        DefinirPropriedade(tarefa, nameof(Tarefa.Id), id);
        DefinirPropriedade(tarefa, nameof(Tarefa.Projeto), projeto);
        DefinirPropriedade(tarefa, nameof(Tarefa.Usuario), usuario);

        return tarefa;
    }

    private static void DefinirPropriedade<T>(T objeto, string propriedade, object valor)
    {
        var prop = typeof(T).GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        prop?.SetValue(objeto, valor);
    }
}