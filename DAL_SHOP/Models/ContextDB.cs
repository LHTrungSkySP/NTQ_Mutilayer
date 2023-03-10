using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DAL_SHOP.Models
{
    public partial class ContextDB : DbContext
    {
        public ContextDB()
            : base("name=ContextDB")
        {
        }

        public virtual DbSet<Category> Categorys { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WishList> WishLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.ProductDetail)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Medias)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Reviews)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.WishLists)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Review>()
                .Property(e => e.Content)
                .IsUnicode(false);

            modelBuilder.Entity<Shop>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.Shop)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Reviews)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.WishLists)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }

        //public System.Data.Entity.DbSet<BO_SHOP.Models.ProductRow> ProductRows { get; set; }
    }
}
