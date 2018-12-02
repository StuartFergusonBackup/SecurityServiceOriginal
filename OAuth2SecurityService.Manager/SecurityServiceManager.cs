using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OAuth2SecurityService.DataTransferObjects;
using OAuth2SecurityService.Manager.Exceptions;
using Shared.General;

namespace OAuth2SecurityService.Manager
{
    public class SecurityServiceManager : ISecurityServiceManager
    {
        #region Fields

        /// <summary>
        /// The password hasher
        /// </summary>
        private readonly IPasswordHasher<IdentityUser> PasswordHasher;

        /// <summary>
        /// The user manager
        /// </summary>
        private readonly UserManager<IdentityUser> UserManager;

        #endregion

        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityServiceManager"/> class.
        /// </summary>
        /// <param name="passwordHasher">The password hasher.</param>
        /// <param name="userManager">The user manager.</param>
        public SecurityServiceManager(IPasswordHasher<IdentityUser> passwordHasher, UserManager<IdentityUser> userManager)
        {
            this.PasswordHasher = passwordHasher;
            this.UserManager = userManager;
        }
        #endregion

        #region Public Methods

        #region public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken)        
        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Error generating password hash value, hash was null or empty</exception>
        /// <exception cref="IdentityResultException">
        /// Error creating user {newIdentityUser.UserName}
        /// or
        /// Error adding roles [{String.Join(",", request.Roles)}] to user {newIdentityUser.UserName}
        /// or
        /// Error adding claims [{String.Join(",", claims)}] to user {newIdentityUser.UserName}
        /// or
        /// Error deleting user {newIdentityUser.UserName}
        /// </exception>
        public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            // Validate the input request
            this.ValidateRequest(request);

            Guid userId = Guid.NewGuid();

            // request is valid now add the user
            IdentityUser newIdentityUser = new IdentityUser
            {
                Id = userId.ToString(),
                Email = request.EmailAddress,
                EmailConfirmed = true,
                UserName = request.EmailAddress,
                NormalizedEmail = request.EmailAddress.ToLower(),
                NormalizedUserName = request.EmailAddress.ToLower(),
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            // Set the password
            //String password = String.IsNullOrEmpty(request.Password) ? GenerateRandomPassword() : request.Password;

            // Hash the new password
            newIdentityUser.PasswordHash = this.PasswordHasher.HashPassword(newIdentityUser, request.Password);

            if (String.IsNullOrEmpty(newIdentityUser.PasswordHash))
            {
                throw new NullReferenceException("Error generating password hash value, hash was null or empty");
            }

            // Default all IdentityResults to failed
            IdentityResult createResult = IdentityResult.Failed();
            IdentityResult addRolesResult = IdentityResult.Failed();
            IdentityResult addClaimsResult = IdentityResult.Failed();

            try
            {
                // Create the User
                createResult = await this.UserManager.CreateAsync(newIdentityUser);

                if (!createResult.Succeeded)
                {
                    throw new IdentityResultException($"Error creating user {newIdentityUser.UserName}", createResult);
                }

                // Add the requested roles to the user
                addRolesResult = await this.UserManager.AddToRolesAsync(newIdentityUser, request.Roles);

                if (!addRolesResult.Succeeded)
                {
                    throw new IdentityResultException($"Error adding roles [{String.Join(",", request.Roles)}] to user {newIdentityUser.UserName}", addRolesResult);
                }

                // Add the requested claims
                var claims = request.Claims.Select(x => new Claim(x.Key, x.Value)).ToList();
                addClaimsResult = await this.UserManager.AddClaimsAsync(newIdentityUser, claims);
                
                if (!addClaimsResult.Succeeded)
                {
                    List<String> claimList = new List<String>();
                    claims.ForEach(c => claimList.Add($"Name: {c.Type} Value: {c.Value}"));
                    throw new IdentityResultException($"Error adding claims [{String.Join(",", claims)}] to user {newIdentityUser.UserName}", addClaimsResult);
                }
            }
            finally
            {
                // Do some cleanup here (if the create was successful but one fo the other steps failed)
                if ((createResult == IdentityResult.Success) && (!addRolesResult.Succeeded || !addClaimsResult.Succeeded))
                {
                    // User has been created so need to remove this
                    var deleteResult = await this.UserManager.DeleteAsync(newIdentityUser);

                    if (!deleteResult.Succeeded)
                    {
                        throw new IdentityResultException($"Error deleting user {newIdentityUser.UserName} as part of cleanup", deleteResult);
                    }
                }
            }

            // Create the response
            RegisterUserResponse response = new RegisterUserResponse
            {
                UserId = userId
            };

            return response;
        }

        #endregion

        #endregion

        #region Private Methods

        #region private void ValidateRequest(RegisterUserRequest request)        
        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ValidateRequest(RegisterUserRequest request)
        {
            Guard.ThrowIfNull(request, typeof(ArgumentNullException), "RegisterUserRequest cannot be null");
            Guard.ThrowIfNullOrEmpty(request.EmailAddress, typeof(ArgumentNullException), "RegisterUserRequest Email Address cannot be null or empty");
            Guard.ThrowIfNullOrEmpty(request.PhoneNumber, typeof(ArgumentNullException), "RegisterUserRequest Phone Number cannot be null or empty");
            Guard.ThrowIfNull(request.Claims, typeof(ArgumentNullException), "RegisterUserRequest Claims cannot be null or empty");            
            Guard.ThrowIfNull(request.Roles, typeof(ArgumentNullException), "RegisterUserRequest Roles cannot be null or empty");
        }
        #endregion

        #endregion
    }
}