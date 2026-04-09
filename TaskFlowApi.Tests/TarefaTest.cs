using System.Reflection;
using Moq;
using Moq.AutoMock;
using TaskFlowApi.Application.Dto.TaskItem;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Application.Services;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Tests
{
    public class TarefaTest
    {
        [Fact]
        public async Task CriarTarefaAsync_DeveCriarTarefaComSucesso()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            const int projetoId = 1;
            const int usuarioId = 2;

            var projeto = CriarProjeto("Projeto API", "Descrição");
            var usuario = CriarUsuario("João", "joao@teste.com", PerfilAcessoEnum.Operacional);

            var request = new TarefaRequest
            {
                Titulo = "Implementar endpoint",
                Descricao = "Criar endpoint de listagem",
                Status = StatusTarefaEnum.Pendente,
                DataLimite = DateTime.UtcNow.AddDays(7),
                UsuarioId = usuarioId
            };

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(projetoId))
                .ReturnsAsync(projeto);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(usuarioId))
                .ReturnsAsync(usuario);

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ValidarTituloExistenteAsync(projetoId, request.Titulo))
                .ReturnsAsync(false);

            mocker.GetMock<ITarefaRepository>().Setup(x => x.AdicionarAsync(It.IsAny<Tarefa>()))
                .ReturnsAsync((Tarefa t) => t);

            var response = await sut.CriarTarefaAsync(projetoId, request);

            mocker.GetMock<ITarefaRepository>().Verify(x => x.ValidarTituloExistenteAsync(projetoId, request.Titulo), Times.Once);

            mocker.GetMock<ITarefaRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Tarefa>()), Times.Once);

            Assert.Equal("Implementar endpoint", response.Titulo);
            Assert.Equal("Criar endpoint de listagem", response.Descricao);
            Assert.Equal(StatusTarefaEnum.Pendente, response.Status);
            Assert.Equal(projeto.Id, response.ProjetoId);
            Assert.Equal(projeto.Nome, response.NomeProjeto);
            Assert.Equal(usuario.Id, response.UsuarioId);
            Assert.Equal(usuario.Nome, response.NomeUsuario);
        }

        [Fact]
        public async Task CriarTarefaAsync_DeveLancarExcecao_QuandoProjetoNaoForEncontrado()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            var request = new TarefaRequest
            {
                Titulo = "Implementar endpoint",
                Descricao = "Criar endpoint de listagem",
                Status = StatusTarefaEnum.Pendente,
                DataLimite = DateTime.UtcNow.AddDays(7),
                UsuarioId = 2
            };

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync((Projeto?)null);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.CriarTarefaAsync(1, request));

            Assert.Equal("Projeto não encontrado!", ex.Message);
            Assert.Equal(ErrorTypeEnum.NotFound, ex.Type);

            mocker.GetMock<ITarefaRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Tarefa>()), Times.Never);
        }

        [Fact]
        public async Task CriarTarefaAsync_DeveLancarExcecao_QuandoUsuarioNaoForEncontrado()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            var projeto = CriarProjeto("Projeto API", "Descrição");

            var request = new TarefaRequest
            {
                Titulo = "Implementar endpoint",
                Descricao = "Criar endpoint de listagem",
                Status = StatusTarefaEnum.Pendente,
                DataLimite = DateTime.UtcNow.AddDays(7),
                UsuarioId = 2
            };

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(projeto);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(2))
                .ReturnsAsync((Usuario?)null);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.CriarTarefaAsync(1, request));

            Assert.Equal("Usuário não encontrado!", ex.Message);
            Assert.Equal(ErrorTypeEnum.NotFound, ex.Type);

            mocker.GetMock<ITarefaRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Tarefa>()), Times.Never);
        }

        [Fact]
        public async Task CriarTarefaAsync_DeveLancarExcecao_QuandoTituloJaExistirNoProjeto()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            var projeto = CriarProjeto("Projeto API", "Descrição");
            var usuario = CriarUsuario("João", "joao@teste.com", PerfilAcessoEnum.Operacional);

            var request = new TarefaRequest
            {
                Titulo = "Implementar endpoint",
                Descricao = "Criar endpoint de listagem",
                Status = StatusTarefaEnum.Pendente,
                DataLimite = DateTime.UtcNow.AddDays(7),
                UsuarioId = 2
            };

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(projeto);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(2))
                .ReturnsAsync(usuario);

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ValidarTituloExistenteAsync(1, request.Titulo))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.CriarTarefaAsync(1, request));

            Assert.Equal("Já existe uma tarefa com esse nome no mesmo projeto!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Conflict, ex.Type);

            mocker.GetMock<ITarefaRepository>().Verify(x => x.AdicionarAsync(It.IsAny<Tarefa>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarTarefaAsync_DeveLancarExcecao_QuandoTarefaNaoForEncontrada()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync((Tarefa?)null);

            var request = new TarefaUpdateRequest
            {
                Titulo = "Novo título"
            };

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.AtualizarTarefaAsync(1, request));

            Assert.Equal("Tarefa não encontrada!", ex.Message);
            Assert.Equal(ErrorTypeEnum.NotFound, ex.Type);
        }

        [Fact]
        public async Task AtualizarTarefaAsync_DeveLancarExcecao_QuandoTituloJaExistirNoProjetoFinal()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            var projeto = CriarProjeto("Projeto API", "Descrição");
            var usuario = CriarUsuario("João", "joao@teste.com", PerfilAcessoEnum.Operacional);
            var tarefa = CriarTarefa(projeto, usuario, "Tarefa Antiga");

            var request = new TarefaUpdateRequest
            {
                Titulo = "Novo Título"
            };

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(tarefa);

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ValidarTituloExistenteAsync(tarefa.ProjetoId, request.Titulo))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.AtualizarTarefaAsync(1, request));

            Assert.Equal("Já existe uma tarefa com esse nome no mesmo projeto!", ex.Message);
            Assert.Equal(ErrorTypeEnum.Conflict, ex.Type);

            mocker.GetMock<ITarefaRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Tarefa>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarTarefaAsync_DeveAtualizarTarefaComSucesso()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            var projetoAtual = CriarProjeto("Projeto API", "Descrição");
            var usuarioAtual = CriarUsuario("João", "joao@teste.com", PerfilAcessoEnum.Operacional);
            var tarefa = CriarTarefa(projetoAtual, usuarioAtual, "Tarefa Antiga");

            var novoProjeto = CriarProjeto("Projeto Novo", "Nova descrição");
            var novoUsuario = CriarUsuario("Maria", "maria@teste.com", PerfilAcessoEnum.Operacional);

            var request = new TarefaUpdateRequest
            {
                ProjetoId = 3,
                UsuarioId = 4,
                Titulo = "Novo Título",
                Descricao = "Descrição nova",
                Status = StatusTarefaEnum.EmAndamento,
                DataLimite = DateTime.UtcNow.AddDays(10)
            };

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(tarefa);

            mocker.GetMock<IProjetoRepository>().Setup(x => x.ObterPorIdAsync(3))
                .ReturnsAsync(novoProjeto);

            mocker.GetMock<IUsuarioRepository>().Setup(x => x.ObterPorIdAsync(4))
                .ReturnsAsync(novoUsuario);

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ValidarTituloExistenteAsync(3, request.Titulo))
                .ReturnsAsync(false);

            mocker.GetMock<ITarefaRepository>().Setup(x => x.AtualizarAsync(It.IsAny<Tarefa>()))
                .ReturnsAsync((Tarefa t) => t);
            

            var response = await sut.AtualizarTarefaAsync(1, request);

            mocker.GetMock<ITarefaRepository>().Verify(x => x.ValidarTituloExistenteAsync(3, "Novo Título"), Times.Once);

            mocker.GetMock<ITarefaRepository>().Verify(x => x.AtualizarAsync(It.IsAny<Tarefa>()), Times.Once);

            Assert.Equal("Novo Título", response.Titulo);
            Assert.Equal("Descrição nova", response.Descricao);
            Assert.Equal(StatusTarefaEnum.EmAndamento, response.Status);
            Assert.Equal(novoProjeto.Id, response.ProjetoId);
            Assert.Equal(novoProjeto.Nome, response.NomeProjeto);
            Assert.Equal(novoUsuario.Id, response.UsuarioId);
            Assert.Equal(novoUsuario.Nome, response.NomeUsuario);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarTarefaComProjetoEUsuario()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            var projeto = CriarProjeto("Projeto API", "Descrição");
            var usuario = CriarUsuario("João", "joao@teste.com", PerfilAcessoEnum.Operacional);
            var tarefa = CriarTarefa(projeto, usuario, "Implementar endpoint");

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ObterPorIdAsync(1))
                .ReturnsAsync(tarefa);

            var response = await sut.ObterPorIdAsync(1);

            Assert.Equal("Implementar endpoint", response.Titulo);
            Assert.Equal(projeto.Nome, response.NomeProjeto);
            Assert.Equal(usuario.Nome, response.NomeUsuario);
        }

        [Fact]
        public async Task ListarAsync_DeveAplicarPaginacaoPadrao_QuandoPageNumberEPageSizeForemInvalidos()
        {
            var mocker = CriarMocker();
            var sut = mocker.CreateInstance<TarefaService>();

            var projeto = CriarProjeto("Projeto API", "Descrição");
            var usuario = CriarUsuario("João", "joao@teste.com", PerfilAcessoEnum.Operacional);

            var tarefas = new List<Tarefa>
            {
                CriarTarefa(projeto, usuario, "Tarefa 1"),
                CriarTarefa(projeto, usuario, "Tarefa 2")
            };

            var request = new TarefaFiltroRequest
            {
                PageNumber = 0,
                PageSize = 0
            };

            TarefaFiltro? filtroRecebido = null;

            mocker.GetMock<ITarefaRepository>().Setup(x => x.ListarAsync(It.IsAny<TarefaFiltro>()))
                .Callback<TarefaFiltro>(filtro => filtroRecebido = filtro).ReturnsAsync((tarefas, 2));

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

        private static Tarefa CriarTarefa(Projeto projeto, Usuario usuario, string titulo)
        {
            var tarefa = Tarefa.Criar(
                projeto.Id,
                usuario.Id,
                titulo,
                StatusTarefaEnum.Pendente,
                "Descrição da tarefa",
                DateTime.UtcNow.AddDays(7));

            DefinirPropriedadePrivada(tarefa, "Projeto", projeto);
            DefinirPropriedadePrivada(tarefa, "Usuario", usuario);

            return tarefa;
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
