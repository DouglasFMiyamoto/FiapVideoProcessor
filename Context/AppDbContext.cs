using FiapVideoProcessor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiapVideoProcessor.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Video> Videos => Set<Video>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired();
            });

            // Video
            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.FileName).IsRequired();
                entity.Property(v => v.Status).IsRequired().HasMaxLength(50);
                entity.HasOne(v => v.User)
                      .WithMany(u => u.Videos)
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
