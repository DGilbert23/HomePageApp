using HomePageApp.Core.Interfaces;
using HomePageApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Infrastructure.Repositories
{
    public class EfToDoRepository : IToDoRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public EfToDoRepository(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<ToDoItem>> GetAllTasksAsync()
        {
            using var context = _dbFactory.CreateDbContext();
            return await context.ToDoItems.OrderBy(t => t.CompletedDate != null)
                                          .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                                          .ThenByDescending(t => t.CreatedDate).ToListAsync<ToDoItem>();
        }

        public async Task AddTaskAsync(ToDoItem task)
        {
            using var context = _dbFactory.CreateDbContext();
            context.ToDoItems.Add(task);
            await context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            using var context = _dbFactory.CreateDbContext();
            var target = await context.ToDoItems.FindAsync(id);

            if (target != null)
            {
                context.ToDoItems.Remove(target);
                await context.SaveChangesAsync();
            }
        }

        public async Task SaveTaskAsync(ToDoItem updatedItem)
        {
            using var context = _dbFactory.CreateDbContext();
            var currentItem = await context.ToDoItems.FindAsync(updatedItem.Id);

            if (currentItem != null)
            {
                currentItem.Title = updatedItem.Title;
                currentItem.Description = updatedItem.Description;
                currentItem.DueDate = updatedItem.DueDate;

                await context.SaveChangesAsync();
            }
        }

        public async Task ToggleTaskCompletionAsync(ToDoItem item)
        {
            using var context = _dbFactory.CreateDbContext();
            var currentItem = await context.ToDoItems.FindAsync(item.Id);

            if (currentItem != null)
            {
                if (currentItem.CompletedDate == null)
                    currentItem.CompletedDate = DateTime.Now;
                else
                    currentItem.CompletedDate = null;

                await context.SaveChangesAsync();
            }
        }
    }
}
