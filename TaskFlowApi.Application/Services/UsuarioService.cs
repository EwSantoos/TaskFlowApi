using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.User;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioResponse> AtualizarAsync(int id, PerfilAcessoEnum perfilLogado, UsuarioUpdateRequest request) 
        {
            if (id == 0) throw new DomainException("Id Inválido!", ErrorTypeEnum.Validation);

            var usuario = await _usuarioRepository.ObterPorIdAsync(id);

            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.NotFound);
            }

            if(NaoPodeAlterarUsuario(usuario.Perfil, perfilLogado)) 
            {
                throw new DomainException("Você não tem permissão para alterar este usuário!", ErrorTypeEnum.Forbidden);
            }

            if(!string.IsNullOrWhiteSpace(request.Email) && !request.Email.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase)) 
            {
                if (await _usuarioRepository.ValidarEmailExistente(request.Email))
                {
                    throw new DomainException("Já existe um e-mail cadastrado!", ErrorTypeEnum.Conflict);
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

        public async Task<UsuarioResponse> BuscarPorIdAsync(int id) 
        {
            if (id == 0) throw new DomainException("Id Inválido!", ErrorTypeEnum.Validation);

            var usuario = await _usuarioRepository.ObterPorIdAsync(id);

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

        public async Task ExcluirAsync(int id) 
        {
            if (id == 0) throw new DomainException("Id Inválido!", ErrorTypeEnum.Validation);

            var usuario = await _usuarioRepository.ObterPorIdAsync(id);

            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.NotFound);
            }

            await _usuarioRepository.RemoverAsync(usuario);
        }

        public async Task<List<UsuarioResponse>> ListarAsync() 
        {
            var lista = await _usuarioRepository.ListarAsync();

            return lista.Select(u => new UsuarioResponse 
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Perfil = u.Perfil,
                CriadoEm=u.CriadoEm
            }).ToList();
        }

        private bool NaoPodeAlterarUsuario(PerfilAcessoEnum usuarioPerfil, PerfilAcessoEnum perfilLogado) 
        {
            return usuarioPerfil == PerfilAcessoEnum.Administrador && perfilLogado != PerfilAcessoEnum.Administrador;
        }
    }
}
