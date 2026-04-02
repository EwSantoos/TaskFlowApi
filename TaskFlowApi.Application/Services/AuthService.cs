using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.Auth;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthRepository _usuarioRepository;
        public AuthService(IPasswordHasher passwordHasher, IAuthRepository usuarioRepository) 
        {
            _passwordHasher = passwordHasher;
            _usuarioRepository = usuarioRepository;
        }
        public async Task<UsuarioResponse> CriarUsuarioAsync(UsuarioRequest request)
        {
            _passwordHasher.CriarSenhaHash(request.Senha, out byte[] senhaHash, out byte[] senhaSalt);

            if(await _usuarioRepository.ObterPorEmailAsync(request.Email) is not null) 
            {
                throw new DomainException("Já existe um usuário com este e-mail.", ErrorTypeEnum.Conflict);
            }

            if (!Enum.TryParse<PerfilAcessoEnum>(request.Perfil, true, out var perfil))
            {
                throw new DomainException("Perfil inválido.", ErrorTypeEnum.Validation);
            }

            var usuario = Usuario.Criar(request.Nome, request.Email, perfil, senhaHash, senhaSalt);

            await _usuarioRepository.AdicionarAsync(usuario);

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                CriadoEm = usuario.CriadoEm
            };

        }

        public async Task<string> LoginAsync(UsuarioLogin login) 
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(login.Email);

            if(usuario is null) 
            { 
                throw new DomainException("E-mail não encontrado!", ErrorTypeEnum.NotFound); 
            }

            if(!_passwordHasher.ValidarSenhaHash(login.Senha, usuario.PasswordHash, usuario.PasswordSalt)) 
            {
                throw new DomainException("Senha incorreta!", ErrorTypeEnum.NotFound);
            }

            return _passwordHasher.CriarToken(usuario);
        }
    }
}
