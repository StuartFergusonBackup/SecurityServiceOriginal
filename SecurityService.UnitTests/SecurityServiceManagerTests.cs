namespace SecurityService.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using Manager;
    using Manager.Exceptions;
    using Manager.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using Shared.EventStore;
    using Shouldly;
    using Xunit;

    public partial class SecurityServiceManagerTests
    {
        #region Methods

        [Fact]
        public void SecurityServiceManager_CanBeCreated_IsCreated()
        {
            Mock<IPasswordHasher<IdentityUser>> passwordHasher = new Mock<IPasswordHasher<IdentityUser>>();
            Mock<IUserStore<IdentityUser>> userStore = new Mock<IUserStore<IdentityUser>>();
            UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(userStore.Object, null, null, null, null, null, null, null, null);
            Mock<IMessagingService> messagingService = new Mock<IMessagingService>();
            Mock<IRoleStore<IdentityRole>> roleStore = new Mock<IRoleStore<IdentityRole>>();
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore.Object, null, null, null, null);
            Mock<IHttpContextAccessor> contextAccessor = new Mock<IHttpContextAccessor>();
            Mock<IUserClaimsPrincipalFactory<IdentityUser>> claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            //Mock<Func<IConfigurationDbContext>> configurationDbContext = new Mock<Func<IConfigurationDbContext>>();
            SignInManager<IdentityUser> signInManager = new SignInManager<IdentityUser>(userManager, contextAccessor.Object, claimsFactory.Object, null, null, null);

            SecurityServiceManager securityServiceManager = new SecurityServiceManager(passwordHasher.Object, userManager, messagingService.Object, roleManager, signInManager);

            securityServiceManager.ShouldNotBeNull();
        }

        [Fact]
        public void SecurityServiceManager_CreateRole_CreateRoleFailed_ErrorThrown()
        {
            TestScenario testScenario = TestScenario.CreateRoleCreateRoleFailed;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            CreateRoleRequest request = SecurityServiceManagerTestData.GetCreateRoleRequest;

            Should.Throw<IdentityResultException>(async () => { await securityServiceManager.CreateRole(request, CancellationToken.None); });
        }

        [Fact]
        public async Task SecurityServiceManager_CreateRole_DuplicateRole_ErrorThrown()
        {
            TestScenario testScenario = TestScenario.CreateRoleDuplicateRoleName;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            CreateRoleRequest request = SecurityServiceManagerTestData.GetCreateRoleRequest;

            Should.Throw<IdentityResultException>(async () => { await securityServiceManager.CreateRole(request, CancellationToken.None); });
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SecurityServiceManager_CreateRole_InvalidData_ErrorThrown(String roleName)
        {
            TestScenario testScenario = TestScenario.CreateRoleInvalidData;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            CreateRoleRequest request = SecurityServiceManagerTestData.GetCreateRoleRequest;
            request.RoleName = roleName;

            Should.Throw<ArgumentNullException>(async () => { await securityServiceManager.CreateRole(request, CancellationToken.None); });
        }

        [Fact]
        public async Task SecurityServiceManager_CreateRole_RoleIsCreated()
        {
            TestScenario testScenario = TestScenario.CreateRoleSuccess;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            CreateRoleRequest request = SecurityServiceManagerTestData.GetCreateRoleRequest;

            var response = await securityServiceManager.CreateRole(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.RoleId.ShouldNotBe(Guid.Empty);
        }

        [Theory]
        [InlineData(TestScenario.RegisterUserPasswordGenerateFailed, typeof(NullReferenceException))]
        [InlineData(TestScenario.RegisterUserCreateUserFailed, typeof(IdentityResultException))]
        [InlineData(TestScenario.RegisterUserAddRolesFailed, typeof(IdentityResultException))]
        [InlineData(TestScenario.RegisterUserAddRolesFailedDeleteFailed, typeof(IdentityResultException))]
        [InlineData(TestScenario.RegisterUserAddClaimsFailed, typeof(IdentityResultException))]
        [InlineData(TestScenario.RegisterUserAddClaimsFailedDeleteFailed, typeof(IdentityResultException))]
        public void SecurityServiceManager_RegisterUser_Failed_ErrorThrown(TestScenario testScenario,
                                                                           Type exceptionType)
        {
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            RegisterUserRequest request = SecurityServiceManagerTestData.GetRegisterUserRequest;

            Should.Throw(async () => { await securityServiceManager.RegisterUser(request, CancellationToken.None); }, exceptionType);
        }

        [Theory]
        [InlineData(true, "givenname", "familyname", "emailaddress", false, false, typeof(ArgumentNullException))]
        [InlineData(true, null, "familyname", "emailaddress", false, false, typeof(ArgumentNullException))]
        [InlineData(true, "", "familyname", "emailaddress", false, false, typeof(ArgumentNullException))]
        [InlineData(true, "givenname", null, "emailaddress", false, false, typeof(ArgumentNullException))]
        [InlineData(true, "givenname", "", "emailaddress", false, false, typeof(ArgumentNullException))]
        [InlineData(false, "givenname", "familyname", null, false, false, typeof(ArgumentNullException))]
        [InlineData(false, "givenname", "familyname", "", false, false, typeof(ArgumentNullException))]
        [InlineData(false, "givenname", "familyname", "", true, false, typeof(ArgumentNullException))]
        [InlineData(false, "givenname", "familyname", "", false, true, typeof(ArgumentNullException))]
        public void SecurityServiceManager_RegisterUser_InvalidRequest_ErrorThrown(Boolean nullRequest,
                                                                                   String givenName,
                                                                                   String familyName,
                                                                                   String emailAddress,
                                                                                   Boolean nullClaims,
                                                                                   Boolean nullRoles,
                                                                                   Type exceptionType)
        {
            TestScenario testScenario = TestScenario.RegisterUserInvalidData;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            RegisterUserRequest request = null;
            if (!nullRequest)
            {
                request = new RegisterUserRequest
                          {
                              Claims = nullClaims ? null : SecurityServiceManagerTestData.Claims,
                              GivenName = givenName,
                              FamilyName = familyName,
                              EmailAddress = emailAddress,
                              Password = SecurityServiceManagerTestData.Password,
                              PhoneNumber = SecurityServiceManagerTestData.PhoneNumber,
                              Roles = nullRoles ? null : SecurityServiceManagerTestData.Roles
                          };
            }

            Should.Throw(async () => { await securityServiceManager.RegisterUser(request, CancellationToken.None); }, exceptionType);
        }

        [Theory]
        [InlineData("123456")]
        //[InlineData(null)]
        //[InlineData("")]
        public async Task SecurityServiceManager_RegisterUser_UserIsRegistered(String password)
        {
            TestScenario testScenario = TestScenario.RegisterUserSuccess;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            RegisterUserRequest request = SecurityServiceManagerTestData.GetRegisterUserRequest;
            request.Password = password;

            RegisterUserResponse response = await securityServiceManager.RegisterUser(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.UserId.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public async Task SecurityServiceManager_GetRoleByName_RoleDataReturned()
        {
            TestScenario testScenario = TestScenario.GetRoleSuccess;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            var response = await securityServiceManager.GetRoleByName("testrole", CancellationToken.None);

            response.ShouldNotBeNull();
            response.Id.ShouldNotBe(Guid.Empty);
            response.Name.ShouldBe("testrole");
            response.NormalizedName.ShouldBe("testrole".ToUpper());
        }

        [Fact]
        public async Task SecurityServiceManager_GetRoleByName_RoleNotFound_ErrorThrown()
        {
            TestScenario testScenario = TestScenario.GetRoleRoleNotFound;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            Should.Throw<NotFoundException>(async () =>
                                            {
                                                await securityServiceManager.GetRoleByName("testrole", CancellationToken.None);
                                            });
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SecurityServiceManager_GetRoleByName_InvalidData_ErrorThrown(String roleName)
        {
            TestScenario testScenario = TestScenario.CreateRoleInvalidData;
            SecurityServiceManager securityServiceManager = this.SetupSecurityServiceManager(testScenario);

            Should.Throw<ArgumentNullException>(async () => { await securityServiceManager.GetRoleByName(roleName, CancellationToken.None); });
        }

        #endregion
    }
}