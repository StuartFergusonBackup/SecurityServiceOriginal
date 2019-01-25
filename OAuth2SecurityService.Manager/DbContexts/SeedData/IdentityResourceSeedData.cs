using System;
using System.Collections.Generic;
using System.Linq;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.Entities;
using IdentityResource = IdentityServer4.Models.IdentityResource;

namespace OAuth2SecurityService.Manager.DbContexts.SeedData
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityResourceSeedData
    {
        private static readonly Dictionary<String, IEnumerable<IdentityClaim>> ScopeToClaimsMapping =
            new Dictionary<String, IEnumerable<IdentityClaim>>
            {
                {
                    IdentityServerConstants.StandardScopes.Profile, new List<IdentityClaim>()
                    {
                        new IdentityClaim() {Type = JwtClaimTypes.Name},
                        new IdentityClaim() {Type = JwtClaimTypes.Role},
                        new IdentityClaim() {Type = JwtClaimTypes.Email},
                        new IdentityClaim() {Type = JwtClaimTypes.GivenName},
                        new IdentityClaim() {Type = JwtClaimTypes.MiddleName},
                        new IdentityClaim() {Type = JwtClaimTypes.FamilyName},
                    }
                },
                {
                    IdentityServerConstants.StandardScopes.OpenId, new List<IdentityClaim>()
                    {
                        new IdentityClaim()
                        {
                            Type = JwtClaimTypes.Subject
                        }
                    }
                }
            };

        /// <summary>
        /// Gets the identity resources.
        /// </summary>
        /// <param name="seedingType">Type of the seeding.</param>
        /// <returns></returns>
        public static List<IdentityResource> GetIdentityResources(SeedingType seedingType)
        {
            //List<IdentityResource> identityResources = new List<IdentityResource>();

            var g = ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.OpenId];

            return new List<IdentityResource>
            {
                new IdentityResource()
                {
                    Name = IdentityServerConstants.StandardScopes.OpenId,
                    DisplayName = "Your user identifier",
                    Required = true,
                    UserClaims = ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.OpenId].Select(x => x.Type).ToList()
                },
                new IdentityResource()
                {
                    Name = IdentityServerConstants.StandardScopes.Profile,
                    DisplayName = "User profile",
                    Description = "Your user profile information (first name, last name, etc.)",
                    Emphasize = true,
                    UserClaims = ScopeToClaimsMapping[IdentityServerConstants.StandardScopes.Profile].Select(x => x.Type).ToList()
                }

            };

            //return identityResources;
        }
    }
}