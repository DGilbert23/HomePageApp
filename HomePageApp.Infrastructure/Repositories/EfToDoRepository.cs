using HomePageApp.Core.Interfaces;
using HomePageApp.Core.Models.ToDoList;
using Microsoft.EntityFrameworkCore;

namespace HomePageApp.Infrastructure.Repositories
{
    public class EfToDoRepository : IToDoRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly IUserAccountService _userAccountService;

        public EfToDoRepository(IDbContextFactory<AppDbContext> dbFactory, IUserAccountService userAccountService)
        {
            _dbFactory = dbFactory;
            _userAccountService = userAccountService;
        }

        public async Task<List<ToDoItem>> GetAllTasksAsync()
        {
            var userId = await GetCurrentUserId();

            using var context = _dbFactory.CreateDbContext();
            return await context.ToDoItems.Where(t => t.UserId == userId)
                                          .OrderBy(t => t.CompletedDate != null)
                                          .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                                          .ThenByDescending(t => t.CreatedDate).ToListAsync<ToDoItem>();
        }

        public async Task<List<ToDoItem>> GetCurrentTasksAsync()
        {
            var userId = await GetCurrentUserId();

            using var context = _dbFactory.CreateDbContext();
            return await context.ToDoItems.Where(t => t.UserId == userId && (t.CompletedDate > DateTime.Now.AddDays(-3) || t.CompletedDate == null))
                                          .OrderBy(t => t.CompletedDate != null)
                                          .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                                          .ThenByDescending(t => t.CreatedDate).ToListAsync<ToDoItem>();
        }

        public async Task AddTaskAsync(ToDoItem task)
        {
            task.UserId = await GetCurrentUserId();

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
                var userId = await GetCurrentUserId();
                if (target.UserId != userId)
                    throw new InvalidOperationException("Authenticated UserId does not match record to save");

                context.ToDoItems.Remove(target);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Unable to find ToDoItems record to delete.");
            }
        }

        public async Task SaveTaskAsync(ToDoItem updatedItem)
        {
            var userId = await GetCurrentUserId();
            if (updatedItem.UserId != userId)
                throw new InvalidOperationException("Authenticated UserId does not match record to save");

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
            var userId = await GetCurrentUserId();
            if (item.UserId != userId)
                throw new InvalidOperationException("Authenticated UserId does not match record to save");

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

        private async Task<int> GetCurrentUserId()
        {
            return await _userAccountService.GetCurrentUserProfileId();
        }
    }
}
