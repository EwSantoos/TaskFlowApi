using System.Reflection;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Auth;
using TaskFlowApi.Application.Interfaces.Demo;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Infra.Repository.Demo;

public class UsuarioRepositoryDemo : IUsuarioRepository
{
    private readonly IPasswordHasher _hasher;
    private readonly IDemoSessionService _demoSessionService;
    private readonly ICurrentSessionService _currentSessionService;

    private static readonly List<Usuario> _usuariosBase = new();
    private static bool _inicializado = false;
    private static readonly object _lock = new();

    public UsuarioRepositoryDemo(IPasswordHasher hasher, IDemoSessionService demoSessionService, ICurrentSessionService currentSessionService)
    {
        _hasher = hasher;
        _demoSessionService = demoSessionService;
        _currentSessionService = currentSessionService;

        if (_inicializado)
            return;

        lock (_lock)
        {
            if (_inicializado)
                return;

            CriarUsuarioBase(1, "AdminDemo", "admin@teste.com", PerfilAcessoEnum.Administrador);
            CriarUsuarioBase(2, "OperacionalDemo", "operacional@teste.com", PerfilAcessoEnum.Operacional);
            CriarUsuarioBase(3, "ConsultaDemo", "consulta@teste.com", PerfilAcessoEnum.Consulta);
            CriarUsuarioBase(4, "UsuarioDemo", "usuariodemo@teste.com", PerfilAcessoEnum.Operacional);
            CriarUsuarioBase(5, "TesteDemo", "testedemo@teste.com", PerfilAcessoEnum.Operacional);

            _inicializado = true;
        }
    }

    private List<Usuario> ObterUsuarios()
    {
        var sessionId = _currentSessionService.ObterSessionId();

        if (string.IsNullOrWhiteSpace(sessionId))
            return _usuariosBase;

        var session = _demoSessionService.ObterSessao(sessionId);
        return session.Usuarios;
    }

    private void CriarUsuarioBase(int id, string nome, string email, PerfilAcessoEnum perfil)
    {
        _hasher.CriarSenhaHash("Teste123", out byte[] senhaHash, out byte[] senhaSalt);

        var usuario = Usuario.Criar(nome, email, perfil, senhaHash, senhaSalt);
        DefinirId(usuario, id);

        _usuariosBase.Add(usuario);
    }

    private static void DefinirId(Usuario usuario, int id)
    {
        var prop = typeof(Usuario).GetProperty(nameof(Usuario.Id), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (prop is null)
            throw new DomainException("Propriedade Id não encontrada em Usuario.", ErrorTypeEnum.InternalError);

        prop.SetValue(usuario, id);
    }

    public Task<Usuario> AdicionarAsync(Usuario usuario)
    {
        var usuarios = ObterUsuarios();

        var novoId = usuarios.Any() ? usuarios.Max(u => u.Id) + 1 : 1;
        DefinirId(usuario, novoId);

        usuarios.Add(usuario);

        return Task.FromResult(usuario);
    }

    public Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        var usuarios = ObterUsuarios();

        var existente = usuarios.FirstOrDefault(u => u.Id == usuario.Id);

        if (existente is null)
            return Task.FromResult(usuario);

        existente.Atualizar(usuario.Nome, usuario.Email, usuario.Perfil);

        return Task.FromResult(existente);
    }

    public Task<Usuario?> ObterPorIdAsync(int idUsuario)
    {
        var usuarios = ObterUsuarios();
        var usuario = usuarios.FirstOrDefault(u => u.Id == idUsuario);

        return Task.FromResult(usuario);
    }

    public Task RemoverAsync(Usuario usuario)
    {
        var usuarios = ObterUsuarios();

        var existente = usuarios.FirstOrDefault(u => u.Id == usuario.Id);

        if (existente is not null)
            usuarios.Remove(existente);

        return Task.CompletedTask;
    }

    public Task<(List<Usuario> Itens, int TotalItens)> ListarAsync(UsuarioFiltro usuarioFiltro)
    {
        var usuarios = ObterUsuarios();
        var query = usuarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(usuarioFiltro.Nome))
        {
            var nome = usuarioFiltro.Nome.Trim();
            query = query.Where(u => u.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(usuarioFiltro.Email))
        {
            var email = usuarioFiltro.Email.Trim().ToLowerInvariant();
            query = query.Where(u => u.Email.Contains(email, StringComparison.OrdinalIgnoreCase));
        }

        if (usuarioFiltro.Perfil.HasValue)
            query = query.Where(u => u.Perfil == usuarioFiltro.Perfil.Value);

        var totalItens = query.Count();

        var itens = query
            .OrderByDescending(u => u.Id)
            .Skip((usuarioFiltro.PageNumber - 1) * usuarioFiltro.PageSize)
            .Take(usuarioFiltro.PageSize)
            .ToList();

        return Task.FromResult((itens, totalItens));
    }

    public Task<Usuario?> ObterPorEmailAsync(string email)
    {
        var usuarios = ObterUsuarios();

        email = email.Trim().ToLowerInvariant();

        var usuario = usuarios.FirstOrDefault(u => u.Email == email);

        return Task.FromResult(usuario);
    }

    public Task<bool> EmailExistente(string email)
    {
        var usuarios = ObterUsuarios();

        email = email.Trim().ToLowerInvariant();

        var existe = usuarios.Any(u => u.Email == email);

        return Task.FromResult(existe);
    }
}