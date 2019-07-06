namespace SecurityService.Service.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using Manager;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// The manager
        /// </summary>
        private readonly ISecurityServiceManager Manager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public UserController(ISecurityServiceManager manager)
        {
            this.Manager = manager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Posts the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] RegisterUserRequest request,
                                                  CancellationToken cancellationToken)
        {
            RegisterUserResponse result = await this.Manager.RegisterUser(request, cancellationToken);

            return this.Ok(result);
        }

        #endregion
    }
}