using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Services;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth2SecurityService.Manager.DbContexts.SeedData;

namespace OAuth2SecurityService.Service
{
    /// <summary>
    /// 
    /// </summary>
    public static class StartupExtensions
    {
        #region Methods

        public static IIdentityServerBuilder AddIdentityServerStorage(this IIdentityServerBuilder builder,
                                                              String connectionString)
        {
            builder.AddConfigurationStore(connectionString);
            builder.AddOperationalStore(connectionString);

            return builder;
        }

        /// <summary>
        /// Adds the configuration store.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddConfigurationStore(this IIdentityServerBuilder builder,
                                                                   String connectionString)
        {
            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            return builder;
        }

        /// <summary>
        /// Adds the configuration store cache.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddConfigurationStoreCache(this IIdentityServerBuilder builder)
        {
            builder.Services.AddMemoryCache(); 
            builder.AddInMemoryCaching();

            // these need to be registered as concrete classes in DI for
            // the caching decorators to work
            builder.Services.AddTransient<ClientStore>();
            builder.Services.AddTransient<ResourceStore>();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();

            builder.AddOperationalStore();

            return builder;
        }


        /// <summary>
        /// Adds the operational store.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddOperationalStore(this IIdentityServerBuilder builder,
                                                                 String connectionString)
        {
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddIntegrationTestConfiguration(this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryClients(ClientSeedData.GetClients(SeedingType.IntegrationTest));
            builder.AddInMemoryApiResources(ApiResourceSeedData.GetApiResources(SeedingType.IntegrationTest));            
            builder.AddInMemoryUsers(IdentityUserSeedData.GetIdentityUsers(SeedingType.IntegrationTest));
            builder.AddInMemoryRoles(RoleSeedData.GetIdentityRoles(SeedingType.IntegrationTest));
            builder.AddInMemoryUserRoles(IdentityUserRoleSeedData.GetIdentityUserRoles(SeedingType.IntegrationTest));
            
            builder.AddInMemoryPersistedGrants();

            return builder;
        }

        public static IIdentityServerBuilder AddInMemoryUsers(this IIdentityServerBuilder builder, IEnumerable<IdentityUser> users)
        {
            builder.Services.AddSingleton(users);

            builder.Services.AddSingleton<IUserStore<IdentityUser>, InMemoryUserStore>();            

            return builder;
        }

        public static IIdentityServerBuilder AddInMemoryRoles(this IIdentityServerBuilder builder, IEnumerable<IdentityRole> roles)
        {
            builder.Services.AddSingleton(roles);   

            return builder;
        }

        public static IIdentityServerBuilder AddInMemoryUserRoles(this IIdentityServerBuilder builder, IEnumerable<IdentityUserRole<String>> userRoles)
        {
            builder.Services.AddSingleton(userRoles);   

            return builder;
        }

        #endregion
    }
}