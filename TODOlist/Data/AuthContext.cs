using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;

namespace TODOlist.Data
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
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

        // Add Role property to differentiate between users and admins
        [Required]
        public string Role { get; set; } = "user"; // Default is "user"

        // Navigation property for tasks
        public ICollection<TaskItem> Tasks { get; set; }
    }

    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
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
        public User User { get; set; }
    }
}
