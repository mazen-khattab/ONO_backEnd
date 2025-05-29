using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Persistence.Configuration
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u => u.UserRoles)  // Define the relationship
               .WithOne(ur => ur.User)
               .HasForeignKey(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.InventoryTransactions)
                .WithOne(it => it.User)
                .HasForeignKey(it => it.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
