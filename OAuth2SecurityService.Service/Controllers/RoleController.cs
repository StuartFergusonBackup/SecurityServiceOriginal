using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuth2SecurityService.DataTransferObjects;
using OAuth2SecurityService.Manager;

namespace OAuth2SecurityService.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// The manager
        /// </summary>
        private readonly ISecurityServiceManager Manager;

        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public RoleController(ISecurityServiceManager manager)
        {
            this.Manager = manager;
        }
        #endregion

        #region Public Methods

        #region public async Task<IActionResult> PostRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
        /// <summary>
        /// Posts the role.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
        {
            var result = await this.Manager.CreateRole(request, cancellationToken);

            return this.Ok(result);
        }
        #endregion

        #endregion
    }
}