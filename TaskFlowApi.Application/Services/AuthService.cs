using TaskFlowApi.Application.Dto.Token;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.Auth;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        public AuthService(IPasswordHasher passwordHasher, IUsuarioRepository usuarioRepository, ITokenService tokenService) 
        {
            _passwordHasher = passwordHasher;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> TokenAsync(UsuarioLogin login) 
        {
            if (string.IsNullOrWhiteSpace(login.Email)) 
            {
                throw new DomainException("E-mail é obrigatório!", ErrorTypeEnum.Validation);
            }

            if (!Usuario.EmailValido(login.Email)) 
            {
                throw new DomainException("E-mail inválido!", ErrorTypeEnum.Validation);
            }

            if (string.IsNullOrWhiteSpace(login.Senha)) 
            {
                throw new DomainException("Senha é obrigatória!", ErrorTypeEnum.Validation);
            }

            var usuario = await _usuarioRepository.ObterPorEmailAsync(login.Email);

            if(usuario is null) 
            { 
                throw new DomainException("E-mail ou senha inválidos!", ErrorTypeEnum.Validation); 
            }

            if(!_passwordHasher.ValidarSenhaHash(login.Senha, usuario.PasswordHash, usuario.PasswordSalt)) 
            {
                throw new DomainException("E-mail ou senha inválidos!", ErrorTypeEnum.Validation);
            }

            return new TokenResponse
            {
                Token = _tokenService.CriarToken(usuario)
            };
        }
    }
}
