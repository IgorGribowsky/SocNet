using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Infrastructure.EFRepository
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Repost> Reposts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserIdentity> UserIdentities { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>().HasKey(s => new { s.SubscriberUserId, s.TargetUserId });
            modelBuilder.Entity<Like>().HasKey(l => new { l.PostId, l.SenderUserId });

            modelBuilder.Entity<Post>().HasData(
                new Post[]
                {
                    new Post {Id = 1, Content = "Post 1", UserId = 1, CreationTime = DateTime.Now},
                    new Post {Id = 2, Content = "Post 2", UserId = 2, CreationTime = DateTime.Now},
                    new Post {Id = 3, Content = "Post 3", UserId = 3, CreationTime = DateTime.Now},
                    new Post {Id = 4, Content = "Post 4", UserId = 4, CreationTime = DateTime.Now}
                });

            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                    new User {Id = 1, FirstName = "Alex", SecondName = "Gribowski"},
                    new User {Id = 2, FirstName = "Pupa", SecondName = "Gribowski"},
                    new User {Id = 3, FirstName = "Lupa", SecondName = "Gribowski"}
                });

            modelBuilder.Entity<UserIdentity>().HasData(
                new UserIdentity[]
                {
                    new UserIdentity {Id = 1, UserId = 1, UserName = "grias", Password = "123123"},
                    new UserIdentity {Id = 2, UserId = 2, UserName = "ppa", Password = "123123"},
                    new UserIdentity {Id = 3, UserId = 3, UserName = "lupa", Password = "123123"}
                });
        }
    }
}
