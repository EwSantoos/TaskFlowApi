using Org.BouncyCastle.Asn1.Ocsp;
using TaskFlowApi.Application.Dto.Project;
using TaskFlowApi.Application.Dto.TaskItem;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Application.Interfaces.Services;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Application.Services
{
    public class ProjetoService : IProjetoService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ProjetoService(IProjetoRepository projetoRepository, IUsuarioRepository usuarioRepository)
        {
            _projetoRepository = projetoRepository;
            _usuarioRepository = usuarioRepository;
        }        

        public async Task<ProjetoResponse> CriarProjetoAsync(int usuarioLogadoId, ProjetoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome)) 
            {
                throw new DomainException("Nome do projeto é obrigatório!", ErrorTypeEnum.Validation);
            }

            var usuarioLogado = await _usuarioRepository.ObterPorIdAsync(usuarioLogadoId);
            if(usuarioLogado is null) 
            {
                throw new DomainException("Usuário autenticado não encontrado!", ErrorTypeEnum.Unauthorized);
            }

            if (await _projetoRepository.ValidarNomeExistenteAsync(request.Nome)) 
            {
                throw new DomainException("Já existe um projeto com esse nome!", ErrorTypeEnum.Conflict);
            }

            var projeto = Projeto.Criar(usuarioLogado.Id, request.Nome, request.Descricao);

            await _projetoRepository.AdicionarAsync(projeto);

            return new ProjetoResponse
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                UsuarioCriadorId = usuarioLogado.Id,
                NomeCriador = usuarioLogado.Nome,
                CriadoEm = projeto.CriadoEm
            };
        }

        public async Task<ProjetoResponse> AtualizarProjetoAsync(int projetoId, ProjetoUpdateRequest request)
        {
            if(projetoId <= 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            var projeto = await _projetoRepository.ObterPorIdAsync(projetoId);
            if(projeto is null) 
            {
                throw new DomainException("Projeto não encontrado!", ErrorTypeEnum.NotFound);
            }

            await ValidarAlteracaoDeNomeProjeto(request, projeto);

            projeto.Atualizar(request.Nome, request.Descricao);

            await _projetoRepository.AtualizarAsync(projeto);

            return new ProjetoResponse
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                UsuarioCriadorId = projeto.UsuarioCriadorId,
                NomeCriador = projeto.UsuarioCriador.Nome,
                CriadoEm = projeto.CriadoEm
            };
        }

        public async Task<ProjetoResponse> ObterPorIdAsync(int projetoId)
        {
            if (projetoId <= 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            var projeto = await _projetoRepository.ObterPorIdAsync(projetoId);
            if (projeto is null)
            {
                throw new DomainException("Projeto não encontrado!", ErrorTypeEnum.NotFound);
            }

            return new ProjetoResponse
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,    
                UsuarioCriadorId = projeto.UsuarioCriadorId,
                NomeCriador = projeto.UsuarioCriador.Nome,
                CriadoEm = projeto.CriadoEm,
                Tarefas = projeto.Tarefas.Select(t => new TarefaResponse
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Descricao = t.Descricao,
                    Status = t.Status,
                    CriadoEm = t.CriadoEm,
                    DataLimite = t.DataLimite,
                    ProjetoId = projeto.Id,
                    NomeProjeto = projeto.Nome,
                    UsuarioId = t.UsuarioId,
                    NomeUsuario = t.Usuario.Nome
                }).ToList()
            };
        }

        public async Task ExcluirAsync(int projetoId)
        {
            if (projetoId <= 0) { throw new DomainException("Id inválido!", ErrorTypeEnum.Validation); }

            var projeto = await _projetoRepository.ObterPorIdAsync(projetoId);
            if (projeto is null)
            {
                throw new DomainException("Projeto não encontrado!", ErrorTypeEnum.NotFound);
            }

            await _projetoRepository.RemoverAsync(projeto);
        }

        private async Task ValidarAlteracaoDeNomeProjeto(ProjetoUpdateRequest request, Projeto projeto) 
        {
            if (request.Nome is not null)
            {
                if (string.IsNullOrWhiteSpace(request.Nome))
                    throw new DomainException("Nome do projeto não pode ser vazio!", ErrorTypeEnum.Validation);

                if (!request.Nome.Trim().Equals(projeto.Nome, StringComparison.OrdinalIgnoreCase) &&
                await _projetoRepository.ValidarNomeExistenteAsync(request.Nome))
                {
                    throw new DomainException("Já existe um projeto com esse nome!", ErrorTypeEnum.Conflict);
                }
            }
        }

        public async Task<List<ProjetoResponse>> ListarAsync()
        {
            var lista = await _projetoRepository.ListarAsync();

            return lista.Select(l => new ProjetoResponse
            {
                Id = l.Id,
                Nome = l.Nome,
                Descricao = l.Descricao,
                CriadoEm = l.CriadoEm,
                UsuarioCriadorId = l.UsuarioCriadorId,
                NomeCriador = l.UsuarioCriador.Nome
            }).ToList();
        }
    }
}
