using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace OAuth2SecurityService.Manager.DbContexts.SeedData
{
    public class RoleSeedData
    {
        public static List<IdentityRole> GetIdentityRoles(SeedingType seedingType)
        {
            List<IdentityRole> identityRoles = new List<IdentityRole>();

            if (seedingType == SeedingType.IntegrationTest)
            {
                identityRoles.AddRange(SeedTestRoles());
            }

            return identityRoles;
        }

        public static List<IdentityRole> SeedTestRoles()
        {
            List<IdentityRole> testRoles = new List<IdentityRole>();
            
            testRoles.Add(new IdentityRole
            {
                Name = "Club Administrator",
                Id = "19A43DAA-D8EB-4EBC-B9EC-391EE0BF0BBF"
            });
            testRoles.Add(new IdentityRole
            {
                Name = "Match Secretary",
                Id = "346942A6-7BB2-4A03-ADF7-043700B4DE62"
            });
            testRoles.Add(new IdentityRole
            {
                Name = "Player",
                Id = "9CA99C33-E266-487C-BA37-A1B2264CABAE"
            });

            return testRoles;
        }
    }
}