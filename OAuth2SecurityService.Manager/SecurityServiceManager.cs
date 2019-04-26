namespace OAuth2SecurityService.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using Exceptions;
    using Microsoft.AspNetCore.Identity;
    using Services;
    using Shared.EventStore;
    using Shared.General;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="OAuth2SecurityService.Manager.ISecurityServiceManager" />
    public class SecurityServiceManager : ISecurityServiceManager
    {
        #region Fields

        /// <summary>
        /// The messaging service
        /// </summary>
        private readonly IMessagingService MessagingService;

        /// <summary>
        /// The password hasher
        /// </summary>
        private readonly IPasswordHasher<IdentityUser> PasswordHasher;

        /// <summary>
        /// The role manager
        /// </summary>
        private readonly RoleManager<IdentityRole> RoleManager;

        /// <summary>
        /// The sign in manager
        /// </summary>
        private readonly SignInManager<IdentityUser> SignInManager;

        /// <summary>
        /// The user manager
        /// </summary>
        private readonly UserManager<IdentityUser> UserManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityServiceManager" /> class.
        /// </summary>
        /// <param name="passwordHasher">The password hasher.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="messagingService">The messaging service.</param>
        /// <param name="roleManager">The role manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        public SecurityServiceManager(IPasswordHasher<IdentityUser> passwordHasher,
                                      UserManager<IdentityUser> userManager,
                                      IMessagingService messagingService,
                                      RoleManager<IdentityRole> roleManager,
                                      SignInManager<IdentityUser> signInManager)
        {
            this.PasswordHasher = passwordHasher;
            this.UserManager = userManager;
            this.MessagingService = messagingService;
            this.RoleManager = roleManager;
            this.SignInManager = signInManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Automatics the provision user.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="providerUserId">The provider user identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public async Task<IdentityUser> AutoProvisionUser(String provider,
                                                          String providerUserId,
                                                          IEnumerable<Claim> claims)
        {
            // TODO;
            return null;
        }

        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="IdentityResultException">
        /// Role {newIdentityRole.Name}
        /// or
        /// Error creating role {newIdentityRole.Name}
        /// </exception>
        public async Task<CreateRoleResponse> CreateRole(CreateRoleRequest request,
                                                         CancellationToken cancellationToken)
        {
            // Validate the input request
            this.ValidateRequest(request);

            Guid roleId = Guid.NewGuid();

            IdentityRole newIdentityRole = new IdentityRole
                                           {
                                               Id = roleId.ToString(),
                                               Name = request.RoleName,
                                               NormalizedName = request.RoleName.ToUpper()
                                           };

            // Default all IdentityResults to failed
            IdentityResult createResult = IdentityResult.Failed();

            // Ensure role name is not a duplicate
            if (await this.RoleManager.RoleExistsAsync(newIdentityRole.Name))
            {
                throw new IdentityResultException($"Role {newIdentityRole.Name} already exists", IdentityResult.Failed());
            }

            createResult = await this.RoleManager.CreateAsync(newIdentityRole);

            if (!createResult.Succeeded)
            {
                throw new IdentityResultException($"Error creating role {newIdentityRole.Name}", createResult);
            }

            // Create the response
            CreateRoleResponse response = new CreateRoleResponse
                                          {
                                              RoleId = roleId
                                          };

            return response;
        }

        /// <summary>
        /// Gets the name of the role by.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">No role found with name {roleName}</exception>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<GetRoleResponse> GetRoleByName(String roleName,
                                                         CancellationToken cancellationToken)
        {
            Guard.ThrowIfNullOrEmpty(roleName, typeof(ArgumentNullException), "role name must be provided to get a role by name");

            IdentityRole identityRole = await this.RoleManager.FindByNameAsync(roleName);

            if (identityRole == null)
            {
                throw new NotFoundException($"No role found with name {roleName}");
            }

            // Role has been found
            GetRoleResponse response = new GetRoleResponse
                                       {
                                           Id = Guid.Parse(identityRole.Id),
                                           Name = identityRole.Name,
                                           NormalizedName = identityRole.NormalizedName
                                       };

            return response;
        }

        /// <summary>
        /// Gets the user by external provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="providerUserId">The provider user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IdentityUser> GetUserByExternalProvider(String providerName,
                                                                  String providerUserId,
                                                                  CancellationToken cancellationToken)
        {
            // TODO;
            return null;
        }

        /// <summary>
        /// Gets the name of the user by user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IdentityUser> GetUserByUserName(String userName,
                                                          CancellationToken cancellationToken)
        {
            IdentityUser user = await this.UserManager.FindByNameAsync(userName);

            return user;
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Error generating password hash value, hash was null or empty</exception>
        /// <exception cref="IdentityResultException">Error creating user {newIdentityUser.UserName}
        /// or
        /// Error adding roles [{String.Join(",", request.Roles)}] to user {newIdentityUser.UserName}
        /// or
        /// Error adding claims [{String.Join(",", claims)}] to user {newIdentityUser.UserName}
        /// or
        /// Error deleting user {newIdentityUser.UserName}</exception>
        /// <exception cref="System.NullReferenceException">Error generating password hash value, hash was null or empty</exception>
        public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request,
                                                             CancellationToken cancellationToken)
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
                                               NormalizedEmail = request.EmailAddress.ToUpper(),
                                               NormalizedUserName = request.EmailAddress.ToUpper(),
                                               SecurityStamp = Guid.NewGuid().ToString("D")
                                           };

            // Set the password
            //String password = String.IsNullOrEmpty(request.Password) ? GenerateRandomPassword() : request.Password;

            // Hash the new password
            newIdentityUser.PasswordHash = this.PasswordHasher.HashPassword(newIdentityUser, request.Password);

            if (string.IsNullOrEmpty(newIdentityUser.PasswordHash))
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
                    throw new IdentityResultException($"Error adding roles [{string.Join(",", request.Roles)}] to user {newIdentityUser.UserName}", addRolesResult);
                }

                // Add the requested claims
                var claims = request.Claims.Select(x => new Claim(x.Key, x.Value)).ToList();
                addClaimsResult = await this.UserManager.AddClaimsAsync(newIdentityUser, claims);

                if (!addClaimsResult.Succeeded)
                {
                    List<String> claimList = new List<String>();
                    claims.ForEach(c => claimList.Add($"Name: {c.Type} Value: {c.Value}"));
                    throw new IdentityResultException($"Error adding claims [{string.Join(",", claims)}] to user {newIdentityUser.UserName}", addClaimsResult);
                }

                // Send the Registration Email
                await this.SendRegistrationEmail(newIdentityUser.Email, request.Password, cancellationToken);
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

        /// <summary>
        /// Signouts this instance.
        /// </summary>
        /// <returns></returns>
        public async Task Signout()
        {
            await this.SignInManager.SignOutAsync();
        }

        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<Boolean> ValidateCredentials(String userName,
                                                       String password,
                                                       CancellationToken cancellationToken)
        {
            // Get the user record by name
            IdentityUser user = await this.UserManager.FindByNameAsync(userName);

            // Now validate the entered password
            PasswordVerificationResult verificationResult = this.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            // Return the result
            return verificationResult == PasswordVerificationResult.Success;
        }

        /// <summary>
        /// Sends the registration email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="password">The password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task SendRegistrationEmail(String emailAddress,
                                                 String password,
                                                 CancellationToken cancellationToken)
        {
            StringBuilder emailBuilder = new StringBuilder();
            emailBuilder.AppendLine("<html><body>");
            emailBuilder.AppendLine("<p>Welcome to mygolfhandicapping.co.uk</p>");
            emailBuilder.AppendLine("<p></p>");
            emailBuilder.AppendLine("<p>Please find below your user details:</p>");
            emailBuilder.AppendLine("<table>");
            emailBuilder.AppendLine("<tr><td><strong>User Name</strong></td></tr>");
            emailBuilder.AppendLine($"<tr><td>{emailAddress}</td></tr>");
            emailBuilder.AppendLine("<tr><td><strong>Password</strong></td></tr>");
            emailBuilder.AppendLine($"<tr><td>{password}</td></tr>");
            emailBuilder.AppendLine("</table>");
            emailBuilder.AppendLine("</body></html>");

            SendEmailRequest sendEmailRequest = new SendEmailRequest
                                                {
                                                    Body = emailBuilder.ToString(),
                                                    FromAddress = "golfhandicapping@btinternet.com",
                                                    IsHtml = true,
                                                    Subject = "mygolfhandicapping.co.uk Registration Completed",
                                                    ToAddresses = new List<String>
                                                                  {
                                                                      emailAddress
                                                                  }
                                                };

            SendEmailResponse response = await this.MessagingService.SendEmail(sendEmailRequest, cancellationToken);

            // TODO: Handle a failure case here
        }

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

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ValidateRequest(CreateRoleRequest request)
        {
            Guard.ThrowIfNull(request, typeof(ArgumentNullException), "RegisterUserRequest cannot be null");
            Guard.ThrowIfNullOrEmpty(request.RoleName, typeof(ArgumentNullException), "RegisterUserRequest Role Name cannot be null or empty");
        }

        #endregion
    }
}