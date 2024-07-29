using API.Data.View;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace API.Data
{
  
    public class DataContext : IdentityDbContext<Users,Roles,string >
    {

        //Operation
        public DbSet<Branch> tblO_Branch { get; set; }
        public DbSet<Position> tblO_Position { get; set; }
        

        //Location
        public DbSet<Province> tblOL_Provinces { get; set; }
        public DbSet<District> tblOL_Districts { get; set; }
        public DbSet<Commune> tblOL_Communes { get; set; }
        public DbSet<Village> tblOL_Villages { get; set; }

        //Staff
        public DbSet<Staffs> tblO_Staff { get; set; }

        //Miller
        public DbSet<Millers> tblO_Miller {  get; set; }

        //Agent
        public DbSet<Agents> tblO_Agent { get; set; }
        public DbSet<ViewO_Agents> ViewO_Agents { get; set; }

        //Customer
        public DbSet<Customers> tblO_Customer { get; set; }
        public DbSet<ViewO_Customer> ViewO_Customers { get; set; }

        //User
        public DbSet<Users> AspNetUsers { get; set; }

        //View
        
        public DbSet<Address> ViewO_Address { get; set; }
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
