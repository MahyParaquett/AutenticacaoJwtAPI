using AutenticacaoJwtAPI.Dtos;
using AutenticacaoJwtAPI.Models;

namespace AutenticacaoJwtAPI.Services.AuthService
{
    public interface IAuthInterface
    {
        Task<Response<UsuarioCriacaoDto>> Registrar(UsuarioCriacaoDto usuarioRegistro);
        Task<Response<string>> Logar(UsuarioLoginDto usuarioLogin);
    }
}
