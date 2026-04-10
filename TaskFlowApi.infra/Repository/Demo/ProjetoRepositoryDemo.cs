using System.Reflection;
using TaskFlowApi.Application.Interfaces.Demo;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Domain.Entities;

namespace TaskFlowApi.Infra.Repository.Demo;

public class ProjetoRepositoryDemo : IProjetoRepository
{
    private readonly IDemoSessionService _demoSessionService;
    private readonly ICurrentSessionService _currentSessionService;

    public ProjetoRepositoryDemo(IDemoSessionService demoSessionService, ICurrentSessionService currentSessionService)
    {
        _demoSessionService = demoSessionService;
        _currentSessionService = currentSessionService;
    }

    private (List<Projeto> projetos, List<Usuario> usuarios) ObterDadosSessao()
    {
        var sessionId = _currentSessionService.ObterSessionId();

        if (string.IsNullOrWhiteSpace(sessionId))
            throw new Exception("Sessão não encontrada para projetos.");

        var session = _demoSessionService.ObterSessao(sessionId);

        return (session.Projetos, session.Usuarios);
    }

    private static void DefinirPropriedade(object entidade, string nomePropriedade, object valor)
    {
        var property = entidade.GetType().GetProperty(
            nomePropriedade,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null)
            throw new InvalidOperationException($"Propriedade {nomePropriedade} não encontrada.");

        property.SetValue(entidade, valor);
    }

    public Task<Projeto> AdicionarAsync(Projeto projeto)
    {
        var (projetos, usuarios) = ObterDadosSessao();

        var novoId = projetos.Any() ? projetos.Max(p => p.Id) + 1 : 1;
        var usuario = usuarios.FirstOrDefault(u => u.Id == projeto.UsuarioCriadorId);

        DefinirPropriedade(projeto, nameof(Projeto.Id), novoId);

        if (usuario is not null)
            DefinirPropriedade(projeto, nameof(Projeto.UsuarioCriador), usuario);

        projetos.Add(projeto);

        return Task.FromResult(projeto);
    }

    public Task<Projeto> AtualizarAsync(Projeto projeto)
    {
        var (projetos, _) = ObterDadosSessao();

        var existente = projetos.FirstOrDefault(p => p.Id == projeto.Id);

        if (existente is null)
            return Task.FromResult(projeto);

        existente.Atualizar(projeto.Nome, projeto.Descricao);

        return Task.FromResult(existente);
    }

    public Task<Projeto?> ObterPorIdAsync(int projetoId)
    {
        var (projetos, _) = ObterDadosSessao();

        var projeto = projetos.FirstOrDefault(p => p.Id == projetoId);

        return Task.FromResult(projeto);
    }

    public Task RemoverAsync(Projeto projeto)
    {
        var (projetos, _) = ObterDadosSessao();

        var existente = projetos.FirstOrDefault(p => p.Id == projeto.Id);

        if (existente is not null)
            projetos.Remove(existente);

        return Task.CompletedTask;
    }

    public Task<List<Projeto>> ListarAsync()
    {
        var (projetos, _) = ObterDadosSessao();

        var lista = projetos
            .OrderByDescending(p => p.Id)
            .ToList();

        return Task.FromResult(lista);
    }

    public Task<bool> ValidarNomeExistenteAsync(string nome)
    {
        var (projetos, _) = ObterDadosSessao();

        nome = nome.Trim();

        var existe = projetos.Any(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(existe);
    }
}