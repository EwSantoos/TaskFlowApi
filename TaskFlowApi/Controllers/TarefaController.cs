using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Application.Dto.Project;
using TaskFlowApi.Application.Dto.WorkItem;
using TaskFlowApi.Application.Interfaces.WorkItem;

namespace TaskFlowApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;

        public TarefaController(ITarefaService projetoService)
        {
            _tarefaService = projetoService;
        }

        [Authorize(Roles = "Administrador, Operacional")]
        [HttpPost("{projetoId}")]
        public async Task<ActionResult<TarefaResponse>> Criar(int projetoId, TarefaRequest request)
        {
            var response = await _tarefaService.CriarTarefaAsync(projetoId, request);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador, Operacional")]
        [HttpPut("{tarefaId}")]
        public async Task<ActionResult<TarefaResponse>> Atualizar(int tarefaId, TarefaRequest request)
        {
            var response = await _tarefaService.AtualizarTarefaAsync(tarefaId, request);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador, Operacional, Consulta")]
        [HttpGet("{id}")]
        public async Task<ActionResult<TarefaResponse>> BuscarPorId(int id)
        {
            var response = await _tarefaService.BuscarPorIdAsync(id);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador, Operacional")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Excluir(int id)
        {
            await _tarefaService.ExcluirAsync(id);

            return NoContent();
        }

        [Authorize(Roles = "Administrador, Operacional, Consulta")]
        [HttpGet]
        public async Task<ActionResult<List<ProjetoResponse>>> Listar()
        {
            var response = await _tarefaService.ListarAsync();

            return Ok(response);
        }
    }
}
