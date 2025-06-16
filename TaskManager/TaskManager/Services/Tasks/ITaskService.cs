using TaskManager.DTOs;

namespace TaskManager.Services.Tasks
{
    public interface ITaskService
    {
        Task<bool> CreateAsync (CreateTaskDTO createTaskDTO);
        Task<List<TaskReportDTO>> ListAsync ();
        
    }
}
