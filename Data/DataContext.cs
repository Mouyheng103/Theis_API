using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace API.Data
{
  
    public class DataContext : IdentityDbContext<Users,Roles,string >
    {
        public DbSet<Branch> tblO_Branch { get; set; }
        public DbSet<Users> AspNetUsers { get; set; }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Ensure there are no configurations for PhoneNumber and PhoneNumberConfirmed
            builder.Entity<Users>().Ignore(u => u.PhoneNumber);
            builder.Entity<Users>().Ignore(u => u.PhoneNumberConfirmed);
        }
    }
}
