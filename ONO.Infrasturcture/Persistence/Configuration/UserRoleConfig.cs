using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Persistence.Configuration
{
    public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // Composite primary key
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.HasQueryFilter(ur => !ur.IsDeleted);
        }
    }
}
