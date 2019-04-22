namespace OAuth2SecurityService.Service.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using Manager;
    using Microsoft.AspNetCore.Mvc;

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

        #region Methods

        /// <summary>
        /// Posts the role.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostRole([FromBody] CreateRoleRequest request,
                                                  CancellationToken cancellationToken)
        {
            CreateRoleResponse result = await this.Manager.CreateRole(request, cancellationToken);

            return this.Ok(result);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetRole([FromQuery] String roleName,
                                                 CancellationToken cancellationToken)
        {
            GetRoleResponse result = await this.Manager.GetRoleByName(roleName, cancellationToken);

            return this.Ok(result);
        }

        #endregion
    }
}