using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    // Identity yapısını kullanarak veritabanı context'i oluşturur
    public class AppDataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        // Constructor - DbContext seçeneklerini base class'a geçirir
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
        {
        }

        // TaskItem tablosu için DbSet tanımı
        public DbSet<TaskItem> Tasks { get; set; }

        // Model yapılandırmasını tanımlar
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity tablolarının yapılandırmasını çağırır
            base.OnModelCreating(modelBuilder);

            // TaskItem tablosu için yapılandırma
            modelBuilder.Entity<TaskItem>(entity =>
            {
                // Primary key tanımı
                entity.HasKey(e => e.Id);

                // Title alanı zorunlu ve maksimum 200 karakter
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                // Description alanı maksimum 1000 karakter
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                // CreatedAt alanı varsayılan olarak UTC tarihi
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Type enum'unu integer olarak saklar
                entity.Property(e => e.Type)
                    .HasConversion<int>();

                // User ile TaskItem arasında one-to-many ilişki
                entity.HasOne(t => t.User)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde görevler de silinir

                // Performans için index'ler
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsCompleted);
                entity.HasIndex(e => e.CreatedAt);
            });

            // User tablosu için yapılandırma
            modelBuilder.Entity<User>(entity =>
            {
                // FirstName alanı zorunlu ve maksimum 50 karakter
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                // LastName alanı zorunlu ve maksimum 50 karakter
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                // CreatedAt alanı varsayılan olarak UTC tarihi
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}