using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Enums;

namespace TaskManager.Models
{
    public class TaskItem
    {

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime DueDate { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserId { get; set; }
        // Navigation property - ilişkili kullanıcı nesnesine erişim
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int Type { get; set; } = (int)TaskTypes.Daily;

    }
}
