using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAPI
{
    public class UserContext : DbContext
    {
        public UserContext()
        {
        }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Password> Passwords { get; set; }
    }
}
