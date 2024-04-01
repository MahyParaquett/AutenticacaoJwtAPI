using System.ComponentModel.DataAnnotations;

namespace AutenticacaoJwtAPI.Dtos
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "O campo email é obrigatório"), EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo senha é obrigatório")]
        public string Senha { get; set; }
    }
}
