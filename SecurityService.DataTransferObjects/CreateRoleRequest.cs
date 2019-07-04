using System;
using System.Collections.Generic;
using System.Text;

namespace OAuth2SecurityService.DataTransferObjects
{
    public class CreateRoleRequest
    {
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>
        /// The name of the role.
        /// </value>
        public String RoleName { get; set; }
    }

    public class CreateRoleResponse
    {
        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <value>
        /// The role identifier.
        /// </value>
        public Guid RoleId { get; set; }
    }
}
