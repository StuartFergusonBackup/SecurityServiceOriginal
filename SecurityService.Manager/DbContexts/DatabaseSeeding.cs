namespace SecurityService.Manager.DbContexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using IdentityServer4.EntityFramework.DbContexts;
    using IdentityServer4.EntityFramework.Mappers;
    using IdentityServer4.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using SeedData;

    /// <summary>
    /// 
    /// </summary>
    public class DatabaseSeeding
    {
        #region Methods

        /// <summary>
        /// Initialises the authentication database.
        /// </summary>
        /// <param name="authenticationDbContext">The authentication database context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        public static void InitialiseAuthenticationDatabase(AuthenticationDbContext authenticationDbContext,
                                                            SeedingType seedingType)
        {
            Boolean isDbInitialised = false;
            Int32 retryCounter = 0;
            while (retryCounter < 20 && !isDbInitialised)
            {
                try
                {
                    if (authenticationDbContext.Database.IsSqlServer())
                    {
                        authenticationDbContext.Database.Migrate();
                    }

                    DatabaseSeeding.AddRoles(authenticationDbContext, seedingType);
                    DatabaseSeeding.AddUsers(authenticationDbContext, seedingType);
                    DatabaseSeeding.AddUsersToRoles(authenticationDbContext, seedingType);

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

        /// <summary>
        /// Initialises the configuration database.
        /// </summary>
        /// <param name="configurationDbContext">The configuration database context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        public static void InitialiseConfigurationDatabase(ConfigurationDbContext configurationDbContext,
                                                           SeedingType seedingType)
        {
            Boolean isDbInitialised = false;
            Int32 retryCounter = 0;
            while (retryCounter < 20 && !isDbInitialised)
            {
                try
                {
                    if (configurationDbContext.Database.IsSqlServer())
                    {
                        configurationDbContext.Database.Migrate();
                    }

                    DatabaseSeeding.AddClients(configurationDbContext, seedingType);
                    DatabaseSeeding.AddApiResources(configurationDbContext, seedingType);
                    DatabaseSeeding.AddIdentityResources(configurationDbContext, seedingType);

                    //configurationDbContext.SaveChanges();

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

        /// <summary>
        /// Initialises the persisted grant database.
        /// </summary>
        /// <param name="persistedGrantDbContext">The persisted grant database context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        public static void InitialisePersistedGrantDatabase(PersistedGrantDbContext persistedGrantDbContext,
                                                            SeedingType seedingType)
        {
            Boolean isDbInitialised = false;
            Int32 retryCounter = 0;
            while (retryCounter < 20 && !isDbInitialised)
            {
                try
                {
                    if (persistedGrantDbContext.Database.IsSqlServer())
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

        /// <summary>
        /// Adds the API resources.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        private static void AddApiResources(ConfigurationDbContext context,
                                            SeedingType seedingType)
        {
            List<ApiResource> apiResources = ApiResourceSeedData.GetApiResources(seedingType);

            foreach (ApiResource apiResource in apiResources)
            {
                Boolean foundResource = context.ApiResources.Any(a => a.Name == apiResource.Name);

                if (!foundResource)
                {
                    context.ApiResources.Add(apiResource.ToEntity());
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Adds the clients.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        private static void AddClients(ConfigurationDbContext context,
                                       SeedingType seedingType)
        {
            List<Client> clientsToAdd = ClientSeedData.GetClients(seedingType);

            foreach (Client client in clientsToAdd)
            {
                Boolean foundClient = context.Clients.Any(a => a.ClientId == client.ClientId);

                if (!foundClient)
                {
                    context.Clients.Add(client.ToEntity());
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Adds the identity resources.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        private static void AddIdentityResources(ConfigurationDbContext context,
                                                 SeedingType seedingType)
        {
            List<IdentityResource> identityResources = IdentityResourceSeedData.GetIdentityResources(seedingType);

            foreach (IdentityResource identityResource in identityResources)
            {
                Boolean foundResource = context.IdentityResources.Any(a => a.Name == identityResource.Name);

                if (!foundResource)
                {
                    context.IdentityResources.Add(identityResource.ToEntity());
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Adds the roles.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        private static void AddRoles(AuthenticationDbContext context,
                                     SeedingType seedingType)
        {
            List<IdentityRole> roles = RoleSeedData.GetIdentityRoles(seedingType);

            foreach (IdentityRole role in roles)
            {
                Boolean foundRole = context.Roles.Any(a => a.Name == role.Name);

                if (!foundRole)
                {
                    context.Roles.Add(role);
                }
            }
        }

        /// <summary>
        /// Adds the users.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        private static void AddUsers(AuthenticationDbContext context,
                                     SeedingType seedingType)
        {
            List<IdentityUser> identityUsers = IdentityUserSeedData.GetIdentityUsers(seedingType);

            foreach (IdentityUser identityUser in identityUsers)
            {
                Boolean foundUser = context.Users.Any(a => a.UserName == identityUser.UserName);

                if (!foundUser)
                {
                    context.Users.Add(identityUser);
                }
            }
        }

        /// <summary>
        /// Adds the users to roles.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        private static void AddUsersToRoles(AuthenticationDbContext context,
                                            SeedingType seedingType)
        {
            List<IdentityUserRole<String>> identityUserRoles = IdentityUserRoleSeedData.GetIdentityUserRoles(seedingType);

            foreach (IdentityUserRole<String> identityUserRole in identityUserRoles)
            {
                Boolean foundUserRole = context.UserRoles.Any(a => a.RoleId == identityUserRole.RoleId && a.UserId == identityUserRole.UserId);

                if (!foundUserRole)
                {
                    context.UserRoles.Add(identityUserRole);
                }
            }
        }

        #endregion
    }
}