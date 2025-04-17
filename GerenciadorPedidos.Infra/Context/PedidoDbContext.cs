

using GerenciadorPedidos.Domain.Entities.Objects;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorPedidos.Infra.Context
{
    public class PedidoDbContext : DbContext
    {
        public PedidoDbContext(DbContextOptions<PedidoDbContext> options) : base(options)
        {
        }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PedidoId).IsRequired();
                entity.Property(e => e.ClienteId).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Imposto).HasPrecision(18, 2);

                entity.HasIndex(e => e.PedidoId).IsUnique();
                entity.HasIndex(e => e.Status);
            });

            modelBuilder.Entity<PedidoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProdutoId).IsRequired();
                entity.Property(e => e.Quantidade).IsRequired();
                entity.Property(e => e.Valor).HasPrecision(18, 2).IsRequired();

                entity.HasOne(e => e.Pedido)
                    .WithMany(p => p.Itens)
                    .HasForeignKey(e => e.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
