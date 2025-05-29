using Microsoft.AspNetCore.Identity;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class Role : IdentityRole<int>, ISoftDeleteble
    {
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public bool IsDeleted { get; set; }
    }
}
