namespace OAuth2SecurityService.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// 
    /// </summary>
    public interface ISecurityServiceManager
    {
        #region Methods

        /// <summary>
        /// Automatics the provision user.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="providerUserId">The provider user identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        Task<IdentityUser> AutoProvisionUser(String provider,
                                             String providerUserId,
                                             IEnumerable<Claim> claims);

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
        /// Gets the user by external provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="providerUserId">The provider user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IdentityUser> GetUserByExternalProvider(String providerName,
                                                     String providerUserId,
                                                     CancellationToken cancellationToken);

        /// <summary>
        /// Gets the name of the user by user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IdentityUser> GetUserByUserName(String userName,
                                             CancellationToken cancellationToken);

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request,
                                                CancellationToken cancellationToken);

        /// <summary>
        /// Signouts this instance.
        /// </summary>
        /// <returns></returns>
        Task Signout();

        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Boolean> ValidateCredentials(String userName,
                                          String password,
                                          CancellationToken cancellationToken);

        #endregion
    }
}