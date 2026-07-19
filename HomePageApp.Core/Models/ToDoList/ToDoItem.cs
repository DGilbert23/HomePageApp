using System.ComponentModel.DataAnnotations;

namespace HomePageApp.Core.Models.ToDoList
{
    public class ToDoItem
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Completed Date")]
        [DataType(DataType.Date)]
        public DateTime? CompletedDate { get; set; }

        public int UserId {  get; set; }
    }
}
