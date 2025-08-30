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
    public class UsersCartConfig : IEntityTypeConfiguration<UsersCart>
    {
        public void Configure(EntityTypeBuilder<UsersCart> builder)
        {
            builder.HasOne(up => up.User)
                .WithMany(u => u.UsersCart)
                .HasForeignKey(up => up.UserId);

            builder.HasOne(up => up.Product)
                .WithMany(p => p.UsersCart)
                .HasForeignKey(up => up.ProductID);
        }
    }
}
