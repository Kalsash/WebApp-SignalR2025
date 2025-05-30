﻿using Microsoft.AspNetCore.Identity;
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
                    IconPath = "https://i.pinimg.com/736x/43/a2/f9/43a2f9d3651715320c76620e8c18b0a2.jpg",
                    NormalizedUserName = "TESTUSER1",
                    NormalizedEmail = "TESTUSER1@EXAMPLE.COM"
                };
                user1.PasswordHash = hasher.HashPassword(user1, "Test1*");

                var user2 = new User
                {
                    UserName = "testuser2",
                    Email = "testuser2@example.com",
                    IconPath = "https://avatars.mds.yandex.net/get-yapic/25358/hVfmUhEXkheMn3SzCUQwMWMgo8-1/orig",
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
