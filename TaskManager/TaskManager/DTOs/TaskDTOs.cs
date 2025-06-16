using System.ComponentModel.DataAnnotations;
using TaskManager.Enums;
using TaskManager.Models;

namespace TaskManager.DTOs
{
    // Yeni görev oluşturmak için kullanılan veri transfer nesnesi
    public class CreateTaskDTO
    {
        public int UserId { get; set; }               // Görevi oluşturan kullanıcının ID'si

        [Required]                                   // Başlık zorunlu alan
        [StringLength(200)]                          // Maksimum 200 karakter
        public string Title { get; set; }

        [StringLength(1000)]                         // Açıklama maksimum 1000 karakter
        public string Description { get; set; }

        public DateTime? DueDate { get; set; }       // İsteğe bağlı bitiş tarihi

        public int Type { get; set; } = (int)TaskTypes.Daily;  // Varsayılan olarak günlük görev tipi
    }

    // Mevcut görevi güncellemek için kullanılan veri transfer nesnesi
    public class UpdateTaskDTO
    {
        [Required]                                   // Başlık zorunlu alan
        [StringLength(200)]                          // Maksimum 200 karakter
        public string Title { get; set; }

        [StringLength(1000)]                         // Açıklama maksimum 1000 karakter
        public string Description { get; set; }

        public DateTime? DueDate { get; set; }       // İsteğe bağlı bitiş tarihi
        public TaskTypes Type { get; set; }          // Görev tipi enum
        public bool IsCompleted { get; set; }        // Görev tamamlanma durumu
    }

    // Görev bilgilerini döndürmek için kullanılan veri transfer nesnesi
    public class TaskResponseDTO
    {
        public int Id { get; set; }                  // Görev benzersiz kimliği
        public string Title { get; set; }            // Görev başlığı
        public string Description { get; set; }      // Görev açıklaması
        public DateTime CreatedAt { get; set; }      // Oluşturulma tarihi
        public DateTime? DueDate { get; set; }       // Bitiş tarihi
        public bool IsCompleted { get; set; }        // Tamamlanma durumu
        public DateTime? CompletedAt { get; set; }   // Tamamlanma tarihi
        public TaskTypes Type { get; set; }          // Görev tipi
        public string UserId { get; set; }           // Görev sahibi kullanıcı ID'si
    }

    // Görev raporu için kullanılan veri transfer nesnesi
    public class TaskReportDTO
    {
        public TaskTypes Type { get; set; }          // Görev tipi
        public int TotalTasks { get; set; }          // Toplam görev sayısı
        public int CompletedTasks { get; set; }      // Tamamlanan görev sayısı
        public int PendingTasks { get; set; }        // Bekleyen görev sayısı
        public double CompletionRate { get; set; }   // Tamamlanma oranı
        public List<TaskResponseDTO> Tasks { get; set; } = new List<TaskResponseDTO>();  // Görev listesi
    }
}