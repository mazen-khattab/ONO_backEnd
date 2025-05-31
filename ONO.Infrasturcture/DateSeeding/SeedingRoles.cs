using Microsoft.AspNetCore.Identity;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Infrasturcture.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.DateSeeding
{
    public static class SeedingRoles
    {
        public static async Task SeedingAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            bool isOwnerExists = await roleManager.RoleExistsAsync(StaticUserRoles.OWNER),
             isAdminExists = await roleManager.RoleExistsAsync(StaticUserRoles.ADMIN),
             isUserExists = await roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (!isOwnerExists && !isAdminExists && !isUserExists)
            {
                await roleManager.CreateAsync(new Role
                {
                    Name = StaticUserRoles.USER,
                    NormalizedName = StaticUserRoles.USER.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });

                await roleManager.CreateAsync(new Role
                {
                    Name = StaticUserRoles.ADMIN,
                    NormalizedName = StaticUserRoles.ADMIN.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });

                await roleManager.CreateAsync(new Role
                {
                    Name = StaticUserRoles.OWNER,
                    NormalizedName = StaticUserRoles.OWNER.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }

            if (!userManager.Users.Any())
            {
                var user = new User()
                {
                    Fname = "mazen",
                    Lname = "khattab",
                    UserName = "MK",
                    Email = "mazenkhtab11@gmail.com",
                    PhoneNumber = "01023839637"
                };

                await userManager.CreateAsync(user, "Mak.01023839637");
                await userManager.AddToRoleAsync(user, StaticUserRoles.USER);
                await userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);
                await userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);
            }
        }
    }
}
