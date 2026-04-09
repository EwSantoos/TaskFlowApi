using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Application.Dto.Pagination;
using TaskFlowApi.Application.Dto.TaskItem;
using TaskFlowApi.Application.Interfaces.Services;

namespace TaskFlowApi.Controllers
{
    [Authorize]
    [Route("api/tarefas")]
    [ApiController]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;

        public TarefaController(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        [Authorize(Roles = "Administrador,Operacional")]
        [HttpPost("/api/projetos/{projetoId}/tarefas")]
        public async Task<ActionResult<TarefaResponse>> Criar(int projetoId, TarefaRequest request)
        {
            var response = await _tarefaService.CriarTarefaAsync(projetoId, request);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [Authorize(Roles = "Administrador,Operacional")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TarefaResponse>> Atualizar(int id, TarefaUpdateRequest request)
        {
            var response = await _tarefaService.AtualizarTarefaAsync(id, request);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador,Operacional,Consulta")]
        [HttpGet("{id}")]
        public async Task<ActionResult<TarefaResponse>> BuscarPorId(int id)
        {
            var response = await _tarefaService.ObterPorIdAsync(id);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador,Operacional")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _tarefaService.ExcluirAsync(id);

            return NoContent();
        }

        [Authorize(Roles = "Administrador,Operacional,Consulta")]
        [HttpGet]
        public async Task<ActionResult<PaginacaoResponse<TarefaResponse>>> Listar([FromQuery] TarefaFiltroRequest request)
        {
            var response = await _tarefaService.ListarAsync(request);

            return Ok(response);
        }
    }
}
