using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Application.Dto.Project;
using TaskFlowApi.Application.Interfaces.Services;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Controllers
{
    [Authorize]
    [Route("api/projetos")]
    [ApiController]
    public class ProjetoController : ControllerBase
    {
        private readonly IProjetoService _projetoService;

        public ProjetoController(IProjetoService projetoService)
        {
            _projetoService = projetoService;
        }

        [Authorize(Roles = "Administrador,Operacional")]
        [HttpPost]
        public async Task<ActionResult<ProjetoResponse>> Criar(ProjetoRequest request)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int usuarioLogadoId))
            {
                throw new DomainException("Usuário autenticado inválido!", ErrorTypeEnum.Unauthorized);
            }

            var response = await _projetoService.CriarProjetoAsync(usuarioLogadoId, request);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [Authorize(Roles = "Administrador,Operacional")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjetoResponse>> Atualizar(int id, ProjetoUpdateRequest request)
        {
            var response = await _projetoService.AtualizarProjetoAsync(id, request);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador,Operacional,Consulta")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjetoResponse>> BuscarPorId(int id)
        {
            var response = await _projetoService.ObterPorIdAsync(id);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador,Operacional")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _projetoService.ExcluirAsync(id);

            return NoContent();
        }

        [Authorize(Roles = "Administrador,Operacional,Consulta")]
        [HttpGet]
        public async Task<ActionResult<List<ProjetoResponse>>> Listar() 
        {
            var response = await _projetoService.ListarAsync();

            return Ok(response);
        }
    }
}
