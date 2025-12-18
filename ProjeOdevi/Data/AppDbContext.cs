using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjeOdevi.Models;

namespace ProjeOdevi.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ServiceType> ServiceTypes { get; set; } = default!;
        public DbSet<Trainer> Trainers { get; set; } = default!;
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; } = default!;
        public DbSet<TrainerServiceType> TrainerServiceTypes { get; set; } = default!;
        public DbSet<Appointment> Appointments { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TrainerServiceType>()
                .HasKey(x => new { x.TrainerId, x.ServiceTypeId });

            builder.Entity<TrainerServiceType>()
                .HasOne(x => x.Trainer)
                .WithMany(t => t.TrainerServiceTypes)
                .HasForeignKey(x => x.TrainerId);

            builder.Entity<TrainerServiceType>()
                .HasOne(x => x.ServiceType)
                .WithMany()
                .HasForeignKey(x => x.ServiceTypeId);

            builder.Entity<ServiceType>()
                .Property(x => x.Price)
                .HasPrecision(10, 2);

            builder.Entity<Appointment>()
                .Property(x => x.PriceSnapshot)
                .HasPrecision(10, 2);
        }
    }
}
