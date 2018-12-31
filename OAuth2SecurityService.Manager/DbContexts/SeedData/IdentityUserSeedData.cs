using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Identity;

namespace OAuth2SecurityService.Manager.DbContexts.SeedData
{
    public class IdentityUserSeedData
    {
        public static List<IdentityUser> GetIdentityUsers(SeedingType seedingType)
        {
            List<IdentityUser> identityUsers = new List<IdentityUser>();

            if (seedingType == SeedingType.IntegrationTest)
            {
                identityUsers.AddRange(SeedTestUsers());
            }

            return identityUsers;
        }

        public static List<IdentityUser> SeedTestUsers()
        {
            PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();

            List<IdentityUser> testUsers = new List<IdentityUser>();
            var clubadministrator = new IdentityUser
            {
                UserName = "clubadministrator@test.co.uk",
                NormalizedUserName = "clubadministrator@test.co.uk".ToUpper(),
                Email = "clubadministrator@test.co.uk",
                NormalizedEmail = "clubadministrator@test.co.uk".ToUpper(),
                EmailConfirmed = true,
                Id = "84DC3E90-16B9-441D-B940-37BAA2AC53AF",                                                                       
            };
            clubadministrator.PasswordHash = passwordHasher.HashPassword(clubadministrator, "123456");
            testUsers.Add(clubadministrator);

            var player = new IdentityUser
            {
                UserName = "player@test.co.uk",
                NormalizedUserName = "player@test.co.uk".ToUpper(),
                Email = "player@test.co.uk",
                NormalizedEmail = "player@test.co.uk".ToUpper(),
                EmailConfirmed = true,
                Id = "84DC3E90-16B9-441D-B940-37BAA2AC53AE"
            };
            player.PasswordHash = passwordHasher.HashPassword(player, "123456");
            testUsers.Add(player);
            
            return testUsers;
        }
    }

    public class IdentityUserRoleSeedData
    {
        public static List<IdentityUserRole<String>> GetIdentityUserRoles(SeedingType seedingType)
        {
            List<IdentityUserRole<String>> identityUserRoles = new List<IdentityUserRole<String>>();

            if (seedingType == SeedingType.IntegrationTest)
            {
                identityUserRoles.AddRange(SeedTestUserRoles());
            }

            return identityUserRoles;
        }

        public static List<IdentityUserRole<String>> SeedTestUserRoles()
        {
            List<IdentityUserRole<String>> identityUserRoles = new List<IdentityUserRole<String>>();

            identityUserRoles.Add(new IdentityUserRole<String>()
            {
                RoleId = "19A43DAA-D8EB-4EBC-B9EC-391EE0BF0BBF",
                UserId = "84DC3E90-16B9-441D-B940-37BAA2AC53AF"
            });
            identityUserRoles.Add(new IdentityUserRole<String>()
            {
                RoleId = "9CA99C33-E266-487C-BA37-A1B2264CABAE",
                UserId = "84DC3E90-16B9-441D-B940-37BAA2AC53AE"
            });

            return identityUserRoles;
        }
    }
}
