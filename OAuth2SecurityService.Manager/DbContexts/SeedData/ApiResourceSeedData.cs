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
               
            apiResources.Add(new ApiResource("managementapi"));                      
            
            return apiResources;
        }
        #endregion
    }
}
