using Microsoft.EntityFrameworkCore;
using Atelie.Api.Entities;

namespace Atelie.Api.Data
{
    public class AtelieDbContext : DbContext
    {
        public AtelieDbContext(DbContextOptions<AtelieDbContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; } 
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
        public DbSet<MovimentacaoFinanceiro> MovimentacoesFinanceiro { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
        }

    }
}