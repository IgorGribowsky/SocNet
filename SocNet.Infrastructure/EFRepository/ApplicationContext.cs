using Microsoft.EntityFrameworkCore;
using System;
using SocNet.Core.Entities;

namespace SocNet.Infrastructure.EFRepository;

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
                    new Post {Id = 3, Content = "Post 3", UserId = 3, CreationTime = DateTime.Now}
            });

        modelBuilder.Entity<User>().HasData(
            new User[]
            {
                    new User {Id = 1, FirstName = "Alex", SecondName = "Gribowski"},
                    new User {Id = 2, FirstName = "Pupa", SecondName = "Dupa"},
                    new User {Id = 3, FirstName = "Lupa", SecondName = "Zupa"}
            });

        modelBuilder.Entity<UserIdentity>().HasData(
            new UserIdentity[]
            {
                    new UserIdentity {Id = 1, UserId = 1, UserName = "grias", Password = "123"},
                    new UserIdentity {Id = 2, UserId = 2, UserName = "ppa", Password = "123"},
                    new UserIdentity {Id = 3, UserId = 3, UserName = "lupa", Password = "123"}
            });

        modelBuilder.Entity<Subscription>().HasData(
            new Subscription[]
            {
                    new Subscription {SubscriberUserId = 1, TargetUserId = 2},
                    new Subscription {SubscriberUserId = 1, TargetUserId = 3},
                    new Subscription {SubscriberUserId = 3, TargetUserId = 1}
            });
    }
}
