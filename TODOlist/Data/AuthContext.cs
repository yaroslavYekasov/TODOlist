using Microsoft.EntityFrameworkCore;

namespace TODOlist.Data
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
