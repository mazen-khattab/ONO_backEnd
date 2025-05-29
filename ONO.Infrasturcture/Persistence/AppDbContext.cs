using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Persistence
{
    public class AppDbContext : IdentityDbContext<User, Role, int,
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");

            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

        }
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted && e.Entity is ISoftDeleteble))
            {
                var entity = (ISoftDeleteble)entry.Entity;
                entity.IsDeleted = true;
                entry.State = EntityState.Modified;
            }
            return base.SaveChanges();
        }
    }
}
