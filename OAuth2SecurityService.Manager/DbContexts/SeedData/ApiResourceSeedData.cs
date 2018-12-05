using System.Collections.Generic;
using IdentityServer4.Models;

namespace OAuth2SecurityService.Manager.DbContexts.SeedData
{
    public class ApiResourceSeedData
    {
        #region public static List<ApiResource> GetApiResources(SeedingType seedingType)        
        /// <summary>
        /// Gets the API resources.
        /// </summary>
        /// <returns></returns>
        public static List<ApiResource> GetApiResources(SeedingType seedingType)
        {
            List<ApiResource> apiResources = new List<ApiResource>();
               
            apiResources.Add(new ApiResource("managementapi.player.read"));
            apiResources.Add(new ApiResource("managementapi.player.write"));
            apiResources.Add(new ApiResource("managementapi.player.reports"));

            apiResources.Add(new ApiResource("managementapi.club.read"));
            apiResources.Add(new ApiResource("managementapi.club.write"));
            apiResources.Add(new ApiResource("managementapi.club.reports"));

            apiResources.Add(new ApiResource("managementapi.tournament.read"));
            apiResources.Add(new ApiResource("managementapi.tournament.write"));
            apiResources.Add(new ApiResource("managementapi.tournament.reports"));            
            
            return apiResources;
        }
        #endregion
    }
}
