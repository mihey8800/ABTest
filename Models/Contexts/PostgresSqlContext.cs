using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Models.Contexts
{
    public sealed class PostgresSqlContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public PostgresSqlContext(DbContextOptions<PostgresSqlContext> options)
            : base(options)
        {
   
        }

    }
}