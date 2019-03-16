using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using OAuth2SecurityService.Manager;
using OAuth2SecurityService.Manager.Services;

namespace OAuth2SecurityService.UnitTests
{
    public partial class SecurityServiceManagerTests
    {
        private Mock<IPasswordHasher<IdentityUser>> PasswordHasher = new Mock<IPasswordHasher<IdentityUser>>();
        private Mock<IUserStore<IdentityUser>> UserStore = new Mock<IUserStore<IdentityUser>>();

        private List<IPasswordValidator<IdentityUser>>
            PasswordValidators = new List<IPasswordValidator<IdentityUser>>();

        private List<IUserValidator<IdentityUser>> UserValidators = new List<IUserValidator<IdentityUser>>();
        private Mock<IRoleStore<IdentityRole>> RoleStore = new Mock<IRoleStore<IdentityRole>>();
        private Mock<IdentityErrorDescriber> ErrorDescriber = new Mock<IdentityErrorDescriber>();
        private Mock<IServiceProvider> ServiceProvider = new Mock<IServiceProvider>();

        public enum TestScenario
        {
            RegisterUserSuccess,
            RegisterUserInvalidData,
            RegisterUserPasswordGenerateFailed,
            RegisterUserCreateUserFailed,
            RegisterUserAddRolesFailed,
            RegisterUserAddRolesFailedDeleteFailed,
            RegisterUserAddClaimsFailed,
            RegisterUserAddClaimsFailedDeleteFailed,
            GetUserByUserIdSuccess,
            GetUserByUserNameSuccess,
            GetUserByEmailAddressSuccess,
            GetUserByUserIdInvalidId,
            GetUserByUserNameInvalidUserName,
            GetUserByEmailAddressInvalidEmailAddress,
            ChangePasswordSuccess,
            ChangePasswordInvalidData,
            ChangePasswordUserNotFound,
            ChangePasswordOldPasswordIncorrect,
            ForgotPasswordSuccess,
            ForgotPasswordInvalidData,
            ForgotPasswordInvalidEmailAddress,
            ResetPasswordSuccess,
            ResetPasswordInvalidData,
            ResetPasswordInvalidEmailAddress,
            ResetPasswordInvalidPasswordResetCode,
            CreateRoleSuccess,
            CreateRoleInvalidData,
            CreateRoleDuplicateRoleName,
            CreateRoleCreateRoleFailed,
            GetRoleSuccess,
            GetRoleRoleNotFound,
            GetRoleInvalidData,
        }

        private void SetupPasswordHasher(TestScenario testScenario)
        {
            if (testScenario == TestScenario.RegisterUserSuccess)
            {
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns("HashedPassword");
            }

            if (testScenario == TestScenario.RegisterUserPasswordGenerateFailed)
            {
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns(String.Empty);
            }

            if (testScenario == TestScenario.RegisterUserCreateUserFailed)
            {
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns("HashedPassword");
            }

            if (testScenario == TestScenario.RegisterUserAddRolesFailed)
            {
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns("HashedPassword");
            }

            if (testScenario == TestScenario.RegisterUserAddRolesFailedDeleteFailed)
            {
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns("HashedPassword");
            }

            if (testScenario == TestScenario.RegisterUserAddClaimsFailed)
            {
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns("HashedPassword");
            }

            if (testScenario == TestScenario.RegisterUserAddClaimsFailedDeleteFailed)
            {
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns("HashedPassword");
            }

            if (testScenario == TestScenario.ChangePasswordSuccess)
            {
                this.PasswordHasher
                    .Setup(ph =>
                        ph.VerifyHashedPassword(It.IsAny<IdentityUser>(), It.IsAny<String>(), It.IsAny<String>()))
                    .Returns(PasswordVerificationResult.Success);
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns("HashedPassword");
            }

            if (testScenario == TestScenario.ResetPasswordSuccess)
            {
                this.PasswordHasher
                    .Setup(ph =>
                        ph.VerifyHashedPassword(It.IsAny<IdentityUser>(), It.IsAny<String>(), It.IsAny<String>()))
                    .Returns(PasswordVerificationResult.Success);
                this.PasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<String>()))
                    .Returns("HashedPassword");
            }

            if (testScenario == TestScenario.ChangePasswordOldPasswordIncorrect)
            {
                this.PasswordHasher
                    .Setup(ph =>
                        ph.VerifyHashedPassword(It.IsAny<IdentityUser>(), It.IsAny<String>(), It.IsAny<String>()))
                    .Returns(PasswordVerificationResult.Failed);
            }
        }

        private void SetupUserStore(TestScenario testScenario)
        {
            if (testScenario == TestScenario.RegisterUserSuccess)
            {
                this.UserStore.Setup(us => us.CreateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);

                this.UserStore.Setup(us => us.UpdateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
            }

            if (testScenario == TestScenario.RegisterUserCreateUserFailed)
            {
                this.UserStore.Setup(us => us.CreateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Failed());
            }

            if (testScenario == TestScenario.RegisterUserAddRolesFailed)
            {
                this.UserStore.Setup(us => us.CreateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
                this.UserStore.Setup(us => us.UpdateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Failed());
                this.UserStore.Setup(us => us.DeleteAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
            }

            if (testScenario == TestScenario.RegisterUserAddRolesFailedDeleteFailed)
            {
                this.UserStore.Setup(us => us.CreateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
                this.UserStore.Setup(us => us.UpdateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Failed());
                this.UserStore.Setup(us => us.DeleteAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Failed());
            }

            if (testScenario == TestScenario.RegisterUserAddClaimsFailed)
            {
                this.UserStore.Setup(us => us.CreateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
                this.UserStore.SetupSequence(us => us.UpdateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success)
                    .ReturnsAsync(IdentityResult.Failed());
                this.UserStore.Setup(us => us.DeleteAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
            }

            if (testScenario == TestScenario.RegisterUserAddClaimsFailedDeleteFailed)
            {
                this.UserStore.Setup(us => us.CreateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
                this.UserStore.SetupSequence(us => us.UpdateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success)
                    .ReturnsAsync(IdentityResult.Failed());
                this.UserStore.Setup(us => us.DeleteAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Failed());
            }

            if (testScenario == TestScenario.ChangePasswordSuccess || testScenario == TestScenario.ResetPasswordSuccess)
            {
                this.UserStore.Setup(us => us.FindByIdAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(new IdentityUser("testuser"));
                this.UserStore.Setup(us => us.UpdateAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
                this.UserStore.Setup(us =>
                        us.SetNormalizedUserNameAsync(It.IsAny<IdentityUser>(), It.IsAny<String>(),
                            CancellationToken.None))
                    .Returns(Task.CompletedTask);
            }

            if (testScenario == TestScenario.ChangePasswordUserNotFound)
            {
                IdentityUser nullUser = null;
                this.UserStore.Setup(us => us.FindByIdAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(nullUser);
            }

            if (testScenario == TestScenario.ChangePasswordOldPasswordIncorrect)
            {
                this.UserStore.Setup(us => us.FindByIdAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(new IdentityUser("testuser"));
                this.UserStore.Setup(us => us.GetUserIdAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync("userid");
            }
        }

        private void SetupUserPasswordStore(TestScenario testScenario)
        {
            if (testScenario == TestScenario.ChangePasswordSuccess || testScenario == TestScenario.ResetPasswordSuccess)
            {
                this.UserStore.As<IUserPasswordStore<IdentityUser>>()
                    .Setup(us => us.GetPasswordHashAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync("TestHash");
                this.UserStore.As<IUserPasswordStore<IdentityUser>>()
                    .Setup(us =>
                        us.SetPasswordHashAsync(It.IsAny<IdentityUser>(), It.IsAny<String>(), CancellationToken.None))
                    .Returns(Task.CompletedTask);
            }

            if (testScenario == TestScenario.ChangePasswordOldPasswordIncorrect)
            {
                this.UserStore.As<IUserPasswordStore<IdentityUser>>()
                    .Setup(us => us.GetPasswordHashAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync("TestHash");
            }
        }

        private void SetupUserSecurityStampStore(TestScenario testScenario)
        {
            if (testScenario == TestScenario.ChangePasswordSuccess || testScenario == TestScenario.ResetPasswordSuccess)
            {
                this.UserStore.As<IUserSecurityStampStore<IdentityUser>>().Setup(us => us.SetSecurityStampAsync(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<String>(), CancellationToken.None)).Returns(Task.CompletedTask);
                this.UserStore.As<IUserSecurityStampStore<IdentityUser>>()
                    .Setup(us => us.GetSecurityStampAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                    .ReturnsAsync("SecurityStamp");
            }
        }

        private void SetupUserEmailStore(TestScenario testScenario)
        {
            this.UserStore.As<IUserEmailStore<IdentityUser>>().Setup(us =>
                    us.SetNormalizedEmailAsync(It.IsAny<IdentityUser>(), It.IsAny<String>(), CancellationToken.None))
                .Returns(Task.CompletedTask);
            this.UserStore.As<IUserEmailStore<IdentityUser>>()
                .Setup(us => us.GetEmailAsync(It.IsAny<IdentityUser>(), CancellationToken.None))
                .ReturnsAsync("emailaddress");

            if (testScenario == TestScenario.ForgotPasswordSuccess)
            {
                this.UserStore.As<IUserEmailStore<IdentityUser>>()
                    .Setup(us => us.FindByEmailAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(new IdentityUser("testuser"));
            }

            if (testScenario == TestScenario.ResetPasswordSuccess)
            {
                this.UserStore.As<IUserEmailStore<IdentityUser>>()
                    .Setup(us => us.FindByEmailAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(new IdentityUser("testuser"));
            }

            if (testScenario == TestScenario.ForgotPasswordInvalidEmailAddress)
            {
                IdentityUser nullUser = null;
                this.UserStore.As<IUserEmailStore<IdentityUser>>()
                    .Setup(us => us.FindByEmailAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(nullUser);
            }

            if (testScenario == TestScenario.ResetPasswordInvalidEmailAddress)
            {
                IdentityUser nullUser = null;
                this.UserStore.As<IUserEmailStore<IdentityUser>>()
                    .Setup(us => us.FindByEmailAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(nullUser);
            }
        }

        private void SetupPasswordValidatiors(TestScenario testScenario)
        {
            if (testScenario == TestScenario.ChangePasswordSuccess || testScenario == TestScenario.ResetPasswordSuccess)
            {
                Mock<IPasswordValidator<IdentityUser>> passwordValidator = new Mock<IPasswordValidator<IdentityUser>>();

                passwordValidator
                    .Setup(pv => pv.ValidateAsync(It.IsAny<UserManager<IdentityUser>>(), It.IsAny<IdentityUser>(),
                        It.IsAny<String>())).ReturnsAsync(IdentityResult.Success);

                this.PasswordValidators.Add(passwordValidator.Object);
            }
        }

        private void SetupUserValidators(TestScenario testScenario)
        {
            if (testScenario == TestScenario.ChangePasswordSuccess || testScenario == TestScenario.ResetPasswordSuccess)
            {
                Mock<IUserValidator<IdentityUser>> userValidator = new Mock<IUserValidator<IdentityUser>>();

                userValidator.Setup(uv =>
                        uv.ValidateAsync(It.IsAny<UserManager<IdentityUser>>(), It.IsAny<IdentityUser>()))
                    .ReturnsAsync(IdentityResult.Success);

                this.UserValidators.Add(userValidator.Object);
            }
        }

        private void SetupUserRoleStore(TestScenario testScenario)
        {
            if (testScenario == TestScenario.RegisterUserSuccess)
            {
                this.UserStore.As<IUserRoleStore<IdentityUser>>()
                    .Setup(us => us.IsInRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(false);

                this.UserStore.As<IUserRoleStore<IdentityUser>>()
                    .Setup(us =>
                        us.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<String>(), CancellationToken.None))
                    .Returns(Task.CompletedTask);
            }

            if (testScenario == TestScenario.RegisterUserAddRolesFailed)
            {
                this.UserStore.As<IUserRoleStore<IdentityUser>>()
                    .Setup(us =>
                        us.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<String>(), CancellationToken.None))
                    .Returns(Task.CompletedTask);
            }

            if (testScenario == TestScenario.RegisterUserAddClaimsFailed || 
                testScenario == TestScenario.RegisterUserAddClaimsFailedDeleteFailed)
            {
                this.UserStore.As<IUserRoleStore<IdentityUser>>()
                    .Setup(us =>
                        us.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<String>(), CancellationToken.None))
                    .Returns(Task.CompletedTask);
            }

            if (testScenario == TestScenario.GetUserByUserIdSuccess ||
                testScenario == TestScenario.GetUserByUserNameSuccess ||
                testScenario == TestScenario.GetUserByEmailAddressSuccess)
            {
                this.UserStore.As<IUserRoleStore<IdentityUser>>()
                    .Setup(us => us.GetRolesAsync(It.IsAny<IdentityUser>(), CancellationToken.None)).ReturnsAsync(
                        SecurityServiceManagerTestData.UserRoles
                            .Where(r => r.Key == SecurityServiceManagerTestData.TestDataUserId)
                            .Select(r => r.Value).First());
            }
        }

        private void SetupUserClaimStore(TestScenario testScenario)
        {
            if (testScenario == TestScenario.RegisterUserSuccess)
            {
                this.UserStore.As<IUserClaimStore<IdentityUser>>().Setup(us =>
                        us.AddClaimsAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<Claim>>(),
                            CancellationToken.None))
                    .Returns(Task.CompletedTask);
            }

            if (testScenario == TestScenario.RegisterUserAddClaimsFailed || 
                testScenario == TestScenario.RegisterUserAddClaimsFailedDeleteFailed)
            {
                this.UserStore.As<IUserClaimStore<IdentityUser>>().Setup(us =>
                        us.AddClaimsAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<Claim>>(),
                            CancellationToken.None))
                    .Returns(Task.CompletedTask);
            }

            if (testScenario == TestScenario.GetUserByUserIdSuccess ||
                testScenario == TestScenario.GetUserByUserNameSuccess ||
                testScenario == TestScenario.GetUserByEmailAddressSuccess)
            {
                this.UserStore.As<IUserClaimStore<IdentityUser>>()
                    .Setup(us => us.GetClaimsAsync(It.IsAny<IdentityUser>(), CancellationToken.None)).ReturnsAsync(
                        SecurityServiceManagerTestData.UserClaims
                            .Where(r => r.Key == SecurityServiceManagerTestData.TestDataUserId).Select(r => r.Value)
                            .First());
            }
        }

        private void SetupQueryableUserStore(TestScenario testScenario)
        {
            if (testScenario == TestScenario.GetUserByUserIdSuccess ||
                testScenario == TestScenario.GetUserByUserNameSuccess ||
                testScenario == TestScenario.GetUserByEmailAddressSuccess ||
                testScenario == TestScenario.GetUserByUserIdInvalidId ||
                testScenario == TestScenario.GetUserByUserNameInvalidUserName ||
                testScenario == TestScenario.GetUserByEmailAddressInvalidEmailAddress)
            {
                this.UserStore.As<IQueryableUserStore<IdentityUser>>().Setup(us => us.Users)
                    .Returns(SecurityServiceManagerTestData.UserList.AsQueryable().BuildMock().Object);
            }
        }

        private void SetupRoleStore(TestScenario testScenario)
        {
            if (testScenario == TestScenario.CreateRoleSuccess)
            {
                this.RoleStore.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Success);
            }
            else if (testScenario == TestScenario.CreateRoleCreateRoleFailed)
            {
                this.RoleStore.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>(), CancellationToken.None))
                    .ReturnsAsync(IdentityResult.Failed());
            }
            else if (testScenario == TestScenario.CreateRoleDuplicateRoleName)
            {
                IdentityRole roleFound = new IdentityRole("testrole");
                this.RoleStore.Setup(r => r.FindByNameAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(roleFound);
            }
            else if (testScenario == TestScenario.GetRoleSuccess)
            {
                IdentityRole roleFound = new IdentityRole("testrole");
                roleFound.NormalizedName = "TESTROLE";
                roleFound.Id = "15426DB6-A04E-4FB7-B796-0C8CD76111E0";

                this.RoleStore.Setup(r => r.FindByNameAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(roleFound);
            }
            else if (testScenario == TestScenario.GetRoleRoleNotFound)
            {
                IdentityRole roleNotFound = null;
                this.RoleStore.Setup(r => r.FindByNameAsync(It.IsAny<String>(), CancellationToken.None))
                    .ReturnsAsync(roleNotFound);
            }
        }

        private void SetupErrorDescriber()
        {
            ErrorDescriber.Setup(e => e.PasswordMismatch()).Returns(new IdentityError
            {
                Code = "1",
                Description = "Password Mismatch"
            });
            ErrorDescriber.Setup(e => e.InvalidToken()).Returns(new IdentityError
            {
                Code = "2",
                Description = "Invalid Token"
            });
            ErrorDescriber.Setup(e => e.LoginAlreadyAssociated()).Returns(new IdentityError
            {
                Code = "3",
                Description = "Login Already Associated"
            });
            ErrorDescriber.Setup(e => e.UserAlreadyInRole(It.IsAny<String>())).Returns(new IdentityError
            {
                Code = "4",
                Description = "User Already In Role"
            });
            ErrorDescriber.Setup(e => e.UserNotInRole(It.IsAny<String>())).Returns(new IdentityError
            {
                Code = "5",
                Description = "User Not In Role"
            });
            ErrorDescriber.Setup(e => e.UserLockoutNotEnabled()).Returns(new IdentityError
            {
                Code = "6",
                Description = "User Lockout Not Enabled"
            });
            ErrorDescriber.Setup(e => e.RecoveryCodeRedemptionFailed()).Returns(new IdentityError
            {
                Code = "7",
                Description = "Recovery Code Redemption Failed"
            });
        }

        private void SetupServiceProvider(TestScenario testScenario)
        {
            Mock<IUserTwoFactorTokenProvider<IdentityUser>> tokenProvider =
                new Mock<IUserTwoFactorTokenProvider<IdentityUser>>();
            tokenProvider.Setup(tp => tp.GenerateAsync(It.IsAny<String>(), It.IsAny<UserManager<IdentityUser>>(),
                It.IsAny<IdentityUser>())).ReturnsAsync("token");

            if (testScenario == TestScenario.ResetPasswordInvalidPasswordResetCode)
            {
                tokenProvider.Setup(tp => tp.ValidateAsync(It.IsAny<String>(), It.IsAny<String>(),
                    It.IsAny<UserManager<IdentityUser>>(),
                    It.IsAny<IdentityUser>())).ReturnsAsync(true);
            }
            else
            {
                tokenProvider.Setup(tp => tp.ValidateAsync(It.IsAny<String>(), It.IsAny<String>(),
                    It.IsAny<UserManager<IdentityUser>>(),
                    It.IsAny<IdentityUser>())).ReturnsAsync(true);
            }

            this.ServiceProvider.Setup(sp => sp.GetService(It.IsAny<Type>())).Returns(tokenProvider.Object);
        }

        private SecurityServiceManager SetupSecurityServiceManager(TestScenario testScenario)
        {
            SetupPasswordHasher(testScenario);

            SetupUserStore(testScenario);
            SetupQueryableUserStore(testScenario);
            SetupUserRoleStore(testScenario);
            SetupUserClaimStore(testScenario);
            SetupUserPasswordStore(testScenario);
            SetupUserSecurityStampStore(testScenario);
            SetupUserEmailStore(testScenario);

            SetupPasswordValidatiors(testScenario);
            SetupUserValidators(testScenario);

            SetupServiceProvider(testScenario);

            SetupErrorDescriber();

            SetupRoleStore(testScenario);

            IdentityOptions identityOptions = new IdentityOptions();
            identityOptions.Tokens.ProviderMap.Add("Default",
                new TokenProviderDescriptor(typeof(IUserTwoFactorTokenProvider<IdentityUser>)));
            Mock<IOptions<IdentityOptions>> options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(o => o.Value).Returns(identityOptions);

            UserManager<IdentityUser> userManager =
                new UserManager<IdentityUser>(this.UserStore.Object, options.Object, this.PasswordHasher.Object,
                    this.UserValidators, this.PasswordValidators,
                    null, ErrorDescriber.Object, ServiceProvider.Object, new NullLogger<UserManager<IdentityUser>>());

            RoleManager<IdentityRole> roleManager =
                new RoleManager<IdentityRole>(this.RoleStore.Object, null, null, null, null);

            Mock<Func<IConfigurationDbContext>> configurationDbContextResolver =
                new Mock<Func<IConfigurationDbContext>>();

            //Mock<IOptions<ServiceOptions>> serviceOptions = new Mock<IOptions<ServiceOptions>>();
            //serviceOptions.Setup(so => so.Value).Returns(new ServiceOptions
            //{
            //    PublicOrigin = "http://localhost"
            //});
            Mock<IMessagingService> messagingService = new Mock<IMessagingService>();

            //SecurityServiceManager securityServiceManager =
            //    new SecurityServiceManager(this.PasswordHasher.Object, userManager, roleManager, configurationDbContextResolver.Object, EmailService.Object, serviceOptions.Object);
            
            SecurityServiceManager securityServiceManager =
                new SecurityServiceManager(this.PasswordHasher.Object, userManager, messagingService.Object, roleManager);

            return securityServiceManager;
        }


    }
}

