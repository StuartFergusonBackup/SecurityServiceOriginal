using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SecurityService.DataTransferObjects
{
    public class UserRecord
    {
        /// <summary>
        /// Gets or sets the identity user.
        /// </summary>
        /// <value>
        /// The identity user.
        /// </value>
        public IdentityUser IdentityUser { get; set; }

        /// <summary>
        /// Gets or sets the identity roles.
        /// </summary>
        /// <value>
        /// The identity roles.
        /// </value>
        public IList<String> IdentityRoles { get; set; }

        /// <summary>
        /// Gets or sets the identity user claims.
        /// </summary>
        /// <value>
        /// The identity user claims.
        /// </value>
        public IList<Claim> IdentityUserClaims { get; set; }
    }
}
