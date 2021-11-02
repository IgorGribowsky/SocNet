using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Core.EFRepository
{
    class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Repost> Reposts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
