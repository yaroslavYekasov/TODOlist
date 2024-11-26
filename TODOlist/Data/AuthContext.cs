using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TODOlist.Data
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; } // Ensure this is included
    }

    public class User
    {
        public User()
        {
            Tasks = new List<TaskItem>();
        }

        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        // Navigation property
        public ICollection<TaskItem> Tasks { get; set; }
    }

    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        public string Description { get; set; }

        // Foreign key
        [Required]
        public int UserId { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
