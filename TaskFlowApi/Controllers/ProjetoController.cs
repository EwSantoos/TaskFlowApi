using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Application.Dto.Project;
using TaskFlowApi.Application.Interfaces.Project;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjetoController : ControllerBase
    {
        private readonly IProjetoService _projetoService;

        public ProjetoController(IProjetoService projetoService)
        {
            _projetoService = projetoService;
        }

        [Authorize(Roles = "Administrador, Operacional")]
        [HttpPost]
        public async Task<ActionResult<ProjetoResponse>> Criar(ProjetoRequest request)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(user, out var idUser);

            var response = await _projetoService.CriarProjetoAsync(idUser, request);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador, Operacional")]
        [HttpPut]
        public async Task<ActionResult<ProjetoResponse>> Atualizar(int id, ProjetoRequest request)
        {
            var response = await _projetoService.AtualizarProjetoAsync(id, request);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador, Operacional, Consulta")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjetoResponse>> BuscarPorId(int id)
        {
            var response = await _projetoService.BuscarPorIdAsync(id);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador, Operacional")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Excluir(int id)
        {
            await _projetoService.ExcluirAsync(id);

            return NoContent();
        }

        [Authorize(Roles = "Administrador, Operacional, Consulta")]
        [HttpGet]
        public async Task<ActionResult<List<ProjetoResponse>>> Listar() 
        {
            var response = await _projetoService.ListarAsync();

            return Ok(response);
        }
    }
}
