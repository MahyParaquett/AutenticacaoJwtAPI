using AutenticacaoJwtAPI.Dtos;
using AutenticacaoJwtAPI.Models;
using AutenticacaoJwtAPI.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoJwtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authInterface;
        public AuthController(IAuthInterface authInterface)
        {
            _authInterface = authInterface;
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register(UsuarioCriacaoDto usuarioRegister)
        {
            var resposta = await _authInterface.Registrar(usuarioRegister);

            return Ok(resposta);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UsuarioLoginDto usuarioLogin)
        {
           var resposta = await _authInterface.Logar(usuarioLogin);

            return Ok(resposta);
        }

    }
}
