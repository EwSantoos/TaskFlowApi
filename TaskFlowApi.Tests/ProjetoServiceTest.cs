using System.Reflection;
using Moq;
using Moq.AutoMock;
using TaskFlowApi.Application.Dto.Project;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Application.Services;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Tests
{
    public class ProjetoServiceTest
    {
        [Fact]
        public async Task CriarProjetoAsync_DeveCriarProjetoComSucesso()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            const int usuarioLogadoId = 1;

            var usuarioLogado = CriarUsuario("Ewerton", "ewerton@teste.com", PerfilAcessoEnum.Administrador);

            var request = new ProjetoRequest
            {
                Nome = "Projeto API",
                Descricao = "Descrição teste"
            };

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioLogadoId))
                .ReturnsAsync(usuarioLogado);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ValidarNomeExistenteAsync(request.Nome))
                .ReturnsAsync(false);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.AdicionarAsync(It.IsAny<Projeto>()))
                .ReturnsAsync((Projeto p) => p);

            var response = await sut.CriarProjetoAsync(usuarioLogadoId, request);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.ValidarNomeExistenteAsync(request.Nome), Times.Once);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Projeto>()), Times.Once);

            Assert.Equal("Projeto API", response.Nome);
            Assert.Equal("Descrição teste", response.Descricao);
            Assert.Equal(usuarioLogado.Id, response.UsuarioCriadorId);
            Assert.Equal("Ewerton", response.NomeCriador);
        }

        [Fact]
        public async Task CriarProjetoAsync_DeveLancarExcecao_QuandoUsuarioLogadoNaoForEncontrado()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            var request = new ProjetoRequest
            {
                Nome = "Projeto API",
                Descricao = "Descrição teste"
            };

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.CriarProjetoAsync(1, request));

            Assert.Equal("Usuário autenticado não encontrado!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Unauthorized, ex.Type);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Projeto>()), Times.Never);
        }

        [Fact]
        public async Task CriarProjetoAsync_DeveLancarExcecao_QuandoNomeJaExistir()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            var usuarioLogado = CriarUsuario("Ewerton", "ewerton@teste.com", PerfilAcessoEnum.Administrador);

            var request = new ProjetoRequest
            {
                Nome = "Projeto API",
                Descricao = "Descrição teste"
            };

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(usuarioLogado);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ValidarNomeExistenteAsync(request.Nome))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.CriarProjetoAsync(1, request));

            Assert.Equal("Já existe um projeto com esse nome!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Conflict, ex.Type);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Projeto>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarProjetoAsync_DeveLancarExcecao_QuandoProjetoNaoForEncontrado()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync((Projeto?)null);

            var request = new ProjetoUpdateRequest
            {
                Nome = "Novo Nome"
            };

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.AtualizarProjetoAsync(1, request));

            Assert.Equal("Projeto não encontrado!", ex.Message);
            Assert.Equal(ErrorTypeEnum.NotFound, ex.Type);
        }

        [Fact]
        public async Task AtualizarProjetoAsync_DeveLancarExcecao_QuandoNovoNomeJaExistir()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            var projeto = CriarProjeto("Projeto Antigo", "Descrição antiga");
            var criador = CriarUsuario("Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);

            var request = new ProjetoUpdateRequest
            {
                Nome = "Projeto Novo",
                Descricao = "Descrição alterada"
            };

            DefinirPropriedadePrivada(projeto, "UsuarioCriador", criador);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(projeto);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ValidarNomeExistenteAsync(request.Nome))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.AtualizarProjetoAsync(1, request));

            Assert.Equal("Já existe um projeto com esse nome!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Conflict, ex.Type);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Projeto>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarProjetoAsync_NaoDeveValidarDuplicidadeQuandoNomeForOMesmo()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            var projeto = CriarProjeto("Projeto API", "Descrição antiga");
            var criador = CriarUsuario("Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);

            DefinirPropriedadePrivada(projeto, "UsuarioCriador", criador);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(projeto);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.AtualizarAsync(It.IsAny<Projeto>()))
                .ReturnsAsync((Projeto p) => p);

            var request = new ProjetoUpdateRequest
            {
                Nome = "Projeto API",
                Descricao = "Descrição nova"
            };

            var response = await sut.AtualizarProjetoAsync(1, request);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.ValidarNomeExistenteAsync(It.IsAny<string>()), Times.Never);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Projeto>()), Times.Once);

            Assert.Equal("Projeto API", response.Nome);
            Assert.Equal("Descrição nova", response.Descricao);
            Assert.Equal("Admin", response.NomeCriador);
        }

        [Fact]
        public async Task AtualizarProjetoAsync_DeveAtualizarProjetoComSucesso()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            var projeto = CriarProjeto("Projeto Antigo", "Descrição antiga");
            var criador = CriarUsuario("Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);

            DefinirPropriedadePrivada(projeto, "UsuarioCriador", criador);

            var request = new ProjetoUpdateRequest
            {
                Nome = "Projeto Novo",
                Descricao = "Descrição nova"
            };

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(projeto);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ValidarNomeExistenteAsync(request.Nome))
                .ReturnsAsync(false);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.AtualizarAsync(It.IsAny<Projeto>()))
                .ReturnsAsync((Projeto p) => p);

            var response = await sut.AtualizarProjetoAsync(1, request);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.ValidarNomeExistenteAsync(It.IsAny<string>()), Times.Once);

            mocker.GetMock<IProjetoRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Projeto>()), Times.Once);

            Assert.Equal("Projeto Novo", response.Nome);
            Assert.Equal("Descrição nova", response.Descricao);
            Assert.Equal("Admin", response.NomeCriador);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarProjetoComTarefas()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            var projeto = CriarProjeto("Projeto API", "Descrição");
            var criador = CriarUsuario("Admin", "admin@teste.com", PerfilAcessoEnum.Administrador);
            var responsavel = CriarUsuario("João", "joao@teste.com", PerfilAcessoEnum.Operacional);

            var tarefa = Tarefa.Criar(
                projeto.Id,
                responsavel.Id,
                "Implementar endpoint",
                StatusTarefaEnum.Pendente,
                "Criar endpoint de listagem",
                DateTime.UtcNow.AddDays(7));

            DefinirPropriedadePrivada(projeto, "Id", 1);
            DefinirPropriedadePrivada(projeto, "UsuarioCriador", criador);
            DefinirPropriedadePrivada(tarefa, "Usuario", responsavel);
            DefinirPropriedadePrivada(projeto, "Tarefas", new List<Tarefa> { tarefa });

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(projeto);

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ListarAsync(It.Is<TarefaFiltro>(f =>f
            .ProjetoId == 1 &&f.PageNumber == 1 &&f.PageSize == 1000)))
                .ReturnsAsync((new List<Tarefa> { tarefa }, 1));


            var response = await sut.ObterPorIdAsync(1);

            Assert.Equal("Projeto API", response.Nome);
            Assert.Equal("Descrição", response.Descricao);

            Assert.NotNull(response.Tarefas);
            Assert.Single(response.Tarefas);

            var tarefaResponse = response.Tarefas[0];

            Assert.Equal("Implementar endpoint", tarefaResponse.Titulo);
            Assert.Equal("Criar endpoint de listagem", tarefaResponse.Descricao);
            Assert.Equal(StatusTarefaEnum.Pendente, tarefaResponse.Status);
            Assert.Equal("Projeto API", tarefaResponse.NomeProjeto);
            Assert.Equal("João", tarefaResponse.NomeUsuario);
        }

        [Fact]
        public async Task ListarAsync_DeveRetornarProjetosMapeadosCorretamente()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<ProjetoService>();

            var projeto1 = CriarProjeto("Projeto 1", "Descrição 1");
            var projeto2 = CriarProjeto("Projeto 2", "Descrição 2");

            DefinirPropriedadePrivada(projeto1, "UsuarioCriador",
                CriarUsuario("Admin 1", "admin1@teste.com", PerfilAcessoEnum.Administrador));

            DefinirPropriedadePrivada(projeto2, "UsuarioCriador",
                CriarUsuario("Admin 2", "admin2@teste.com", PerfilAcessoEnum.Administrador));

            var projetos = new List<Projeto> { projeto1, projeto2 };

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ListarAsync())
                .ReturnsAsync(projetos);

            var response = await sut.ListarAsync();

            Assert.Equal(2, response.Count);
            Assert.Equal("Projeto 1", response[0].Nome);
            Assert.Equal("Admin 1", response[0].NomeCriador);
            Assert.Equal("Projeto 2", response[1].Nome);
            Assert.Equal("Admin 2", response[1].NomeCriador);
        }

        private static Projeto CriarProjeto(string nome, string descricao)
        {
            return Projeto.Criar(1, nome, descricao);
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

        private static void DefinirPropriedadePrivada<T>(object obj, string nomePropriedade, T valor)
        {
            var propriedade = obj.GetType().GetProperty(
                nomePropriedade,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (propriedade is null)
                throw new InvalidOperationException($"Propriedade '{nomePropriedade}' não encontrada.");

            propriedade.SetValue(obj, valor);
        }

        private static AutoMocker CriarMocker()
        {
            return new AutoMocker();
        }
    }
}
