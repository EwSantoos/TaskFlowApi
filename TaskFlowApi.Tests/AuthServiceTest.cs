using Moq;
using Moq.AutoMock;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.Auth;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Application.Services;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Tests
{
    public class AuthServiceTest
    {
        [Fact]
        public async Task TokenAsync_DeveLancarExcecao_QuandoEmailForVazio()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<AuthService>();

            var request = new UsuarioLogin
            {
                Email = "",
                Senha = "Senha123"
            };

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.TokenAsync(request));

            Assert.Equal("E-mail é obrigatório!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Validation, ex.Type);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.ObterPorEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TokenAsync_DeveLancarExcecao_QuandoEmailForInvalido()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<AuthService>();

            var request = new UsuarioLogin
            {
                Email = "email-invalido",
                Senha = "Senha123"
            };

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.TokenAsync(request));

            Assert.Equal("E-mail inválido!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Validation, ex.Type);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.ObterPorEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TokenAsync_DeveLancarExcecao_QuandoSenhaForVazia()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<AuthService>();

            var request = new UsuarioLogin
            {
                Email = "usuario@teste.com",
                Senha = ""
            };

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.TokenAsync(request));

            Assert.Equal("Senha é obrigatória!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Validation, ex.Type);

            mocker.GetMock<IUsuarioRepository>().Verify(x => x.ObterPorEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TokenAsync_DeveLancarExcecao_QuandoUsuarioNaoForEncontrado()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<AuthService>();

            var request = new UsuarioLogin
            {
                Email = "usuario@teste.com",
                Senha = "Senha123"
            };

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.TokenAsync(request));

            Assert.Equal("E-mail ou senha inválidos!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Validation, ex.Type);

            mocker.GetMock<IPasswordHasher>().Verify(x => x.ValidarSenhaHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Never);

            mocker.GetMock<ITokenService>().Verify(x => x.CriarToken(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task TokenAsync_DeveLancarExcecao_QuandoSenhaForInvalida()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<AuthService>();

            var request = new UsuarioLogin
            {
                Email = "usuario@teste.com",
                Senha = "SenhaErrada123"
            };

            var usuario = CriarUsuario("Ewerton", "usuario@teste.com", PerfilAcessoEnum.Administrador);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);

            mocker.GetMock<IPasswordHasher>().Setup(x => x.ValidarSenhaHash(request.Senha, usuario.PasswordHash, usuario.PasswordSalt))
                .Returns(false);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.TokenAsync(request));

            Assert.Equal("E-mail ou senha inválidos!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Validation, ex.Type);

            mocker.GetMock<ITokenService>().Verify(x => x.CriarToken(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task TokenAsync_DeveRetornarToken_QuandoLoginForValido()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<AuthService>();

            var request = new UsuarioLogin
            {
                Email = "usuario@teste.com",
                Senha = "Senha123"
            };

            var usuario = CriarUsuario("Ewerton", "usuario@teste.com", PerfilAcessoEnum.Administrador);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);

            mocker.GetMock<IPasswordHasher>().Setup(x => x.ValidarSenhaHash(request.Senha, usuario.PasswordHash, usuario.PasswordSalt))
                .Returns(true);

            mocker.GetMock<ITokenService>().Setup(x => x.CriarToken(usuario))
                .Returns("token-fake");

            var response = await sut.TokenAsync(request);

            Assert.NotNull(response);
            Assert.Equal("token-fake", response.Token);

            mocker.GetMock<ITokenService>().Verify(x => x.CriarToken(usuario), Times.Once);
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
