using cloud_backend.Models;

namespace cloud_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Quotation_Request> Quotation_Request { get; set; }
        public DbSet<Quotation_Request_Items> Quotation_Request_Items { get; set; }
        public DbSet<Quotations> Quotations { get; set; }
        public DbSet<Quotation_Items> Quotation_Items { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Order_Items> Order_Items { get; set; }
        public DbSet<Manufacturing_Request> Manufacturing_Request { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Receipts> Receipts { get; set; }
        public DbSet<User_Credentials> User_Credentials { get; set; }
        public DbSet<Staff_User> Staff_User { get; set; }
        public DbSet<Store_User> Store_User { get; set; }
        public DbSet<Customer_User> Customer_User { get; set; }

    }
}
