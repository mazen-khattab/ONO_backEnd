using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Persistence.Configuration
{
    public class UserProductsConfig : IEntityTypeConfiguration<UserProducts>
    {
        public void Configure(EntityTypeBuilder<UserProducts> builder)
        {
            builder.HasOne(up => up.User)
                .WithMany(u => u.UserProducts)
                .HasForeignKey(up => up.UserId);

            builder.HasOne(up => up.Product)
                .WithMany(p => p.UserProducts)
                .HasForeignKey(up => up.ProductID);
        }
    }
}
