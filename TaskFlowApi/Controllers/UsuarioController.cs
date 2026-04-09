using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Application.Dto.Pagination;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.Services;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Controllers
{
    [Authorize ]
    [Route("api/usuarios")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService) 
        {
            _usuarioService = usuarioService;
        }

        [HttpPost()]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<UsuarioResponse>> Criar(UsuarioRequest request)
        {
            var response = await _usuarioService.CriarUsuarioAsync(request);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [Authorize(Roles = "Administrador,Operacional")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioResponse>> Atualizar(int id, UsuarioUpdateRequest request) 
        {
            if(!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int usuarioLogadoId)) 
            {
                throw new DomainException("Usuário autenticado inválido!", ErrorTypeEnum.Unauthorized);
            }

            var response = await _usuarioService.AtualizarAsync(id, usuarioLogadoId, request);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador,Operacional,Consulta")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponse>> BuscarPorId(int id) 
        {
            var response = await _usuarioService.ObterPorIdAsync(id);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador,Operacional")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id) 
        {
            await _usuarioService.ExcluirAsync(id);

            return NoContent();
        }

        [Authorize(Roles = "Administrador,Operacional,Consulta")]
        [HttpGet]
        public async Task<ActionResult<PaginacaoResponse<UsuarioResponse>>> Listar([FromQuery] UsuarioFiltroRequest request) 
        {
            var response = await _usuarioService.ListarAsync(request);

            return Ok(response);
        }

    }
}
