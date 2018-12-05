using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using Client = IdentityServer4.Models.Client;
using Secret = IdentityServer4.Models.Secret;

namespace OAuth2SecurityService.Manager.DbContexts.SeedData
{
    public class ClientSeedData
    {
        #region Public Methods

        #region public static List<Client> GetClients()        
        /// <summary>
        /// Gets the clients.
        /// </summary>
        /// <returns></returns>
        public static List<Client> GetClients(SeedingType seedingType)
        {
            List<Client> clients = new List<Client>();
                       
            // Developer Client
            clients.Add(CreateDeveloperClient(seedingType));

            // Mobile Client
            clients.Add(CreateMobileAppClient(seedingType));

            // Web Client
            clients.Add(CreateWebAppClient(seedingType));
            
            return clients;
        }
        #endregion

        #endregion

        #region Private Methods

        #region private static Client CreateDeveloperClient(SeedingType seedingType) 
        /// <summary>
        /// Creates the developer client.
        /// </summary>
        /// <param name="seedingType">Type of the seeding.</param>
        /// <returns></returns>
        private static Client CreateDeveloperClient(SeedingType seedingType)
        {
            Client client = null;

            // Setup the scopes
            List<String> scopes = new List<String>();
            scopes.AddRange(ApiResourceSeedData.GetApiResources(seedingType).Select(y => y.Name).ToList());

            if (seedingType == SeedingType.IntegrationTest)
            {
                client = new Client
                {
                    ClientId = "integrationTestClient",
                    ClientName = "Integration Test Client",
                    ClientSecrets = {new Secret("integrationTestClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = scopes,                    
                };
            }
            else if (seedingType == SeedingType.Development || seedingType == SeedingType.Staging)
            {
                client = new Client
                {
                    ClientId = "developerClient",
                    ClientName = "Developer Client",
                    ClientSecrets = {new Secret("developerClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = scopes
                };
            }
            else if (seedingType == SeedingType.Production)
            {
                // TODO
            }

            return client;
        }
        #endregion

        #region private static Client CreateMobileAppClient(SeedingType seedingType)        
        /// <summary>
        /// Creates the mobile application client.
        /// </summary>
        /// <param name="seedingType">Type of the seeding.</param>
        /// <returns></returns>
        private static Client CreateMobileAppClient(SeedingType seedingType)
        {
            Client client = null;

            // Setup the scopes
            List<String> scopes = new List<String>();
            scopes.AddRange(ApiResourceSeedData.GetApiResources(seedingType)
                .Where(a => a.Name == "managementapi.player.read" ||
                            a.Name == "managementapi.player.write" ||
                            a.Name == "managementapi.player.reports").Select(y => y.Name).ToList());

            if (seedingType == SeedingType.IntegrationTest)
            {
                client = new Client
                {
                    ClientId = "integrationTestMobileClient",
                    ClientName = "Integration Test Mobile Client",
                    ClientSecrets = {new Secret("integrationTestMobileClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = scopes,                    
                };
            }
            else if (seedingType == SeedingType.Development || seedingType == SeedingType.Staging)
            {
                client = new Client
                {
                    ClientId = "golfhandicap.mobile",
                    ClientName = "Mobile App Client",
                    ClientSecrets = {new Secret("golfhandicap.mobile".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = scopes
                };
            }
            else if (seedingType == SeedingType.Production)
            {
                // TODO
            }

            return client;
        }
        #endregion

        #region private static Client CreateWebAppClient(SeedingType seedingType)        
        /// <summary>
        /// Creates the web application client.
        /// </summary>
        /// <param name="seedingType">Type of the seeding.</param>
        /// <returns></returns>
        private static Client CreateWebAppClient(SeedingType seedingType)
        {
            Client client = null;

            // Setup the scopes
            List<String> scopes = new List<String>();
            scopes.AddRange(ApiResourceSeedData.GetApiResources(seedingType).Select(y => y.Name).ToList());

            if (seedingType == SeedingType.IntegrationTest)
            {
                client = new Client
                {
                    ClientId = "integrationTestWebClient",
                    ClientName = "Integration Test Web Client",
                    ClientSecrets = {new Secret("integrationTestWebClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowedScopes = scopes,                    
                };
            }
            else if (seedingType == SeedingType.Development || seedingType == SeedingType.Staging)
            {
                client = new Client
                {
                    ClientId = "golfhandicap.web",
                    ClientName = "Web App Client",
                    ClientSecrets = {new Secret("golfhandicap.web".Sha256())},
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowedScopes = scopes
                };
            }
            else if (seedingType == SeedingType.Production)
            {
                // TODO
            }

            return client;
        }
        #endregion

        #endregion
    }
}
