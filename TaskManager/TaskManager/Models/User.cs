using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    
    public class User : IdentityUser<int>
    {
        // Id property'si IdentityUser<int>'ten otomatik gelir
        [Required]                                          // Ad alanı zorunlu
        public string FirstName { get; set; }
        [Required]                                          // Soyad alanı zorunlu
        public string LastName { get; set; }

        // UserName ve Email property'leri IdentityUser'dan otomatik gelir

        // Navigation property - kullanıcının görevleri (one-to-many ilişki)
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Hesap oluşturulma tarihi (varsayılan: şu an)
    }
}