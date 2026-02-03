using Microsoft.EntityFrameworkCore;
using Atelie.Api.Entities;

namespace Atelie.Api.Data
{
    public class AtelieDbContext : DbContext
    {
        public AtelieDbContext(DbContextOptions<AtelieDbContext> options) : base(options)
        {
        }

        // Estoque
        public DbSet<Material> Materiais { get; set; } 
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }

        // Peças Prontas e Vendas
        public DbSet<PecaPronta> PecasProntas { get; set; }
        public DbSet<PecaProntaMaterial> PecaProntaMateriais { get; set; }
        public DbSet<Venda> Vendas { get; set; }

        // Encomendas
        public DbSet<Encomenda> Encomendas { get; set; }

        // To-Do List
        public DbSet<ListaTarefa> ListasTarefa { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }

        // Financeiro
        public DbSet<MovimentacaoFinanceiro> MovimentacoesFinanceiro { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de relacionamentos
            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasOne(m => m.Material)
                .WithMany()
                .HasForeignKey(m => m.MaterialId);

            // PecaPronta - PecaProntaMaterial - Material
            modelBuilder.Entity<PecaProntaMaterial>()
                .HasOne(pm => pm.PecaPronta)
                .WithMany(p => p.Materiais)
                .HasForeignKey(pm => pm.PecaProntaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PecaProntaMaterial>()
                .HasOne(pm => pm.Material)
                .WithMany()
                .HasForeignKey(pm => pm.MaterialId);

            // Venda - PecaPronta
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.PecaPronta)
                .WithMany()
                .HasForeignKey(v => v.PecaProntaId);

            modelBuilder.Entity<Tarefa>()
                .HasOne(t => t.ListaTarefa)
                .WithMany(lt => lt.Tarefas)
                .HasForeignKey(t => t.ListaTarefaId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
