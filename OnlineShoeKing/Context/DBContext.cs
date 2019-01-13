

namespace OnlineShoeKing.Context
{
    using System.Data.Entity;

    using Microsoft.AspNet.Identity.EntityFramework;

    using Models;

    public class DBContext : IdentityDbContext<UserModel>
    {
        public DBContext()
            : base("OnlineShoeKing")
        {
        }

        public static DBContext Create()
        {
            return new DBContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gallery>().ToTable("dbo.Galleries");
            modelBuilder.Entity<Message>().ToTable("dbo.Messages");
            modelBuilder.Entity<HomePage>().ToTable("dbo.HomePage");
            modelBuilder.Entity<Cart>().ToTable("dbo.Carts");
            modelBuilder.Entity<Category>().ToTable("dbo.Categories");
            modelBuilder.Entity<Order>().ToTable("dbo.Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("dbo.OrderDetails");
            modelBuilder.Entity<OrderStatus>().ToTable("dbo.OrderStatus");
            modelBuilder.Entity<OrderMessage>().ToTable("dbo.OrderMessages");
            modelBuilder.Entity<Product>().ToTable("dbo.Products");
            modelBuilder.Entity<PhotoOfProduct>().ToTable("dbo.PhotoOfProducts");
            modelBuilder.Entity<SizeOfProduct>().ToTable("dbo.SizeOfProducts");
            modelBuilder.Entity<About>().ToTable("dbo.About");
            modelBuilder.Entity<Comment>().ToTable("dbo.Comments");
            modelBuilder.Entity<Rating>().ToTable("dbo.Ratings");

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Gallery> Galleries { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<HomePage> HomePage { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderMessage> OrderMessages { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<PhotoOfProduct> PhotoOfProducts { get; set; }
        public virtual DbSet<SizeOfProduct> SizeOfProducts { get; set; }
        public virtual DbSet<About> About { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
    }
}