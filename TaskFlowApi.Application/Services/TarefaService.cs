using TaskFlowApi.Application.Dto.Pagination;
using TaskFlowApi.Application.Dto.TaskItem;
using TaskFlowApi.Application.Filters;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Application.Interfaces.Services;
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
        public TarefaService(ITarefaRepository tarefaRepository, IUsuarioRepository usuarioRepository, IProjetoRepository projetoRepository)
        {
            _tarefaRepository = tarefaRepository;
            _usuarioRepository = usuarioRepository;
            _projetoRepository = projetoRepository;
        }
        public async Task<TarefaResponse> CriarTarefaAsync(int projetoId, TarefaRequest request)
        {
            if (projetoId <= 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            if (string.IsNullOrWhiteSpace(request.Titulo))
            {
                throw new DomainException("Título é obrigatório!", ErrorTypeEnum.Validation);
            }

            var projeto = await _projetoRepository.ObterPorIdAsync(projetoId);
            if (projeto is null)
            {
                throw new DomainException("Projeto não encontrado!", ErrorTypeEnum.NotFound);
            }

            if(request.UsuarioId <= 0) 
            {
                throw new DomainException("Usuário inválido!", ErrorTypeEnum.Validation);
            }

            var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId);
            if (usuario is null)
            {
                throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.NotFound);
            }

            await ValidarTitulo(projetoId, request.Titulo);

            var tarefa = Tarefa.Criar(projetoId, request.UsuarioId, request.Titulo, request.Status, request.Descricao, request.DataLimite);

            await _tarefaRepository.AdicionarAsync(tarefa);

            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                ProjetoId = projeto.Id,
                NomeProjeto = projeto.Nome,
                UsuarioId = usuario.Id,
                NomeUsuario = usuario.Nome,
                CriadoEm = tarefa.CriadoEm,
                DataLimite = tarefa.DataLimite
            };
        }

        public async Task<TarefaResponse> AtualizarTarefaAsync(int tarefaId, TarefaUpdateRequest request)
        {
            if (tarefaId <= 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
            if (tarefa is null)
            {
                throw new DomainException("Tarefa não encontrada!", ErrorTypeEnum.NotFound);
            }

            Projeto? projetoResponse = tarefa.Projeto;
            Usuario? usuarioResponse = tarefa.Usuario;

            if (request.ProjetoId.HasValue) 
            {
                if (request.ProjetoId.Value <= 0) throw new DomainException("Projeto inválido!", ErrorTypeEnum.Validation);

                var projeto = await _projetoRepository.ObterPorIdAsync(request.ProjetoId.Value);
                if (projeto is null)
                {
                    throw new DomainException("Projeto não encontrado!", ErrorTypeEnum.NotFound);
                }

                projetoResponse = projeto;
            }

            if (request.UsuarioId.HasValue) 
            {
                if (request.UsuarioId.Value <= 0) throw new DomainException("Usuário inválido!", ErrorTypeEnum.Validation);

                var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId.Value);
                if (usuario is null)
                {
                    throw new DomainException("Usuário não encontrado!", ErrorTypeEnum.Validation);
                }

                usuarioResponse = usuario;
            }

            await ValidarAlteracaoDeTituloOuProjeto(tarefa, request);

            tarefa.Atualizar(request.ProjetoId, request.UsuarioId, request.Titulo, request.Descricao, request.Status, request.DataLimite);

            await _tarefaRepository.AtualizarAsync(tarefa);

            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                ProjetoId = projetoResponse.Id,
                NomeProjeto = projetoResponse.Nome,
                UsuarioId = usuarioResponse.Id,
                NomeUsuario = usuarioResponse.Nome,
                CriadoEm = tarefa.CriadoEm,
                DataLimite = tarefa.DataLimite
            };
        }

        public async Task<TarefaResponse> ObterPorIdAsync(int tarefaId)
        {
            if (tarefaId <= 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);

            if (tarefa is null)
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

        public async Task ExcluirAsync(int tarefaId)
        {
            if (tarefaId <= 0) { throw new DomainException("Id inválido!", ErrorTypeEnum.Validation); }

            var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);

            if (tarefa is null)
            {
                throw new DomainException("Tarefa não encontrada!", ErrorTypeEnum.NotFound);
            }

            await _tarefaRepository.RemoverAsync(tarefa);
        }

        public async Task<PaginacaoResponse<TarefaResponse>> ListarAsync(TarefaFiltroRequest request)
        {
            if (request.PageNumber <= 0)
                request.PageNumber = 1;

            if (request.PageSize <= 0)
                request.PageSize = 10;

            var filtro = new TarefaFiltro
            {
                Nome = request.Nome,
                ProjetoId = request.ProjetoId,
                UsuarioId = request.UsuarioId,
                Status = request.Status,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var (lista, totalItems) = await _tarefaRepository.ListarAsync(filtro);

            var items = lista.Select(l => new TarefaResponse
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

            return new PaginacaoResponse<TarefaResponse>
            {
                Items = items,
                PageNumber = filtro.PageNumber,
                PageSize = filtro.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)filtro.PageSize)
            };
        }

        private async Task ValidarAlteracaoDeTituloOuProjeto(Tarefa tarefa, TarefaUpdateRequest request)
        {
            if (request.Titulo is not null && string.IsNullOrWhiteSpace(request.Titulo))
                throw new DomainException("Titulo da tarefa não pode ser vazio!", ErrorTypeEnum.Validation);

            var projetoFinalId = request.ProjetoId ?? tarefa.ProjetoId;
            var tituloFinal = request.Titulo ?? tarefa.Titulo;

            var projetoMudou = projetoFinalId != tarefa.ProjetoId;
            var tituloMudou = !tituloFinal.Equals(tarefa.Titulo, StringComparison.OrdinalIgnoreCase);

            if (projetoMudou || tituloMudou)
            {
                await ValidarTitulo(projetoFinalId, tituloFinal);
            }
        }

        private async Task ValidarTitulo(int projetoId, string titulo) 
        {
            if(await _tarefaRepository.ValidarTituloExistenteAsync(projetoId, titulo))
            {
                throw new DomainException("Já existe uma tarefa com esse nome no mesmo projeto!", ErrorTypeEnum.Conflict);
            }
        }
    }
}
