using HomePageApp.Core.Models.BillTracker;
using HomePageApp.Core.Models.ToDoList;
using Microsoft.EntityFrameworkCore;

namespace HomePageApp.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<Bill> Bills { get; set; }
    }
}
