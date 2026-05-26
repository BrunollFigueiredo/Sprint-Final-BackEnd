using Microsoft.EntityFrameworkCore;
using BugTracker.Models;

namespace BugTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<Bug> Bugs { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Bug>()
            .HasOne(b => b.ReportadoPor)
            .WithMany(u => u.BugsReportados)
            .HasForeignKey(b => b.ReportadoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Bug>()
            .HasOne(b => b.AtribuidoPara)
            .WithMany(u => u.BugsAtribuidos)
            .HasForeignKey(b => b.AtribuidoParaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Bug>()
            .HasOne(b => b.Projeto)
            .WithMany(p => p.Bugs)
            .HasForeignKey(b => b.ProjetoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comentario>()
            .HasOne(c => c.Bug)
            .WithMany()
            .HasForeignKey(c => c.BugId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comentario>()
            .HasOne(c => c.Usuario)
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
