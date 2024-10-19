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
        

        //Staff
        public DbSet<Staffs> tblO_Staff { get; set; }
        public DbSet<Payroll> tblO_Staff_Payroll { get; set; }
        public DbSet<Salary> tblO_Staff_Salary { get; set; }
        public DbSet<SalaryChange> tblO_Staff_SalaryChange { get; set; }

        //Miller
        public DbSet<Millers> tblO_Miller {  get; set; }
        public DbSet<RicePurchase> tblS_RicePurchase {  get; set; }
        public DbSet<RicePurchasePayment> tblS_RicePurchase_Payment {  get; set; }
        public DbSet<PaymentComponent> tblS_PaymentComponent {  get; set; }

        //Miller
        public DbSet<ShareToBranch> tblS_ShareToBranch { get; set; }
        public DbSet<TakeRice> tblS_TakeRice { get; set; }

        //Agent
        public DbSet<Agents> tblO_Agent { get; set; }
        public DbSet<ViewO_Agents> ViewO_Agents { get; set; }
        public DbSet<Order> tblP_Order { get; set; }
        public DbSet<Provide> tblP_Provide { get; set; }

        //Customer
        public DbSet<Customers> tblO_Customer { get; set; }
        public DbSet<ViewO_Customer> ViewO_Customers { get; set; }
        public DbSet<CollectMoney> tblCol_Collect { get; set; }
        public DbSet<CusLoan> tblP_CusLoan { get; set; }

        //User
        public DbSet<Users> AspNetUsers { get; set; }
        public DbSet<Roles> AspNetRoles { get; set; }
        public DbSet<ViewAuth_UserRole> ViewAuth_UserRole { get; set; }
        public DbSet<ViewO_Users> ViewO_Users { get; set; }

        //View

        public DbSet<Address> ViewO_Address { get; set; }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Users>().Ignore(u => u.PhoneNumber);
            builder.Entity<Users>().Ignore(u => u.PhoneNumberConfirmed);
        }
    }
}
