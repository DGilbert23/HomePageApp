using HomePageApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Core.Interfaces
{
    public interface IToDoRepository
    {
        Task<List<ToDoItem>> GetAllTasksAsync();
        Task AddTaskAsync(ToDoItem item);
        Task DeleteTaskAsync(int id);
        Task SaveTaskAsync(ToDoItem item);
        Task ToggleTaskCompletionAsync(ToDoItem item);
    }
}
