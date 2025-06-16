using TaskManager.Models;

namespace TaskManager.Repositories
{
    public interface ITaskRepository
    {
        Task CreateAsync(TaskItem task);
        Task<List<TaskItem>> ListAsync();
    }
}
