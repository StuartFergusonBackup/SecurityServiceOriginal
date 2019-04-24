namespace OAuth2SecurityService.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Primitives;

    public interface ISecurityServiceManager
    {
        #region Methods

        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<CreateRoleResponse> CreateRole(CreateRoleRequest request,
                                            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the name of the role by.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<GetRoleResponse> GetRoleByName(String roleName,
                                            CancellationToken cancellationToken);

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request,
                                                CancellationToken cancellationToken);

        Task<Boolean> ValidateCredentials(String userName,
                                          String password, CancellationToken cancellationToken);

        Task<IdentityUser> GetUserByUserName(String userName,
                                             CancellationToken cancellationToken);

        Task<IdentityUser> GetUserByExternalProvider(String providerName, String providerUserId,
                                             CancellationToken cancellationToken);

        Task<IdentityUser> AutoProvisionUser(String provider, String providerUserId, IEnumerable<Claim> claims);

        #endregion
    }
}