using System.Reflection;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Demo;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Infra.Repository.Demo;

public class TarefaRepositoryDemo : ITarefaRepository
{
    private readonly IDemoSessionService _demoSessionService;
    private readonly ICurrentSessionService _currentSessionService;

    public TarefaRepositoryDemo(IDemoSessionService demoSessionService, ICurrentSessionService currentSessionService)
    {
        _demoSessionService = demoSessionService;
        _currentSessionService = currentSessionService;
    }

    private (List<Tarefa> tarefas, List<Usuario> usuarios, List<Projeto> projetos) ObterDadosSessao()
    {
        var sessionId = _currentSessionService.ObterSessionId();

        if (string.IsNullOrWhiteSpace(sessionId))
            throw new DomainException("Sessão não encontrada para tarefas.", ErrorTypeEnum.Unauthorized);

        var session = _demoSessionService.ObterSessao(sessionId);

        return (session.Tarefas, session.Usuarios, session.Projetos);
    }

    private static void DefinirPropriedade(object entidade, string nomePropriedade, object valor)
    {
        var prop = entidade.GetType().GetProperty(nomePropriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (prop is null)
            throw new InvalidOperationException($"Propriedade {nomePropriedade} não encontrada.");

        prop.SetValue(entidade, valor);
    }

    public Task<Tarefa> AdicionarAsync(Tarefa tarefa)
    {
        var (tarefas, usuarios, projetos) = ObterDadosSessao();

        var novoId = tarefas.Any() ? tarefas.Max(t => t.Id) + 1 : 1;

        var projeto = projetos.FirstOrDefault(p => p.Id == tarefa.ProjetoId);
        var usuario = usuarios.FirstOrDefault(u => u.Id == tarefa.UsuarioId);

        DefinirPropriedade(tarefa, nameof(Tarefa.Id), novoId);

        if (projeto is not null)
            DefinirPropriedade(tarefa, nameof(Tarefa.Projeto), projeto);

        if (usuario is not null)
            DefinirPropriedade(tarefa, nameof(Tarefa.Usuario), usuario);

        tarefas.Add(tarefa);

        return Task.FromResult(tarefa);
    }

    public Task<Tarefa> AtualizarAsync(Tarefa tarefa)
    {
        var (tarefas, usuarios, projetos) = ObterDadosSessao();

        var existente = tarefas.FirstOrDefault(t => t.Id == tarefa.Id);

        if (existente is null)
            return Task.FromResult(tarefa);

        existente.Atualizar(
            tarefa.ProjetoId,
            tarefa.UsuarioId,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Status,
            tarefa.DataLimite);

        var projeto = projetos.FirstOrDefault(p => p.Id == existente.ProjetoId);
        var usuario = usuarios.FirstOrDefault(u => u.Id == existente.UsuarioId);

        if (projeto is not null)
            DefinirPropriedade(existente, nameof(Tarefa.Projeto), projeto);

        if (usuario is not null)
            DefinirPropriedade(existente, nameof(Tarefa.Usuario), usuario);

        return Task.FromResult(existente);
    }

    public Task<Tarefa?> ObterPorIdAsync(int id)
    {
        var (tarefas, usuarios, projetos) = ObterDadosSessao();

        var tarefa = tarefas.FirstOrDefault(t => t.Id == id);

        if (tarefa is null)
            return Task.FromResult<Tarefa?>(null);

        var projeto = projetos.FirstOrDefault(p => p.Id == tarefa.ProjetoId);
        var usuario = usuarios.FirstOrDefault(u => u.Id == tarefa.UsuarioId);

        if (projeto is not null)
            DefinirPropriedade(tarefa, nameof(Tarefa.Projeto), projeto);

        if (usuario is not null)
            DefinirPropriedade(tarefa, nameof(Tarefa.Usuario), usuario);

        return Task.FromResult<Tarefa?>(tarefa);
    }

    public Task RemoverAsync(Tarefa tarefa)
    {
        var (tarefas, _, _) = ObterDadosSessao();

        var existente = tarefas.FirstOrDefault(t => t.Id == tarefa.Id);

        if (existente is not null)
            tarefas.Remove(existente);

        return Task.CompletedTask;
    }

    public Task<(List<Tarefa> Itens, int TotalItems)> ListarAsync(TarefaFiltro tarefaFiltro)
    {
        var (tarefas, _, _) = ObterDadosSessao();

        var query = tarefas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(tarefaFiltro.Nome))
        {
            var nome = tarefaFiltro.Nome.Trim();
            query = query.Where(t => t.Titulo.Contains(nome, StringComparison.OrdinalIgnoreCase));
        }

        if (tarefaFiltro.ProjetoId.HasValue)
            query = query.Where(t => t.ProjetoId == tarefaFiltro.ProjetoId.Value);

        if (tarefaFiltro.UsuarioId.HasValue)
            query = query.Where(t => t.UsuarioId == tarefaFiltro.UsuarioId.Value);

        if (tarefaFiltro.Status.HasValue)
            query = query.Where(t => t.Status == tarefaFiltro.Status.Value);

        var totalItems = query.Count();

        var itens = query
            .OrderByDescending(t => t.Id)
            .Skip((tarefaFiltro.PageNumber - 1) * tarefaFiltro.PageSize)
            .Take(tarefaFiltro.PageSize)
            .ToList();

        return Task.FromResult((itens, totalItems));
    }

    public Task<bool> ValidarTituloExistenteAsync(int projetoId, string titulo)
    {
        var (tarefas, _, _) = ObterDadosSessao();

        titulo = titulo.Trim();

        var existe = tarefas.Any(t =>
            t.ProjetoId == projetoId &&
            t.Titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(existe);
    }
}