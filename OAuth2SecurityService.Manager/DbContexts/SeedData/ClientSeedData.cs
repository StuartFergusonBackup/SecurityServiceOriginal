using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4;
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
            clients.Add(ClientSeedData.CreateDeveloperClient(seedingType));

            // Golf Handicapping Clients
            clients.Add(ClientSeedData.CreateGolfHandicapMobileClient(seedingType));
            clients.Add(ClientSeedData.CreateGolfHandicapSubscriptionServiceClient(seedingType));
            clients.Add(ClientSeedData.CreateGolfHandicapTestDataGeneratorClient(seedingType)); 
            
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
            
            if (seedingType == SeedingType.IntegrationTest || seedingType == SeedingType.Development || seedingType == SeedingType.Staging)
            {
                // Add in the standard scopes for GetUserInfo
                scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
                scopes.Add(IdentityServerConstants.StandardScopes.Profile);
                scopes.Add(IdentityServerConstants.StandardScopes.Email);

                client = new Client
                {
                    ClientId = "developerClient",
                    ClientName = "Developer Client",
                    ClientSecrets = {new Secret("developerClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = scopes
                };
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
        private static Client CreateGolfHandicapMobileClient(SeedingType seedingType)
        {
            Client client = null;

            // Setup the scopes
            List<String> scopes = new List<String>();
            scopes.AddRange(ApiResourceSeedData.GetApiResources(seedingType).Select(y => y.Name).ToList());

            // Add in the standard scopes for GetUserInfo
            scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            scopes.Add(IdentityServerConstants.StandardScopes.Profile);
            scopes.Add(IdentityServerConstants.StandardScopes.Email);

            if (seedingType == SeedingType.IntegrationTest || seedingType == SeedingType.Development || seedingType == SeedingType.Staging)
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

            return client;
        }
        #endregion

        #region private static Client CreateGolfHandicapSubscriptionServiceClient(SeedingType seedingType)        
        /// <summary>
        /// Creates the mobile application client.
        /// </summary>
        /// <param name="seedingType">Type of the seeding.</param>
        /// <returns></returns>
        private static Client CreateGolfHandicapSubscriptionServiceClient(SeedingType seedingType)
        {
            Client client = null;

            // Setup the scopes
            List<String> scopes = new List<String>();
            scopes.AddRange(ApiResourceSeedData.GetApiResources(seedingType).Select(y => y.Name).ToList());

            // Add in the standard scopes for GetUserInfo
            scopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            scopes.Add(IdentityServerConstants.StandardScopes.Profile);
            scopes.Add(IdentityServerConstants.StandardScopes.Email);

            if (seedingType == SeedingType.IntegrationTest || seedingType == SeedingType.Development || seedingType == SeedingType.Staging)
            {
                client = new Client
                {
                    ClientId = "golfhandicap.subscriptionservice",
                    ClientName = "Subscription Service Client",
                    ClientSecrets = { new Secret("golfhandicap.subscriptionservice".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = scopes
                };
            }

            return client;
        }
        #endregion

        #region private static Client CreateGolfHandicapSubscriptionServiceClient(SeedingType seedingType)        
        /// <summary>
        /// Creates the mobile application client.
        /// </summary>
        /// <param name="seedingType">Type of the seeding.</param>
        /// <returns></returns>
        private static Client CreateGolfHandicapTestDataGeneratorClient(SeedingType seedingType)
        {
            Client client = null;

            // Setup the scopes
            List<String> scopes = new List<String>();
            scopes.AddRange(ApiResourceSeedData.GetApiResources(seedingType).Select(y => y.Name).ToList());

            if (seedingType == SeedingType.IntegrationTest || seedingType == SeedingType.Development || seedingType == SeedingType.Staging)
            {
                client = new Client
                         {
                             ClientId = "golfhandicap.testdatagenerator",
                             ClientName = "Test Data Generator Client",
                             ClientSecrets = { new Secret("golfhandicap.testdatagenerator".Sha256()) },
                             AllowedGrantTypes = GrantTypes.ClientCredentials,
                             AllowedScopes = scopes
                         };
            }

            return client;
        }
        #endregion


        #endregion
    }
}
