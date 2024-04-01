using AutenticacaoJwtAPI.Data;
using AutenticacaoJwtAPI.Dtos;
using AutenticacaoJwtAPI.Models;
using AutenticacaoJwtAPI.Services.SenhaService;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoJwtAPI.Services.AuthService
{
    public class AuthService : IAuthInterface
    {
        private readonly AppDbContext _context;
        private readonly ISenhaInterface _senhaInterface;

        public AuthService(AppDbContext context, ISenhaInterface senhaInterface)
        {
            _context = context;
            _senhaInterface = senhaInterface;
        }

        public async Task<Response<UsuarioCriacaoDto>> Registrar(UsuarioCriacaoDto usuarioRegistro)
        {
            Response<UsuarioCriacaoDto> respostaServico = new Response<UsuarioCriacaoDto>();

            try
            {
                //1º verificar se existe esse registro no banco
                //a ! é para que quando eu receber como resposta false (usuario existe), ele troque pra true e entre na condição
                if (!VerficaSeEmaileUsuarioNaoExiste(usuarioRegistro))
                {
                    respostaServico.Dados = null;
                    respostaServico.Status = false;
                    respostaServico.Mensagem = "Email/Usuário já cadastrado!";
                    return respostaServico;
                }

                //2º Criptorafar a senha recebida
                _senhaInterface.CriarSenhaHash(usuarioRegistro.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                //3º Passar do Dto recebido para o usuario model
                UsuarioModel usuario = new UsuarioModel()
                {
                    Usuario = usuarioRegistro.Usuario,
                    Email = usuarioRegistro.Email,
                    Cargo = usuarioRegistro.Cargo,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };

                //4º Salvar no banco
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                //5º Responder o usuário
                respostaServico.Mensagem = "Usuario criado com sucesso";


            }catch (Exception ex)
            {
                respostaServico.Dados = null;
                respostaServico.Mensagem = ex.Message;
                respostaServico.Status = false;
            }

            return respostaServico;
        }

        public async Task<Response<string>> Logar(UsuarioLoginDto usuarioLogin)
        {
            Response<string> respostaServico = new Response<string>();

            try
            {
                var usuario = await _context.Usuario.FirstOrDefaultAsync(userBanco => userBanco.Email == usuarioLogin.Email);

                //verifica se tem o usuario
                if(usuario == null)
                {
                    respostaServico.Mensagem = "Credenciais inválidas!";
                    respostaServico.Status = false;
                    return respostaServico;
                }

                //verifica se tem a senha
                if (!_senhaInterface.VerificaSenhaHash(usuarioLogin.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    respostaServico.Mensagem = "Credenciais inválidas!";
                    respostaServico.Status = false;
                    return respostaServico;
                }

                var token = _senhaInterface.CriarToken(usuario);

                respostaServico.Dados = token;
                respostaServico.Mensagem = "Usuário logado com sucesso!";

            }
            catch (Exception ex)
            {
                respostaServico.Dados = null;
                respostaServico.Mensagem = ex.Message;
                respostaServico.Status = false;
            }

            return respostaServico;

        }

        public bool VerficaSeEmaileUsuarioNaoExiste(UsuarioCriacaoDto usuarioRegistro)
        {
            var usuario = _context.Usuario.FirstOrDefault(userBanco => userBanco.Email == usuarioRegistro.Email || userBanco.Usuario ==  usuarioRegistro.Usuario);

            //se o usuario ou email já cadastrado retorno false
            if(usuario != null) return false;

            //se não tem nada no banco eu retorno true
            return true;
        }
    }
}
