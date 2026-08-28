using Microsoft.EntityFrameworkCore;
using MedicacionAPI.Models;

namespace MedicacionAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Horario> Horarios { get; set; }
        public DbSet<Confirmacion> Confirmaciones { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<ContactoFamiliar> ContactosFamiliares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Rol)
                .HasMaxLength(20);

            // Medicamento → Usuario (CASCADE)
            modelBuilder.Entity<Medicamento>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.Medicamentos)
                .HasForeignKey(m => m.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Horario → Medicamento (CASCADE)
            modelBuilder.Entity<Horario>()
                .HasOne(h => h.Medicamento)
                .WithMany(m => m.Horarios)
                .HasForeignKey(h => h.IdMedicamento)
                .OnDelete(DeleteBehavior.Cascade);

            // Confirmacion → Usuario (NO ACTION)
            modelBuilder.Entity<Confirmacion>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Confirmaciones)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

            // Confirmacion → Medicamento (NO ACTION)
            modelBuilder.Entity<Confirmacion>()
                .HasOne(c => c.Medicamento)
                .WithMany()
                .HasForeignKey(c => c.IdMedicamento)
                .OnDelete(DeleteBehavior.NoAction);

            // Confirmacion → Horario (NO ACTION)
            modelBuilder.Entity<Confirmacion>()
                .HasOne(c => c.Horario)
                .WithMany(h => h.Confirmaciones)
                .HasForeignKey(c => c.IdHorario)
                .OnDelete(DeleteBehavior.NoAction);

            // Alerta → Usuario (NO ACTION)
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.Alertas)
                .HasForeignKey(a => a.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

            // Alerta → Medicamento (NO ACTION)
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Medicamento)
                .WithMany()
                .HasForeignKey(a => a.IdMedicamento)
                .OnDelete(DeleteBehavior.NoAction);

            // Alerta → Horario (NO ACTION)
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Horario)
                .WithMany(h => h.Alertas)
                .HasForeignKey(a => a.IdHorario)
                .OnDelete(DeleteBehavior.NoAction);

            // ContactoFamiliar → Usuario (CASCADE, UNIQUE)
            modelBuilder.Entity<ContactoFamiliar>()
                .HasOne(cf => cf.Usuario)
                .WithOne(u => u.ContactoFamiliar)
                .HasForeignKey<ContactoFamiliar>(cf => cf.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContactoFamiliar>()
                .HasIndex(cf => cf.IdUsuario)
                .IsUnique();
        }
    }
}