// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SecurityService.Service.Controllers.Diagnostics
{
    using System.Linq;
    using System.Threading.Tasks;
    using Attributes;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Diagnostics;

    [SecurityHeaders]
    [Authorize]
    public class DiagnosticsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var localAddresses = new string[] { "127.0.0.1", "::1", this.HttpContext.Connection.LocalIpAddress.ToString() };
            if (!localAddresses.Contains(this.HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                return this.NotFound();
            }

            var model = new DiagnosticsViewModel(await this.HttpContext.AuthenticateAsync());
            return this.View(model);
        }
    }
}