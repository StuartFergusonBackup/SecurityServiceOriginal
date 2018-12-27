using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NLog.Web.LayoutRenderers;
using OAuth2SecurityService.DataTransferObjects;
using OAuth2SecurityService.Manager;
using OAuth2SecurityService.Manager.Exceptions;
using OAuth2SecurityService.Manager.Services;
using Shouldly;
using Xunit;

namespace OAuth2SecurityService.UnitTests
{
    public partial class SecurityServiceManagerTests
    {
        [Fact]
        public void SecurityServiceManager_CanBeCreated_IsCreated()
        {
            Mock<IPasswordHasher<IdentityUser>> passwordHasher = new Mock<IPasswordHasher<IdentityUser>>();
            Mock<IUserStore<IdentityUser>> userStore = new Mock<IUserStore<IdentityUser>>();
            UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(userStore.Object, null, null, null, null,null, null, null, null);
            Mock<IMessagingService> messagingService = new Mock<IMessagingService>();
            Mock<IRoleStore<IdentityRole>> roleStore = new Mock<IRoleStore<IdentityRole>>();
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore.Object, null, null, null, null);
            //Mock<Func<IConfigurationDbContext>> configurationDbContext = new Mock<Func<IConfigurationDbContext>>();

            SecurityServiceManager securityServiceManager = new SecurityServiceManager(passwordHasher.Object, userManager, messagingService.Object, roleManager);

            securityServiceManager.ShouldNotBeNull();
        }
        
        #region Register User Tests

        [Theory]
        [InlineData("123456")]
        //[InlineData(null)]
        //[InlineData("")]
        public async Task SecurityServiceManager_RegisterUser_UserIsRegistered(String password)
        {
            TestScenario testScenario = TestScenario.RegisterUserSuccess;
            SecurityServiceManager securityServiceManager = SetupSecurityServiceManager(testScenario);
            
            RegisterUserRequest request = SecurityServiceManagerTestData.GetRegisterUserRequest;
            request.Password = password;

            var response = await securityServiceManager.RegisterUser(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.UserId.ShouldNotBe(Guid.Empty);
        }
        
        [Theory]
        [InlineData(true, "emailaddress", false, false, typeof(ArgumentNullException))]
        [InlineData(false, null, false, false, typeof(ArgumentNullException))]
        [InlineData(false, "", false, false, typeof(ArgumentNullException))]
        [InlineData(false, "", true, false, typeof(ArgumentNullException))]
        [InlineData(false, "", false, true, typeof(ArgumentNullException))]
        public void SecurityServiceManager_RegisterUser_InvalidRequest_ErrorThrown(Boolean nullRequest, String emailAddress, Boolean nullClaims, Boolean nullRoles, Type exceptionType)
        {
            TestScenario testScenario = TestScenario.RegisterUserInvalidData;
            SecurityServiceManager securityServiceManager = SetupSecurityServiceManager(testScenario);
           
            RegisterUserRequest request = null;
            if (!nullRequest)
            {
                request = new RegisterUserRequest
                {
                    Claims = nullClaims ? null : SecurityServiceManagerTestData.Claims,
                    EmailAddress = emailAddress,
                    Password = SecurityServiceManagerTestData.Password,
                    PhoneNumber = SecurityServiceManagerTestData.PhoneNumber,
                    Roles = nullRoles ? null :SecurityServiceManagerTestData.Roles,
                };
            }

            Should.Throw(async () =>
            {                
                await securityServiceManager.RegisterUser(request, CancellationToken.None);
            },exceptionType);
        }

        [Theory]
        [InlineData(TestScenario.RegisterUserPasswordGenerateFailed, typeof(NullReferenceException))]
        [InlineData(TestScenario.RegisterUserCreateUserFailed, typeof(IdentityResultException))]
        [InlineData(TestScenario.RegisterUserAddRolesFailed, typeof(IdentityResultException))]
        [InlineData(TestScenario.RegisterUserAddRolesFailedDeleteFailed, typeof(IdentityResultException))]
        [InlineData(TestScenario.RegisterUserAddClaimsFailed, typeof(IdentityResultException))]
        [InlineData(TestScenario.RegisterUserAddClaimsFailedDeleteFailed, typeof(IdentityResultException))]
        public void SecurityServiceManager_RegisterUser_Failed_ErrorThrown(TestScenario testScenario, Type exceptionType)
        {
            SecurityServiceManager securityServiceManager = SetupSecurityServiceManager(testScenario);
            
            RegisterUserRequest request = SecurityServiceManagerTestData.GetRegisterUserRequest;

            Should.Throw(async () =>
            {                
                await securityServiceManager.RegisterUser(request, CancellationToken.None);
            },exceptionType);
        }

        #endregion

        #region Create Role Tests

        [Fact]
        public async Task SecurityServiceManager_CreateRole_RoleIsCreated()
        {
            TestScenario testScenario = TestScenario.CreateRoleSuccess;
            SecurityServiceManager securityServiceManager = SetupSecurityServiceManager(testScenario);
            
            CreateRoleRequest request = SecurityServiceManagerTestData.GetCreateRoleRequest;

            var response = await securityServiceManager.CreateRole(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.RoleId.ShouldNotBe(Guid.Empty);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SecurityServiceManager_CreateRole_InvalidData_ErrorThrown(String roleName)
        {
            TestScenario testScenario = TestScenario.CreateRoleInvalidData;
            SecurityServiceManager securityServiceManager = SetupSecurityServiceManager(testScenario);
            
            CreateRoleRequest request = SecurityServiceManagerTestData.GetCreateRoleRequest;
            request.RoleName = roleName;

            Should.Throw<ArgumentNullException>(async () =>
            {
                await securityServiceManager.CreateRole(request, CancellationToken.None);
            });
        }

        [Fact]
        public async Task SecurityServiceManager_CreateRole_DuplicateRole_ErrorThrown()
        {
            TestScenario testScenario = TestScenario.CreateRoleDuplicateRoleName;
            SecurityServiceManager securityServiceManager = SetupSecurityServiceManager(testScenario);
            
            CreateRoleRequest request = SecurityServiceManagerTestData.GetCreateRoleRequest;

            Should.Throw<IdentityResultException>(async () =>
            {
                await securityServiceManager.CreateRole(request, CancellationToken.None);
            });
        }

        [Fact]
        public void SecurityServiceManager_CreateRole_CreateRoleFailed_ErrorThrown()
        {
            TestScenario testScenario = TestScenario.CreateRoleCreateRoleFailed;
            SecurityServiceManager securityServiceManager = SetupSecurityServiceManager(testScenario);
            
            CreateRoleRequest request = SecurityServiceManagerTestData.GetCreateRoleRequest;

            Should.Throw<IdentityResultException>(async () =>
            {
                await securityServiceManager.CreateRole(request, CancellationToken.None);
            });
        }

        #endregion
    }
}
