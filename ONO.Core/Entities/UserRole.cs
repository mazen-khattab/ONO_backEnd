using Microsoft.AspNetCore.Identity;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class UserRole : IdentityUserRole<int>, ISoftDeleteble
    {
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}