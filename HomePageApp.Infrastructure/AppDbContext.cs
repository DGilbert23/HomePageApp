using HomePageApp.Core.Models;
using HomePageApp.Core.Models.BillTracker;
using HomePageApp.Core.Models.ToDoList;
using HomePageApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomePageApp.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<UserProfile> UserProfiles {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
