using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp_SignalR2025.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public void Seed()
        {
            if (!Users.Any())
            {
                var hasher = new PasswordHasher<User>();

                var user1 = new User
                {
                    UserName = "testuser1",
                    Email = "testuser1@example.com",
                    IconPath = "path/to/icon1.png",
                    NormalizedUserName = "TESTUSER1",
                    NormalizedEmail = "TESTUSER1@EXAMPLE.COM"
                };
                user1.PasswordHash = hasher.HashPassword(user1, "Test1*");

                var user2 = new User
                {
                    UserName = "testuser2",
                    Email = "testuser2@example.com",
                    IconPath = "path/to/icon2.png",
                    NormalizedUserName = "TESTUSER2",
                    NormalizedEmail = "TESTUSER2@EXAMPLE.COM"
                };
                user2.PasswordHash = hasher.HashPassword(user2, "Test1*");

                Users.Add(user1);
                Users.Add(user2);

                SaveChanges();
            }
        }
    }
}
