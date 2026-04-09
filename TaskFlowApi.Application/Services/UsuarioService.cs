using TaskFlowApi.Application.Dto.Pagination;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Auth;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Application.Interfaces.Services;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UsuarioResponse> CriarUsuarioAsync(UsuarioRequest request)
        {
            ValidarDadosCriacaoUsuario(request);

            if (await _usuarioRepository.EmailExistente(request.Email))
            {
                throw new DomainException("Já existe um usuário com este e-mail.", ErrorTypeEnum.Conflict);
            }

            var perfil = Enum.Parse<PerfilAcessoEnum>(request.Perfil, true);

            _passwordHasher.CriarSenhaHash(request.Senha, out byte[] senhaHash, out byte[] senhaSalt);

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

        public async Task<UsuarioResponse> AtualizarAsync(int idUsuario, int usuarioLogadoId, UsuarioUpdateRequest request) 
        {
            if (idUsuario <= 0) throw new DomainException("Id Inválido!", ErrorTypeEnum.Validation);

            var usuarioLogado = await _usuarioRepository.ObterPorIdAsync(usuarioLogadoId);
            if (usuarioLogado is null)
            {
                throw new DomainException("Usuário autenticado não encontrado!", ErrorTypeEnum.Unauthorized);
            }

            var usuario = await _usuarioRepository.ObterPorIdAsync(idUsuario);
            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.NotFound);
            }

            if (NaoPodeAlterarUsuario(usuario.Perfil, usuarioLogado.Perfil, request.Perfil)) 
            {
                throw new DomainException("Você não tem permissão para alterar este usuário!", ErrorTypeEnum.Forbidden);
            }

            if(request.Nome is not null && string.IsNullOrWhiteSpace(request.Nome)) 
            {
                throw new DomainException("Nome não pode ser vazio!", ErrorTypeEnum.Validation);
            }

            if (request.Email is not null)
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new DomainException("E-mail não pode ser vazio!", ErrorTypeEnum.Validation);

                if (!Usuario.EmailValido(request.Email))
                    throw new DomainException("E-mail inválido!", ErrorTypeEnum.Validation);

                if (!request.Email.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _usuarioRepository.EmailExistente(request.Email))
                    {
                        throw new DomainException("Já existe um e-mail cadastrado!", ErrorTypeEnum.Conflict);
                    }
                }
            }

            usuario.Atualizar(request.Nome, request.Email, request.Perfil);

            await _usuarioRepository.AtualizarAsync(usuario);

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil
            };
        }

        public async Task<UsuarioResponse> ObterPorIdAsync(int idUsuario) 
        {
            if (idUsuario <= 0) throw new DomainException("Id Inválido!", ErrorTypeEnum.Validation);

            var usuario = await _usuarioRepository.ObterPorIdAsync(idUsuario);

            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.NotFound);
            }

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                CriadoEm = usuario.CriadoEm
            };
        }

        public async Task ExcluirAsync(int idUsuario) 
        {
            if (idUsuario <= 0) throw new DomainException("Id Inválido!", ErrorTypeEnum.Validation);

            var usuario = await _usuarioRepository.ObterPorIdAsync(idUsuario);

            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.NotFound);
            }

            await _usuarioRepository.RemoverAsync(usuario);
        }

        public async Task<PaginacaoResponse<UsuarioResponse>> ListarAsync(UsuarioFiltroRequest request)
        {
            if (request.PageNumber <= 0)
                request.PageNumber = 1;

            if (request.PageSize <= 0)
                request.PageSize = 10;

            var filtro = new UsuarioFiltro
            {
                Nome = request.Nome,
                Email = request.Email,
                Perfil = request.Perfil,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var (itens, totalItens) = await _usuarioRepository.ListarAsync(filtro);

            var usuarios = itens.Select(u => new UsuarioResponse
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Perfil = u.Perfil,
                CriadoEm = u.CriadoEm
            }).ToList();

            return new PaginacaoResponse<UsuarioResponse> 
            {
                Items = usuarios,
                TotalItems = totalItens,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItens / request.PageSize)
            };
        }

        private bool NaoPodeAlterarUsuario(PerfilAcessoEnum perfilUsuarioBanco, PerfilAcessoEnum perfilLogado, PerfilAcessoEnum? perfilDesejado) 
        {
            if (perfilLogado == PerfilAcessoEnum.Consulta)
                return true;

            if (perfilLogado != PerfilAcessoEnum.Administrador && 
                (perfilUsuarioBanco == PerfilAcessoEnum.Administrador
                || perfilDesejado == PerfilAcessoEnum.Administrador)) 
            {
                return true;
            }

            return false;
        }

        private void ValidarDadosCriacaoUsuario(UsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new DomainException("Nome é obrigatório!", ErrorTypeEnum.Validation);

            if (string.IsNullOrWhiteSpace(request.Email) || !Usuario.EmailValido(request.Email))
                throw new DomainException("E-mail é inválido!", ErrorTypeEnum.Validation);

            if (string.IsNullOrWhiteSpace(request.Perfil))
                throw new DomainException("Tipo do perfil é obrigatório!", ErrorTypeEnum.Validation);

            if (!Enum.TryParse<PerfilAcessoEnum>(request.Perfil, true, out _))
                throw new DomainException("Perfil inválido.", ErrorTypeEnum.Validation);

            if (string.IsNullOrWhiteSpace(request.Senha))
                throw new DomainException("Senha é obrigatória!", ErrorTypeEnum.Validation);

            if (!Usuario.SenhaValida(request.Senha))
                throw new DomainException("Senha deve ter no mínimo 8 caracteres, contendo letras e números!", ErrorTypeEnum.Validation);

            if (string.IsNullOrWhiteSpace(request.ConfirmaSenha))
                throw new DomainException("Confirmação de senha é obrigatória!", ErrorTypeEnum.Validation);

            if (request.Senha != request.ConfirmaSenha)
                throw new DomainException("Senhas não coincidem!", ErrorTypeEnum.Validation);           
        }
    }
}
