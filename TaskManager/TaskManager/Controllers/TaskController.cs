using Microsoft.AspNetCore.Mvc;
using TaskManager.DTOs;
using TaskManager.Services.Tasks;

namespace TaskManager.Controllers
{
    // API controller olarak işaretlenir ve route yapılandırması
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        // Task işlemleri için servis bağımlılığı
        private readonly ITaskService _taskService;

        // Constructor - task service'i enjekte eder
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // POST api/task/create endpoint'i - yeni görev oluşturur
        [HttpPost("create")]
        public async Task<bool> CreateAsync(CreateTaskDTO createTaskDTO)
        {
            // Görev oluşturma işlemini task service'e yönlendirir
            return await _taskService.CreateAsync(createTaskDTO);
        }

        // POST api/task/list endpoint'i - görevleri listeler
        [HttpPost("list")]
        public async Task<List<TaskReportDTO>> ListAsync()
        {
            // Tüm görevleri listeleme işlemini task service'e yönlendirir
            return await _taskService.ListAsync();
        }
    }
}