using HomePageApp.Core.Interfaces.ToDoList;
using HomePageApp.Core.Models.BillTracker;
using HomePageApp.Core.Models.ToDoList;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Infrastructure.DemoInfrastructure.Repositories
{
    internal class DemoToDoRepository : IToDoRepository
    {
        private List<ToDoItem> toDoItems;

        public DemoToDoRepository()
        {
            toDoItems = GenerateDemoItems();
        }

        private List<ToDoItem> GenerateDemoItems()
        {
            List<ToDoItem> demoItems = new List<ToDoItem>();
            demoItems.Add(new ToDoItem
            {
                Id = 0,
                Title = "Water yard",
                CreatedDate = DateTime.Now.AddDays(-2),
                CompletedDate = DateTime.Now.AddDays(-1),
            });

            demoItems.Add(new ToDoItem
            {
                Id = 1,
                Title = "Mow yard",
                CreatedDate = DateTime.Now.AddDays(-1),                
            });

            demoItems.Add(new ToDoItem
            {
                Id = 2,
                Title = "Cancel lawn services",
                CreatedDate = DateTime.Now.AddDays(-7),
                DueDate = DateTime.Now.AddDays(-1),
            });

            demoItems.Add(new ToDoItem
            {
                Id = 3,
                Title = "Purchase decorations",
                Description = "Look for some nice themed decorations for the garden",
                CreatedDate = DateTime.Now.AddDays(-6),
                DueDate = DateTime.Now.AddDays(2),
            });

            return demoItems;
        }

        public Task AddTaskAsync(ToDoItem item)
        {
            item.Id = (toDoItems.MaxBy(x => x.Id)?.Id ?? 0) + 1;

            toDoItems.Add(item);

            return Task.CompletedTask;
        }

        public Task DeleteTaskAsync(int id)
        {
            var itemToRemove = toDoItems.Find(b => b.Id == id);
            if (itemToRemove != null)
                toDoItems.Remove(itemToRemove);
            else
                throw new InvalidOperationException("No ToDoItem with id " + id + " found to remove.");

            return Task.CompletedTask;
        }

        public Task<List<ToDoItem>> GetAllTasksAsync()
        {
            return Task.FromResult(toDoItems.OrderBy(t => t.CompletedDate != null)
                                            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                                            .ThenByDescending(t => t.CreatedDate)
                                            .ToList()
                                            );
        }

        public Task<List<ToDoItem>> GetCurrentTasksAsync()
        {
            return Task.FromResult(toDoItems.Where(t => t.CompletedDate > DateTime.Now.AddDays(-3) || t.CompletedDate == null)
                                            .OrderBy(t => t.CompletedDate != null)
                                            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                                            .ThenByDescending(t => t.CreatedDate)
                                            .ToList()
                                            );
        }

        public Task SaveTaskAsync(ToDoItem item)
        {
            var editItem = toDoItems.Find(t => t.Id == item.Id);

            if (editItem != null)
            {
                editItem.Title = item.Title;
                editItem.Description = item.Description;
                editItem.CreatedDate = item.CreatedDate;
                editItem.DueDate = item.DueDate;
                editItem.CompletedDate = item.CompletedDate;
            }
            else
                throw new InvalidOperationException("No ToDoItem with id " + item.Id + " found to save.");

            return Task.CompletedTask;
        }

        public Task ToggleTaskCompletionAsync(ToDoItem item)
        {
            var editItem = toDoItems.Find(i => i.Id == item.Id);
            if (editItem == null)
                throw new InvalidOperationException("No item found with ID " + item.Id + " to toggle completion");

            if (editItem.CompletedDate == null)
                editItem.CompletedDate = DateTime.Now;
            else
                editItem.CompletedDate = null;

            return Task.CompletedTask;
        }
    }
}
