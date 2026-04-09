using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Application.Dto.Token;
using TaskFlowApi.Application.Dto.User;
using TaskFlowApi.Application.Interfaces.Auth;

namespace TaskFlowApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("token")]
        public async Task<ActionResult<TokenResponse>> Token(UsuarioLogin login)
        {
            var response = await _authService.TokenAsync(login);

            return Ok(response);
        }
    }
}
