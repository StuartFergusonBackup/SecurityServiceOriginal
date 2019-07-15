namespace SecurityService.Manager.DbContexts.SeedData
{
    using System;
    using System.Collections.Generic;
    using IdentityServer4.Models;

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
               
            apiResources.Add(new ApiResource("managementapi", new List<String> { "GolfClubId", "PlayerId", "role" } ));
            apiResources.Add(new ApiResource("securirtyserviceapi"));

            return apiResources;
        }
        #endregion
    }
}
