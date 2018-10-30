using System;

namespace SecurityService.DataTransferObjects
{
    public class ClientDataTransferObject
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public String ClientId { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public String Environment { get; set; }

        /// <summary>
        /// Gets or sets the name of the client application.
        /// </summary>
        /// <value>
        /// The name of the client application.
        /// </value>
        public String ClientAppName { get; set; }

        /// <summary>
        /// Gets or sets the home URL.
        /// </summary>
        /// <value>
        /// The home URL.
        /// </value>
        public String HomeUrl { get; set; }
    }
}