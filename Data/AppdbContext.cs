using IdentityOrnek.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityOrnek.Data
{
    public class AppdbContext : IdentityDbContext<AppUser>
    {
        public AppdbContext(DbContextOptions<AppdbContext> options) : base(options)
        {
        }

        public DbSet<Haber> Haberler { get; set; }
        public DbSet<Yorum> Yorumlar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Haberler tablosu için ilişki yapılandırması
            modelBuilder.Entity<Haber>()
                .HasOne(h => h.Yazar)
                .WithMany()
                .HasForeignKey(h => h.YazarId)
                .OnDelete(DeleteBehavior.SetNull);
        }


    }

}