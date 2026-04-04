using Moq;
using Moq.AutoMock;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.User;
using TaskFlowApi.Application.Services;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Tests
{
    public class UsuarioServiceTest
    {
        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoIdForInvalido()
        {
            var mocker = AutoMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var result = await Assert.ThrowsAnyAsync<DomainException>(() =>
                sut.AtualizarAsync(0, PerfilAcessoEnum.Administrador, new UsuarioUpdateRequest()));

            Assert.Equal("Id Inválido!", result.Message);
            Assert.Equal(ErrorTypeEnum.Validation, result.Type);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoUsuarioNaoForEncontrado()
        {
            var mocker = AutoMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Usuario?)null);

            var result = await Assert.ThrowsAnyAsync<DomainException>(() =>
                sut.AtualizarAsync(1, PerfilAcessoEnum.Administrador, new UsuarioUpdateRequest()));

            Assert.Equal("Usuário não encontrado!", result.Message);
            Assert.Equal(ErrorTypeEnum.NotFound, result.Type);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoOperacionalTentarAlterarAdministrador()
        {
            var mocker = AutoMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var usuarioAdm = Usuario.Criar(
                "Admin",
                "admin@teste.com",
                PerfilAcessoEnum.Administrador,
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 });

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(usuarioAdm);

            var request = new UsuarioUpdateRequest
            {
                Nome = "Novo Nome"
            };

            var result = await Assert.ThrowsAnyAsync<DomainException>(() =>
                sut.AtualizarAsync(1, PerfilAcessoEnum.Operacional, request));

            Assert.Equal("Você não tem permissão para alterar este usuário!", result.Message);
            Assert.Equal(ErrorTypeEnum.Forbidden, result.Type);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoEmailJaExistir()
        {
            var mocker = AutoMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var usuario = Usuario.Criar(
                "Usuario Teste",
                "usuario@teste.com",
                PerfilAcessoEnum.Operacional,
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 });

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(usuario);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ValidarEmailExistente(It.IsAny<string>()))
                .ReturnsAsync(true);

            var request = new UsuarioUpdateRequest
            {
                Email = "novoemail@teste.com"
            };

            var result = await Assert.ThrowsAnyAsync<DomainException>(() =>
                sut.AtualizarAsync(1, PerfilAcessoEnum.Administrador, request));

            Assert.Equal("Já existe um e-mail cadastrado!", result.Message);
            Assert.Equal(ErrorTypeEnum.Conflict, result.Type);
        }

        [Fact]
        public async Task AtualizarAsync_NaoDeveValidarEmail_QuandoEmailForOMesmo()
        {
            var mocker = AutoMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var usuario = Usuario.Criar(
                "Usuario Teste",
                "usuario@teste.com",
                PerfilAcessoEnum.Operacional,
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 });

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(usuario);

            var request = new UsuarioUpdateRequest
            {
                Nome = "Nome Alterado",
                Email = "usuario@teste.com"
            };

            var response = await sut.AtualizarAsync(1, PerfilAcessoEnum.Administrador, request);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.ValidarEmailExistente(It.IsAny<string>()), Times.Never);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Usuario>()), Times.Once);

            Assert.Equal("Nome Alterado", response.Nome);
            Assert.Equal("usuario@teste.com", response.Email);
            Assert.Equal(PerfilAcessoEnum.Operacional, response.Perfil);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarUsuarioComSucesso()
        {
            var mocker = AutoMocker();
            var sut = mocker.CreateInstance<UsuarioService>();

            var usuario = Usuario.Criar(
                "Usuario Antigo",
                "antigo@teste.com",
                PerfilAcessoEnum.Operacional,
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 });

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(usuario);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ValidarEmailExistente(It.IsAny<string>()))
                .ReturnsAsync(false);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.AtualizarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => u);

            var request = new UsuarioUpdateRequest
            {
                Nome = "Usuario Novo",
                Email = "novo@teste.com",
                Perfil = PerfilAcessoEnum.Consulta
            };

            var response = await sut.AtualizarAsync(1, PerfilAcessoEnum.Administrador, request);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Usuario>()), Times.Once);

            Assert.Equal(usuario.Id, response.Id);
            Assert.Equal("Usuario Novo", response.Nome);
            Assert.Equal("novo@teste.com", response.Email);
            Assert.Equal(PerfilAcessoEnum.Consulta, response.Perfil);
        }

        private AutoMocker AutoMocker() 
        {
            return new AutoMocker();
        }
    }
}