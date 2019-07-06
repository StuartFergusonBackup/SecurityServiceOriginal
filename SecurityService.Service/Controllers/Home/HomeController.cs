// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SecurityService.Service.Controllers.Home
{
    using System.Threading.Tasks;
    using Attributes;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.Home;

    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService IdentityServerInteractionService;
        private readonly IHostingEnvironment HostingEnvironment;
        private readonly ILogger Logger;

        public HomeController(IIdentityServerInteractionService identityServerInteractionService, IHostingEnvironment hostingEnvironment, ILogger<HomeController> logger)
        {
            this.IdentityServerInteractionService = identityServerInteractionService;
            this.HostingEnvironment = hostingEnvironment;
            this.Logger = logger;
        }

        public IActionResult Index()
        {
            if (this.HostingEnvironment.IsDevelopment())
            {
                // only show in development
                return this.View();
            }

            this.Logger.LogInformation("Homepage is disabled in production. Returning 404.");
            return this.NotFound();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await this.IdentityServerInteractionService.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!this.HostingEnvironment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return this.View("Error", vm);
        }
    }
}