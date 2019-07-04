using System;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OAuth2SecurityService.Manager.DbContexts.SeedData;

namespace OAuth2SecurityService.Manager.DbContexts
{
    using System.Threading;

    public class DatabaseSeeding
    {
        public static void InitialisePersistedGrantDatabase(PersistedGrantDbContext persistedGrantDbContext,
                                                            SeedingType seedingType)
        {
            Boolean isDbInitialised = false;
            Int32 retryCounter = 0;
            while (retryCounter < 20 && !isDbInitialised)
            {
                try
                {
                    if (persistedGrantDbContext.Database.IsMySql())
                    {
                        persistedGrantDbContext.Database.Migrate();
                    }

                    persistedGrantDbContext.SaveChanges();

                    isDbInitialised = true;
                    break;
                }
                catch(Exception ex)
                {
                    retryCounter++;
                    Thread.Sleep(10000);
                }
            }

            if (!isDbInitialised)
            {
                String connString = persistedGrantDbContext.Database.GetDbConnection().ConnectionString;

                Exception newException = new Exception($"Error initialising Db with Connection String [{connString}]");
                throw newException;
            }
        }

        public static void InitialiseAuthenticationDatabase(AuthenticationDbContext authenticationDbContext,
                                                            SeedingType seedingType)
        {
            Boolean isDbInitialised = false;
            Int32 retryCounter = 0;
            while (retryCounter < 20 && !isDbInitialised)
            {
                try
                {
                    if (authenticationDbContext.Database.IsMySql())
                    {
                        authenticationDbContext.Database.Migrate();
                    }

                    AddRoles(authenticationDbContext, seedingType);
                    AddUsers(authenticationDbContext, seedingType);
                    AddUsersToRoles(authenticationDbContext, seedingType);

                    authenticationDbContext.SaveChanges();

                    isDbInitialised = true;
                    break;
                }
                catch(Exception ex)
                {
                    retryCounter++;
                    Thread.Sleep(10000);
                }
            }

            if (!isDbInitialised)
            {
                String connString = authenticationDbContext.Database.GetDbConnection().ConnectionString;

                Exception newException = new Exception($"Error initialising Db with Connection String [{connString}]");
                throw newException;
            }
        }

        public static void InitialiseConfigurationDatabase(ConfigurationDbContext configurationDbContext,
                                              SeedingType seedingType)
        {
            Boolean isDbInitialised = false;
            Int32 retryCounter = 0;
            while (retryCounter < 20 && !isDbInitialised)
            {
                try
                {
                    if (configurationDbContext.Database.IsMySql())
                    {
                        configurationDbContext.Database.Migrate();
                    }

                    AddClients(configurationDbContext, seedingType);
                    AddApiResources(configurationDbContext, seedingType);
                    AddIdentityResources(configurationDbContext, seedingType);

                    configurationDbContext.SaveChanges();

                    isDbInitialised = true;
                    break;
                }
                catch(Exception ex)
                {
                    retryCounter++;
                    Thread.Sleep(10000);
                }
            }

            if (!isDbInitialised)
            {
                String connString = configurationDbContext.Database.GetDbConnection().ConnectionString;

                Exception newException = new Exception($"Error initialising Db with Connection String [{connString}]");
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

        private static void AddIdentityResources(ConfigurationDbContext context, SeedingType seedingType)
        {
            var identityResources = IdentityResourceSeedData.GetIdentityResources(seedingType);

            foreach (var identityResource in identityResources)
            {
                var foundResource = context.IdentityResources.Any(a => a.Name == identityResource.Name);

                if (!foundResource)
                {
                    context.IdentityResources.Add(identityResource.ToEntity());
                }
            }
        }

        private static void AddUsers(AuthenticationDbContext context, SeedingType seedingType)
        {
            var identityUsers = IdentityUserSeedData.GetIdentityUsers(seedingType);

            foreach (var identityUser in identityUsers)
            {
                var foundUser = context.Users.Any(a => a.UserName== identityUser.UserName);

                if (!foundUser)
                {
                    context.Users.Add(identityUser);
                }
            }
        }

        private static void AddRoles(AuthenticationDbContext context, SeedingType seedingType)
        {
            var roles = RoleSeedData.GetIdentityRoles(seedingType);

            foreach (var role in roles)
            {
                var foundRole = context.Roles.Any(a => a.Name== role.Name);

                if (!foundRole)
                {
                    context.Roles.Add(role);
                }
            }
        }

        private static void AddUsersToRoles(AuthenticationDbContext context, SeedingType seedingType)
        {
            var identityUserRoles = IdentityUserRoleSeedData.GetIdentityUserRoles(seedingType);

            foreach (var identityUserRole in identityUserRoles)
            {
                var foundUserRole = context.UserRoles.Any(a => a.RoleId== identityUserRole.RoleId && a.UserId == identityUserRole.UserId);

                if (!foundUserRole)
                {
                    context.UserRoles.Add(identityUserRole);
                }
            }
        }
    }
}
