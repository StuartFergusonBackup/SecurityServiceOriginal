// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace OAuth2SecurityService.Service.Controllers.Grants
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Attributes;
    using IdentityServer4.Events;
    using IdentityServer4.Extensions;
    using IdentityServer4.Quickstart.UI;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// This sample controller allows a user to revoke grants given to clientStore
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class GrantsController : Controller
    {
        private readonly IIdentityServerInteractionService IdentityServerInteractionService;
        private readonly IClientStore ClientStore;
        private readonly IResourceStore ResourceStore;
        private readonly IEventService EventService;

        public GrantsController(IIdentityServerInteractionService identityServerInteractionService,
            IClientStore clientStore,
            IResourceStore resourceStore,
            IEventService eventService)
        {
            this.IdentityServerInteractionService = identityServerInteractionService;
            this.ClientStore = clientStore;
            this.ResourceStore = resourceStore;
            this.EventService = eventService;
        }

        /// <summary>
        /// Show list of grants
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return this.View("Index", await this.BuildViewModelAsync());
        }

        /// <summary>
        /// Handle postback to revoke a client
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(string clientId)
        {
            await this.IdentityServerInteractionService.RevokeUserConsentAsync(clientId);
            await this.EventService.RaiseAsync(new GrantsRevokedEvent(this.User.GetSubjectId(), clientId));

            return this.RedirectToAction("Index");
        }

        private async Task<GrantsViewModel> BuildViewModelAsync()
        {
            var grants = await this.IdentityServerInteractionService.GetAllUserConsentsAsync();

            var list = new List<GrantViewModel>();
            foreach(var grant in grants)
            {
                var client = await this.ClientStore.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    var resources = await this.ResourceStore.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantViewModel()
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resources.ApiResources.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            return new GrantsViewModel
            {
                Grants = list
            };
        }
    }
}