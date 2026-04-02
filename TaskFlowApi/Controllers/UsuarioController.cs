using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.User;
using TaskFlowApi.Domain.Enum;

namespace TaskFlowApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService) 
        {
            _usuarioService = usuarioService;
        }

        [Authorize(Roles = "Administrador, Operacional")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioResponse>> Atualizar(int id, UsuarioUpdateRequest resquest) 
        {
            var perfilLogado = Enum.Parse<PerfilAcessoEnum>(User.FindFirst(ClaimTypes.Role)?.Value);

            var response = await _usuarioService.AtualizarAsync(id, perfilLogado, resquest);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador, Operacional, Consulta")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponse>> BuscarPorId(int id) 
        {
            var response = await _usuarioService.BuscarPorIdAsync(id);

            return Ok(response);
        }

        [Authorize(Roles = "Administrador, Operacional")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Excluir(int id) 
        {
            await _usuarioService.ExcluirAsync(id);

            return NoContent();
        }

        [Authorize(Roles = "Administrador, Operacional, Consulta")]
        [HttpGet]
        public async Task<ActionResult<List<UsuarioResponse>>> Listar() 
        {
            var response = await _usuarioService.ListarAsync();

            return Ok(response);
        }

    }
}
