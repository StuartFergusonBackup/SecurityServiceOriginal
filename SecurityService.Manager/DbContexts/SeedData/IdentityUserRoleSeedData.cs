
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace OAuth2SecurityService.Manager.DbContexts.SeedData
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityUserRoleSeedData
    {
        /// <summary>
        /// Gets the identity user roles.
        /// </summary>
        /// <param name="seedingType">Type of the seeding.</param>
        /// <returns></returns>
        public static List<IdentityUserRole<String>> GetIdentityUserRoles(SeedingType seedingType)
        {
            List<IdentityUserRole<String>> identityUserRoles = new List<IdentityUserRole<String>>();

            if (seedingType == SeedingType.IntegrationTest)
            {
                identityUserRoles.AddRange(SeedTestUserRoles());
            }

            return identityUserRoles;
        }

        /// <summary>
        /// Seeds the test user roles.
        /// </summary>
        /// <returns></returns>
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