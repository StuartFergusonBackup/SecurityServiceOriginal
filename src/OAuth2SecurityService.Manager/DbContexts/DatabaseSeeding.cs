using System;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using OAuth2SecurityService.Manager.DbContexts.SeedData;

namespace OAuth2SecurityService.Manager.DbContexts
{
    public class DatabaseSeeding
    {
        public static void InitialiseDatabase(ConfigurationDbContext configurationDbContext, 
                                              PersistedGrantDbContext persistedGrantDbContext, 
                                              AuthenticationDbContext authenticationDbContext,
                                              SeedingType seedingType)
        {
            try
            {
                configurationDbContext.Database.Migrate();
                persistedGrantDbContext.Database.Migrate();
                authenticationDbContext.Database.Migrate();

                AddClients(configurationDbContext, seedingType);
                AddApiResources(configurationDbContext, seedingType);

                configurationDbContext.SaveChanges();
                persistedGrantDbContext.SaveChanges();
                authenticationDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                var connString = configurationDbContext.Database.GetDbConnection().ConnectionString;

                Exception newException = new Exception($"Connection String [{connString}]", ex);
                throw newException;
            }
        }

        private static void AddClients(ConfigurationDbContext context, SeedingType seedingType)
        {
            var clientsToAdd = ClientSeedData.GetClients(seedingType);

            foreach (var client in clientsToAdd)
            {
                var foundClient = context.Clients.Any(a => a.ClientId == client.ClientId);

                if (!foundClient)
                {
                    context.Clients.Add(client.ToEntity());
                }
            }
        }

        private static void AddApiResources(ConfigurationDbContext context, SeedingType seedingType)
        {
            var apiResources = ApiResourceSeedData.GetApiResources(seedingType);

            foreach (var apiResource in apiResources)
            {
                var foundResource = context.ApiResources.Any(a => a.Name == apiResource.Name);

                if (!foundResource)
                {
                    context.ApiResources.Add(apiResource.ToEntity());
                }
            }
        }

        
    }
}
