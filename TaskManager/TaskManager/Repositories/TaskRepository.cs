using Microsoft.EntityFrameworkCore;
using System;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories
{
    // Görev veritabanı işlemlerini yöneten repository sınıfı
    public class TaskRepository : ITaskRepository
    {
        // Veritabanı bağlamı için dependency injection
        private readonly AppDataContext _context;
        // Constructor - veritabanı context'ini enjekte eder
        public TaskRepository(AppDataContext context)
        {
            _context = context;
        }
        // Yeni görev oluşturma işlemi
        public async Task CreateAsync(TaskItem task)
        {
            // Görevi veritabanına asenkron olarak ekler
            await _context.Tasks.AddAsync(task);
            // Değişiklikleri veritabanına kaydeder
            await _context.SaveChangesAsync();
        }
        // Tüm görevleri listeleme işlemi
        public async Task<List<TaskItem>> ListAsync()
        {
            // Tüm görevleri asenkron olarak listeye çevirir ve döndürür
            return await _context.Tasks.ToListAsync();
        }
    }
}