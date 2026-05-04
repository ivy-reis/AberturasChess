using LivroAberturasAPI.Models;
using Microsoft.EntityFrameworkCore;
// tabelas; exclusão em cascata
namespace LivroAberturasAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Abertura> Aberturas { get; set; }
    public DbSet<Variante> Variantes { get; set; }
    public DbSet<Partida> Partidas { get; set; }
    public DbSet<Precisao> Precisoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Partida>()
            .HasOne(p => p.Precisao)
            .WithOne(a => a.Partida)
            .HasForeignKey<Precisao>(a => a.PartidaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 