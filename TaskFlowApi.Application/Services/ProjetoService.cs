using TaskFlowApi.Application.Dto.Project;
using TaskFlowApi.Application.Dto.WorkItem;
using TaskFlowApi.Application.Interfaces.Project;
using TaskFlowApi.Domain.Entities;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Application.Services
{
    public class ProjetoService : IProjetoService
    {
        private readonly IProjetoRepository _projetoRepository;

        public ProjetoService(IProjetoRepository projetoRepository)
        {
            _projetoRepository = projetoRepository;
        }        

        public async Task<ProjetoResponse> CriarProjetoAsync(int idUser, ProjetoRequest request)
        {
            if (await _projetoRepository.ValidarNomeExistenteAsync(request.Nome)) 
            {
                throw new DomainException("Já existe um projeto com esse nome!", ErrorTypeEnum.Conflict);
            }

            var projeto = Projeto.Criar(idUser, request.Nome, request.Descricao);

            await _projetoRepository.AdicionarAsync(projeto);

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

        public async Task<ProjetoResponse> AtualizarProjetoAsync(int id, ProjetoRequest request)
        {
            if(id == 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            var projeto = await _projetoRepository.ObterPorIdAsync(id);

            if(projeto == null) 
            {
                throw new DomainException("Projeto não encontrado!", ErrorTypeEnum.NotFound);
            }

            if (await _projetoRepository.ValidarNomeExistenteAsync(request.Nome))
            {
                throw new DomainException("Já existe um projeto com esse nome!", ErrorTypeEnum.Conflict);
            }

            projeto.Atualizar(request.Nome, request.Descricao);

            await _projetoRepository.AtualizarAsync(projeto);

            return new ProjetoResponse
            {
                Nome = projeto.Nome,
                Descricao = projeto.Descricao
            };
        }

        public async Task<ProjetoResponse> BuscarPorIdAsync(int id)
        {
            if (id == 0) throw new DomainException("Id inválido!", ErrorTypeEnum.Validation);

            var projeto = await _projetoRepository.ObterPorIdAsync(id);

            if (projeto == null)
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
                    ProjetoId = t.ProjetoId,
                    NomeProjeto = t.Projeto.Nome,
                    UsuarioId = t.UsuarioId,
                    NomeUsuario = t.Usuario.Nome
                }).ToList()
            };
        }

        public async Task ExcluirAsync(int id)
        {
            if (id == 0) { throw new DomainException("Id inválido!", ErrorTypeEnum.Validation); }

            var projeto = await _projetoRepository.ObterPorIdAsync(id);

            if (projeto == null)
            {
                throw new DomainException("Projeto não encontrado!", ErrorTypeEnum.NotFound);
            }

            await _projetoRepository.RemoverAsync(projeto);
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
