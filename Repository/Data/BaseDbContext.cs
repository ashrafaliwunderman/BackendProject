using Core.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Emit;

namespace Repository.Data
{
    public class BaseDbContext : DbContext, IEntityContext
    {
        public virtual DbSet<Order> Order{ get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<OrderProduct> OrderProduct { get; set; }

        public BaseDbContext(DbContextOptions<BaseDbContext> options)
            : base(options)
        {

        }

        public BaseDbContext()
            : base()
        {

        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Status>().HasData(new Status
            {
                StatusID = 1,
                StatusName = "Pending",
            });
            
            modelbuilder.Entity<Status>().HasData(new Status
            {
                StatusID = 2,
                StatusName = "Approved",
            });

            modelbuilder.Entity<Status>().HasData(new Status
            {
                StatusID = 3,
                StatusName = "Rejected",
            });

            modelbuilder.Entity<Status>().HasData(new Status
            {
                StatusID = 4,
                StatusName = "Onprocess",
            });

            modelbuilder.Entity<OrderProduct>()
                .HasOne<Order>(o => o.Order)
                .WithMany(x => x.OrderProduct)
                .HasForeignKey(o => o.OrderID);
            //.HasForeignKey(o => o.ProductID);

            modelbuilder.Entity<OrderProduct>()
                .HasOne<Product>(o => o.Product)
                .WithMany(x => x.Orders)
                .HasForeignKey(o => o.ProductID);

            modelbuilder.Entity<Order>()
                .Property(x => x.CreatedDate)
                .HasDefaultValueSql("getdate()");
        }
    }
}
