using Moq;
using Moq.AutoMock;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Auth;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Application.Services;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Tests
{
    public class UsuarioServiceTest
    {
        [Fact]
        public async Task CriarUsuarioAsync_DeveCriarUsuarioComSucesso()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var request = new UsuarioRequest
            {
                Nome = "Ewerton Santos",
                Email = "ewerton@teste.com",
                Perfil = "Administrador",
                Senha = "Senha123",
                ConfirmaSenha = "Senha123"
            };

            var usuarioLogado = Usuario.Criar(
                "Admin Logado",
                "admin@teste.com",
                PerfilAcessoEnum.Administrador,
                new byte[] { 1 },
                new byte[] { 2 });

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(usuarioLogado);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.EmailExistente(request.Email)).ReturnsAsync(false);

            byte[] hashGerado = { 1, 2, 3 };
            byte[] saltGerado = { 4, 5, 6 };

            mocker.GetMock<IPasswordHasher>()
                .Setup(x => x.CriarSenhaHash(request.Senha, out hashGerado, out saltGerado));

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.AdicionarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => u);

            var response = await sut.CriarUsuarioAsync(1, request);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.ObterPorIdAsync(1), Times.Once);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.EmailExistente(request.Email), Times.Once);

            mocker.GetMock<IPasswordHasher>().Verify(x => x.CriarSenhaHash(request.Senha, out hashGerado, out saltGerado), Times.Once);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);

            Assert.Equal(request.Nome, response.Nome);
            Assert.Equal(request.Email, response.Email);
            Assert.Equal(PerfilAcessoEnum.Administrador, response.Perfil);
        }

        [Fact]
        public async Task CriarUsuarioAsync_DeveLancarExcecao_QuandoEmailJaExistir()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var request = new UsuarioRequest
            {
                Nome = "Ewerton Santos",
                Email = "ewerton@teste.com",
                Perfil = "Operacional",
                Senha = "Senha123",
                ConfirmaSenha = "Senha123"
            };

            var usuarioLogado = Usuario.Criar(
                "Admin Logado",
                "admin@teste.com",
                PerfilAcessoEnum.Administrador,
                new byte[] { 1 },
                new byte[] { 2 });

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(usuarioLogado);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.EmailExistente(request.Email)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.CriarUsuarioAsync(1, request));

            Assert.Equal("Já existe um usuário com este e-mail.", ex.Message);
            Assert.Equal(ErrorTypeEnum.Conflict, ex.Type);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.ObterPorIdAsync(1), Times.Once);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.EmailExistente(request.Email), Times.Once);

            mocker.GetMock<IPasswordHasher>().Verify(x => x.CriarSenhaHash(It.IsAny<string>(), out It.Ref<byte[]>.IsAny, out It.Ref<byte[]>.IsAny), Times.Never);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task CriarUsuarioAsync_DeveLancarExcecao_QuandoOperacionalTentarCriarAdmin()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var request = new UsuarioRequest
            {
                Nome = "Novo Admin",
                Email = "novoadmin@teste.com",
                Perfil = "Administrador",
                Senha = "Senha123",
                ConfirmaSenha = "Senha123"
            };

            var usuarioLogado = Usuario.Criar(
                "Operacional Logado",
                "operacional@teste.com",
                PerfilAcessoEnum.Operacional,
                new byte[] { 1 },
                new byte[] { 2 });

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(usuarioLogado);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.CriarUsuarioAsync(1, request));

            Assert.Equal("Você não ter permissão para criar usuário admin!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Forbidden, ex.Type);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoIdUsuarioForInvalido()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var ex = await Assert.ThrowsAsync<DomainException>(() =>sut.AtualizarAsync(0, 1, new UsuarioUpdateRequest()));

            Assert.Equal("Id Inválido!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Validation, ex.Type);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoUsuarioLogadoNaoForEncontrado()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(99))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<DomainException>(() =>sut.AtualizarAsync(1, 99, new UsuarioUpdateRequest()));

            Assert.Equal("Usuário autenticado não encontrado!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Unauthorized, ex.Type);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoUsuarioNaoForEncontrado()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var usuarioLogado = CriarUsuario("Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(usuarioLogado);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(2))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<DomainException>(() =>sut.AtualizarAsync(2, usuarioLogado.Id, new UsuarioUpdateRequest()));

            Assert.Equal("Usuário não encontrado!", ex.Message);
            Assert.Equal(ErrorTypeEnum.NotFound, ex.Type);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoOperacionalTentarAlterarAdministrador()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            const int usuarioAlvoId = 2;
            const int usuarioLogadoId = 1;

            var usuarioLogado = CriarUsuario("Operacional", "operacional@teste.com", PerfilAcessoEnum.Operacional);

            var usuarioAlvo = CriarUsuario("Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioLogadoId))
                .ReturnsAsync(usuarioLogado);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioAlvoId))
                .ReturnsAsync(usuarioAlvo);

            var request = new UsuarioUpdateRequest
            {
                Nome = "Novo Nome"
            };

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.AtualizarAsync(usuarioAlvoId, usuarioLogadoId, request));

            Assert.Equal("Você não tem permissão para alterar este usuário!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Forbidden, ex.Type);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoEmailJaExistir()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            const int usuarioAlvoId = 2;
            const int usuarioLogadoId = 1;

            var usuarioLogado = CriarUsuario( "Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);

            var usuarioAlvo = CriarUsuario( "Usuário", "usuario@teste.com", PerfilAcessoEnum.Operacional);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioLogadoId))
                .ReturnsAsync(usuarioLogado);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioAlvoId))
                .ReturnsAsync(usuarioAlvo);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.EmailExistente("novo@teste.com"))
                .ReturnsAsync(true);

            var request = new UsuarioUpdateRequest
            {
                Email = "novo@teste.com"
            };

            var ex = await Assert.ThrowsAsync<DomainException>(() =>sut.AtualizarAsync(usuarioAlvoId, usuarioLogadoId, request));

            Assert.Equal("Já existe um e-mail cadastrado!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Conflict, ex.Type);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_NaoDeveValidarDuplicidadeQuandoEmailForOMesmo()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            const int usuarioAlvoId = 2;
            const int usuarioLogadoId = 1;

            var usuarioLogado = CriarUsuario( "Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);

            var usuarioAlvo = CriarUsuario( "Usuário", "usuario@teste.com", PerfilAcessoEnum.Operacional);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioLogadoId))
                .ReturnsAsync(usuarioLogado);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioAlvoId))
                .ReturnsAsync(usuarioAlvo);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.AtualizarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => u);

            var request = new UsuarioUpdateRequest
            {
                Nome = "Nome Alterado",
                Email = "usuario@teste.com"
            };

            var response = await sut.AtualizarAsync(usuarioAlvoId, usuarioLogadoId, request);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.EmailExistente(It.IsAny<string>()), Times.Never);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Usuario>()), Times.Once);

            Assert.Equal("Nome Alterado", response.Nome);
            Assert.Equal("usuario@teste.com", response.Email);
            Assert.Equal(PerfilAcessoEnum.Operacional, response.Perfil);
        }
        [Fact]
        public async Task AtualizarAsync_DeveAtualizarUsuarioComSucesso()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            const int usuarioAlvoId = 2;
            const int usuarioLogadoId = 1;

            var usuarioLogado = CriarUsuario( "Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);

            var usuarioAlvo = CriarUsuario("Usuário Antigo", "antigo@teste.com", PerfilAcessoEnum.Operacional);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioLogadoId))
                .ReturnsAsync(usuarioLogado);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioAlvoId))
                .ReturnsAsync(usuarioAlvo);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.EmailExistente("novo@teste.com"))
                .ReturnsAsync(false);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.AtualizarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => u);

            var request = new UsuarioUpdateRequest
            {
                Nome = "Usuário Novo",
                Email = "novo@teste.com",
                Perfil = PerfilAcessoEnum.Consulta
            };

            var response = await sut.AtualizarAsync(usuarioAlvoId, usuarioLogadoId, request);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Usuario>()), Times.Once);

            Assert.Equal("Usuário Novo", response.Nome);
            Assert.Equal("novo@teste.com", response.Email);
            Assert.Equal(PerfilAcessoEnum.Consulta, response.Perfil);
        }

        [Fact]
        public async Task ListarAsync_DeveAplicarPaginacaoPadrao_QuandoPageNumberEPageSizeForemInvalidos()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var usuarios = new List<Usuario>
            {
                CriarUsuario("João", "joao@teste.com", PerfilAcessoEnum.Administrador),
                CriarUsuario("Maria", "maria@teste.com", PerfilAcessoEnum.Operacional)
            };

            UsuarioFiltro? filtroRecebido = null;

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ListarAsync(It.IsAny<UsuarioFiltro>()))
                .Callback<UsuarioFiltro>(filtro => filtroRecebido = filtro).ReturnsAsync((usuarios, 2));

            var request = new UsuarioFiltroRequest
            {
                PageNumber = 0,
                PageSize = 0
            };

            var response = await sut.ListarAsync(request);

            Assert.NotNull(filtroRecebido);
            Assert.Equal(1, filtroRecebido!.PageNumber);
            Assert.Equal(10, filtroRecebido.PageSize);

            Assert.Equal(1, response.PageNumber);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(2, response.TotalItems);
            Assert.Equal(1, response.TotalPages);
            Assert.Equal(2, response.Items.Count);
        }

        private static Usuario CriarUsuario(string nome, string email, PerfilAcessoEnum perfil)
        {
            return Usuario.Criar(
                nome,
                email,
                perfil,
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 });
        }

        private static AutoMocker CriarMocker() 
        {
            return new AutoMocker();
        }
    }
}