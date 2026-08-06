using HomePageApp.Core.Models.ToDoList;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Core.Interfaces.ToDoList
{
    public interface IToDoRepository
    {
        Task<List<ToDoItem>> GetAllTasksAsync();
        Task<List<ToDoItem>> GetCurrentTasksAsync();

        Task AddTaskAsync(ToDoItem item);
        Task DeleteTaskAsync(int id);
        Task SaveTaskAsync(ToDoItem item);
        Task ToggleTaskCompletionAsync(ToDoItem item);
    }
}
