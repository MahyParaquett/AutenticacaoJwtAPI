using AutenticacaoJwtAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace AutenticacaoJwtAPI.Services.SenhaService
{
    public class SenhaService : ISenhaInterface
    {
        private readonly IConfiguration _config;
        public SenhaService(IConfiguration config) 
        {
            _config = config;
        }

        public void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                //Chave para criar a senha hash
                senhaSalt = hmac.Key;

                //senha criptografada
                senhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
            }
        }

        public bool VerificaSenhaHash(string senha, byte[]senhaHash, byte[] senhaSalt)
        {
            using (var hmac = new HMACSHA512(senhaSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
                return computedHash.SequenceEqual(senhaHash);
            }
        }

        public string CriarToken(UsuarioModel usuario)
        {
            //O que vai ter dentro do token
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Cargo", usuario.Cargo.ToString()),
                new Claim("Email", usuario.Email),
                new Claim ("Username", usuario.Usuario)
            };

            //Chave do token
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //Credenciais e métodos de criptografia
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            //Estrutura e tempo pra expirar
            var token = new JwtSecurityToken(
                claims: claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials: cred);

            //Transformar em string
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            return jwt;
        }

        
    }
  
}
