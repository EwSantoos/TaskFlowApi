using TaskFlowApi.Application.Dto.WorkItem;
using TaskFlowApi.Application.Interfaces.Project;
using TaskFlowApi.Application.Interfaces.User;
using TaskFlowApi.Application.Interfaces.WorkItem;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Application.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _tarefaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProjetoRepository _projetoRepository;
        public TarefaService(ITarefaRepository tarefa, IUsuarioRepository usuarioRepository, IProjetoRepository projetoRepository)
        {
            _tarefaRepository = tarefa;
            _usuarioRepository = usuarioRepository;
            _projetoRepository = projetoRepository;
        }
        public async Task<TarefaResponse> CriarTarefaAsync(int projetoId, TarefaRequest request)
        {
            if (projetoId == 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            if (string.IsNullOrWhiteSpace(request.Titulo))
            {
                throw new DomainException("Título é obrigatório!", ErrorTypeEnum.Validation);
            }

            var projeto = await _projetoRepository.ObterPorIdAsync(projetoId);
            if (projeto == null)
            {
                throw new DomainException("Projeto não encontrado!", ErrorTypeEnum.NotFound);
            }

            var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId);
            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.NotFound);
            }

            if (await _tarefaRepository.ValidarTituloExistenteAsync(request.Titulo))
            {
                throw new DomainException("Já existe uma tarefa com esse nome!", ErrorTypeEnum.Conflict);
            }

            var tarefa = Tarefa.Criar(projetoId, request.UsuarioId, request.Titulo, request.Descricao, request.DataLimite);

            await _tarefaRepository.AdicionarAsync(tarefa);

            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                ProjetoId = tarefa.Projeto.Id,
                NomeProjeto = tarefa.Projeto.Nome,
                UsuarioId = tarefa.Usuario.Id,
                NomeUsuario = tarefa.Usuario.Nome,
                CriadoEm = tarefa.CriadoEm,
                DataLimite = tarefa.DataLimite
            };
        }

        public async Task<TarefaResponse> AtualizarTarefaAsync(int tarefaId, TarefaRequest request)
        {
            if (tarefaId == 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
            if (tarefa == null)
            {
                throw new DomainException("Tarefa não encontrada!", ErrorTypeEnum.NotFound);
            }

            var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId);
            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.Validation);
            }

            if (await _tarefaRepository.ValidarTituloExistenteAsync(request.Titulo))
            {
                throw new DomainException("Já existe uma tarefa com esse nome!", ErrorTypeEnum.Conflict);
            }

            tarefa.Atualizar(request.UsuarioId, request.Titulo, request.Descricao, request.DataLimite);

            await _tarefaRepository.AtualizarAsync(tarefa);

            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                ProjetoId = tarefa.Projeto.Id,
                NomeProjeto = tarefa.Projeto.Nome,
                UsuarioId = tarefa.Usuario.Id,
                NomeUsuario = tarefa.Usuario.Nome,
                CriadoEm = tarefa.CriadoEm,
                DataLimite = tarefa.DataLimite
            };
        }

        public async Task<TarefaResponse> BuscarPorIdAsync(int id)
        {
            if (id == 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            var tarefa = await _tarefaRepository.ObterPorIdAsync(id);

            if (tarefa == null)
            {
                throw new DomainException("Tarefa não encontrada!", ErrorTypeEnum.NotFound);
            }

            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                ProjetoId = tarefa.Projeto.Id,
                NomeProjeto = tarefa.Projeto.Nome,
                UsuarioId = tarefa.Usuario.Id,
                NomeUsuario = tarefa.Usuario.Nome,
                CriadoEm = tarefa.CriadoEm,
                DataLimite = tarefa.DataLimite
            };
        }

        public async Task ExcluirAsync(int id)
        {
            if (id == 0) { throw new DomainException("Id inválido!", ErrorTypeEnum.Validation); }

            var tarefa = await _tarefaRepository.ObterPorIdAsync(id);

            if (tarefa == null)
            {
                throw new DomainException("Tarefa não encontrada!", ErrorTypeEnum.NotFound);
            }

            await _tarefaRepository.RemoverAsync(tarefa);
        }

        public async Task<List<TarefaResponse>> ListarAsync()
        {
            var lista = await _tarefaRepository.ListarAsync();

            return lista.Select(l => new TarefaResponse
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Descricao = l.Descricao,
                Status = l.Status,
                ProjetoId = l.Projeto.Id,
                NomeProjeto = l.Projeto.Nome,
                UsuarioId = l.Usuario.Id,
                NomeUsuario = l.Usuario.Nome,
                CriadoEm = l.CriadoEm,
                DataLimite = l.DataLimite

            }).ToList();
        }
    }
}
