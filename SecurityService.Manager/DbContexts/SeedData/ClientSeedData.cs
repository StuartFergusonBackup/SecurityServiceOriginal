namespace SecurityService.Manager.DbContexts.SeedData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IdentityServer4;
    using IdentityServer4.Models;

    public class ClientSeedData
    {
        #region Methods

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
            clients.Add(ClientSeedData.CreateGolfHandicapAdminWebsiteClient(seedingType));

            return clients;
        }

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
                             ClientSecrets =
                             {
                                 new Secret("developerClient".Sha256())
                             },
                             AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                             AllowedScopes = scopes
                         };
            }

            return client;
        }

        private static Client CreateGolfHandicapAdminWebsiteClient(SeedingType seedingType)
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
                             ClientId = "golfhandicap.adminwebsite",
                             ClientName = "Golf Club Admin Website Client",
                             ClientSecrets =
                             {
                                 new Secret("golfhandicap.adminwebsite".Sha256())
                             },
                             AllowedGrantTypes = GrantTypes.Hybrid,
                             AllowedScopes = scopes,
                             AllowOfflineAccess = true,
                             RedirectUris = ClientSeedData.SetRedirectUris(seedingType),
                             PostLogoutRedirectUris = ClientSeedData.SetPostLogoutRedirectUris(seedingType),
                             RequireConsent = false,                            
                         };
            }
            
            return client;
        }

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
                             ClientSecrets =
                             {
                                 new Secret("golfhandicap.mobile".Sha256())
                             },
                             AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                             AllowedScopes = scopes
                         };
            }

            return client;
        }

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
                             ClientSecrets =
                             {
                                 new Secret("golfhandicap.subscriptionservice".Sha256())
                             },
                             AllowedGrantTypes = GrantTypes.ClientCredentials,
                             AllowedScopes = scopes
                         };
            }

            return client;
        }

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
                             ClientSecrets =
                             {
                                 new Secret("golfhandicap.testdatagenerator".Sha256())
                             },
                             AllowedGrantTypes = GrantTypes.ClientCredentials,
                             AllowedScopes = scopes
                         };
            }

            return client;
        }

        /// <summary>
        /// Sets the post logout redirect uris.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        private static List<String> SetPostLogoutRedirectUris(SeedingType seedingType)
        {
            List<String> redirectUriList = new List<String>();
            switch(seedingType)
            {
                case SeedingType.Development:
                    redirectUriList.Add("http://localhost:5005/signout-callback-oidc");
                    redirectUriList.Add("http://3.9.26.155:5005/signout-callback-oidc");
                    break;
                case SeedingType.Staging:
                    break;
                case SeedingType.Production:
                    break;
            }

            return redirectUriList;
        }

        /// <summary>
        /// Sets the redirect uris.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="seedingType">Type of the seeding.</param>
        private static List<String> SetRedirectUris(SeedingType seedingType)
        {
            List<String> redirectUriList = new List<String>();

            switch (seedingType)
            {
                case SeedingType.Development:
                    redirectUriList.Add("http://localhost:5005/signin-oidc");
                    redirectUriList.Add("http://3.9.26.155:5005/signin-oidc");
                    break;
                case SeedingType.Staging:
                    break;
                case SeedingType.Production:
                    break;
            }

            return redirectUriList;
        }

        #endregion
    }
}