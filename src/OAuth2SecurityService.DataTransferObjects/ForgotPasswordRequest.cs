using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityService.DataTransferObjects
{
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public String ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client environment.
        /// </summary>
        /// <value>
        /// The client environment.
        /// </value>
        public String ClientEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the callback controller.
        /// </summary>
        /// <value>
        /// The callback controller.
        /// </value>
        public String CallbackController { get; set; }

        /// <summary>
        /// Gets or sets the callback action.
        /// </summary>
        /// <value>
        /// The callback action.
        /// </value>
        public String CallbackAction { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public String EmailAddress { get; set; }
    }
}
