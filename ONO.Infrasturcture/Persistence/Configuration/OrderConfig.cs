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
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(o => o.Address)
            .WithMany()
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.InventoryTransactions)
                .WithOne(it => it.Order)
                .HasForeignKey(it => it.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
