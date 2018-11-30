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
            var testUser1 = new IdentityUser
            {
                UserName = "integrationtest",
                NormalizedUserName = "integrationtest",
                Email = "integrationtest@test.co.uk",
                NormalizedEmail = "integrationtest@test.co.uk",
                EmailConfirmed = true,
                Id = "84DC3E90-16B9-441D-B940-37BAA2AC53AF",                                       
            };
            testUser1.PasswordHash = passwordHasher.HashPassword(testUser1, "123456");
            testUsers.Add(testUser1);

            return testUsers;
        }

    }
}
