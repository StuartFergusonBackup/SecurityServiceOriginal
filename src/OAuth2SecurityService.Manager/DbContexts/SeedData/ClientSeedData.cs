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
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = scopes
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

        #endregion
    }
}
