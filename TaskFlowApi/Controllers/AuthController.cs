using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.Auth;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authSerice) 
        {
            _authService = authSerice;
        }

        [HttpPost("Registrar")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<UsuarioResponse>> Criar(UsuarioRequest request) 
        {
            var response = await _authService.CriarUsuarioAsync(request);

            return Ok(response);
        } 
        
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UsuarioLogin login) 
        {
            var response = await _authService.LoginAsync(login);

            return Ok(response);
        }
    }
}
