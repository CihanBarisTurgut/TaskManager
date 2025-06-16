using TaskManager.DTOs;
using TaskManager.Enums;
using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Services.Tasks
{
    // Görev işlemleri için business logic servis sınıfı
    public class TaskService : ITaskService
    {
        // Repository katmanına erişim için dependency injection
        private readonly ITaskRepository _taskRepository;
        // Constructor - task repository'sini enjekte eder
        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        // Yeni görev oluşturma işlemi
        public async Task<bool> CreateAsync(CreateTaskDTO createTaskDTO)
        {
            // DTO'dan TaskItem model nesnesine manuel mapping
            var task = new TaskItem()
            {
                Title = createTaskDTO.Title,
                Description = createTaskDTO.Description,
                UserId = createTaskDTO.UserId,
                CreatedAt = DateTime.UtcNow,        // Oluşturulma zamanı şu an
                IsCompleted = false                 // Varsayılan olarak tamamlanmamış
            };
            // Repository aracılığıyla veritabanına kaydeder
            await _taskRepository.CreateAsync(task);
            return true;  // İşlem başarılı
        }
        // Görev raporlarını listeleme işlemi
        public async Task<List<TaskReportDTO>> ListAsync()
        {
            // Tüm görevleri repository'den alır
            var tasks = await _taskRepository.ListAsync();
            // Görevleri türlerine göre grupla ve rapor oluştur
            var groupedTasks = tasks
                .GroupBy(t => t.Type)                    // Görev tipine göre grupla
                .Select(group =>
                {
                    var taskList = group.ToList();
                    var completedCount = taskList.Count(t => t.IsCompleted);  // Tamamlanan görev sayısı
                    var totalCount = taskList.Count;                          // Toplam görev sayısı
                    var pendingCount = totalCount - completedCount;           // Bekleyen görev sayısı

                    return new TaskReportDTO
                    {
                        Type = (TaskTypes)group.Key,                          // Görev tipi
                        TotalTasks = totalCount,                              // Toplam görev sayısı
                        CompletedTasks = completedCount,                      // Tamamlanan görev sayısı
                        PendingTasks = pendingCount,                          // Bekleyen görev sayısı
                        CompletionRate = totalCount == 0 ? 0 :               // Tamamlanma oranı hesaplama
                            Math.Round((double)completedCount / totalCount * 100, 2),
                        Tasks = taskList.Select(t => new TaskResponseDTO     // Her görev için detay DTO oluştur
                        {
                            Id = t.Id,
                            Title = t.Title,
                            Description = t.Description,
                            CreatedAt = t.CreatedAt,
                            DueDate = t.DueDate,
                            IsCompleted = t.IsCompleted,
                            Type = (TaskTypes)t.Type,
                            UserId = t.UserId.ToString()
                        }).ToList()
                    };
                }).ToList();
            return groupedTasks;
        }
    }
}