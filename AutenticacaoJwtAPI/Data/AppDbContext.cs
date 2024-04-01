using AutenticacaoJwtAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoJwtAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UsuarioModel> Usuario {get; set;}
    }
}
