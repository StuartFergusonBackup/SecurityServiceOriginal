using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;

namespace OAuth2SecurityService.Service
{
    public class InMemoryUserStore : IUserStore<IdentityUser>, 
        IUserPasswordStore<IdentityUser>, 
        IUserRoleStore<IdentityUser>,
        IUserClaimStore<IdentityUser>
    {
        private readonly List<IdentityUser> IdentityUsers;
        private readonly List<IdentityUserRole<String>> IdentityUserRoles;
        private readonly List<IdentityRole> IdentityRoles;
        private readonly List<IdentityUserClaim<String>> IdentityUserClaims;
        
        public void Dispose()
        {
            
        }

        public InMemoryUserStore(IEnumerable<IdentityUser> identityUsers, IEnumerable<IdentityRole> identityRoles, 
            IEnumerable<IdentityUserRole<String>> identityUserRoles, 
            IEnumerable<IdentityUserClaim<String>> identityUserClaims)
        {
            if (identityUsers.HasDuplicates(m => m.UserName))
            {
                throw new ArgumentException("Users must not contain duplicate user names");
            }

            if (identityRoles.HasDuplicates(m => m.Name))
            {
                throw new ArgumentException("Roles must not contain duplicate names");
            }

            this.IdentityUsers = identityUsers.ToList();
            this.IdentityRoles = identityRoles.ToList();
            this.IdentityUserRoles = identityUserRoles.ToList();
            this.IdentityUserClaims = identityUserClaims.ToList();
        }

        private void ValidateModel(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
        }

        #region IUserStore Implementation

        public async Task<String> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            this.ValidateModel(user);
           
            return user.Id;
        }

        public async Task<String> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            this.ValidateModel(user);

            return user.UserName;
        }

        public async Task SetUserNameAsync(IdentityUser user, String userName, CancellationToken cancellationToken)
        {
            this.ValidateModel(user);

            user.UserName = userName;
        }

        public async Task<String> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            this.ValidateModel(user);

            return user.NormalizedUserName;
        }

        public async Task SetNormalizedUserNameAsync(IdentityUser user, String normalizedName, CancellationToken cancellationToken)
        {
            this.ValidateModel(user);

            user.NormalizedUserName = normalizedName;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            this.ValidateModel(user);

            this.IdentityUsers.Add(user);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            this.ValidateModel(user);

            var localUser = this.IdentityUsers.Where(iu => iu.Id == user.Id).Single();
            this.IdentityUsers.Remove(localUser);

            this.IdentityUsers.Add(user);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            this.ValidateModel(user);
            
            this.IdentityUsers.Remove(user);

            return IdentityResult.Success;
        }

        public async Task<IdentityUser> FindByIdAsync(String userId, CancellationToken cancellationToken)
        {
            var localUser = this.IdentityUsers.Where(iu => iu.Id == userId).SingleOrDefault();

            return localUser;
        }

        public async Task<IdentityUser> FindByNameAsync(String normalizedUserName, CancellationToken cancellationToken)
        {
            var localUser = this.IdentityUsers.Where(iu => iu.NormalizedUserName == normalizedUserName).SingleOrDefault();

            return localUser;
        }
        #endregion

        #region IUserPasswordStore Implementation

        public async Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
        }

        public async Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return user.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return String.IsNullOrEmpty(user.PasswordHash) ? false : true;
        }
        #endregion

        #region IUserRoleStore Implementation

        public async Task AddToRoleAsync(IdentityUser user, String roleName, CancellationToken cancellationToken)
        {
            var role = this.IdentityRoles.Where(r => r.NormalizedName == roleName).Single();

            this.IdentityUserRoles.Add(new IdentityUserRole<String>{RoleId = role.Id, UserId = user.Id});
        }

        public async Task RemoveFromRoleAsync(IdentityUser user, String roleName, CancellationToken cancellationToken)
        {
            var role = this.IdentityRoles.Where(r => r.Name == roleName).Single();
            var identityUserRole = this.IdentityUserRoles.Where(ir => ir.UserId == user.Id && ir.RoleId == role.Id).Single();
            this.IdentityUserRoles.Remove(identityUserRole);
        }

        public async Task<IList<String>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var result = (from identityRoles in this.IdentityRoles
                join identityUserRoles in this.IdentityUserRoles on identityRoles.Id equals identityUserRoles.RoleId
                join identityUsers in this.IdentityUsers on identityUserRoles.UserId equals identityUsers.Id
                where identityUsers.Id == user.Id
                select identityRoles.NormalizedName).ToList();

            return result;

        }

        public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            var result = (from identityRoles in this.IdentityRoles
                join identityUserRoles in this.IdentityUserRoles on identityRoles.Id equals identityUserRoles.RoleId
                join identityUsers in this.IdentityUsers on identityUserRoles.UserId equals identityUsers.Id
                where identityUsers.Id == user.Id
                      && identityRoles.NormalizedName == roleName
                select identityRoles.Name).Any();

            return result;
        }

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var result = (from identityRoles in this.IdentityRoles
                join identityUserRoles in this.IdentityUserRoles on identityRoles.Id equals identityUserRoles.RoleId
                join identityUsers in this.IdentityUsers on identityUserRoles.UserId equals identityUsers.Id
                where identityRoles.NormalizedName == roleName
                select identityUsers).ToList();

            return result;
        }

        #endregion

        #region IUserClaimStore

        public async Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var claims = (from c in this.IdentityUserClaims
                          where c.UserId == user.Id
                          select new Claim(c.ClaimType, c.ClaimValue)).ToList();

            return claims;
        }

        public async Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                this.IdentityUserClaims.Add(new IdentityUserClaim<String>
                {
                    Id = this.IdentityUserClaims.Count + 1,
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
        }

        public async Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {            
            this.IdentityUserClaims.RemoveAll(c => c.ClaimType == claim.Type && c.UserId == user.Id);
            this.IdentityUserClaims.Add(new IdentityUserClaim<String>
            {
                Id = this.IdentityUserClaims.Count + 1,
                UserId = user.Id,
                ClaimType = newClaim.Type,
                ClaimValue = newClaim.Value
            });
        }

        public async Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                this.IdentityUserClaims.RemoveAll(c => c.ClaimType == claim.Type && c.UserId == user.Id);
            }
        }

        public async Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var users = (from identityUsers in this.IdentityUsers
                join identityUserClaims in this.IdentityUserClaims on identityUsers.Id equals identityUserClaims.UserId
                select identityUsers).ToList();

            return users;
        }
        #endregion
    }
}