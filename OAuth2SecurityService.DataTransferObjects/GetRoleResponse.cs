namespace OAuth2SecurityService.DataTransferObjects
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class GetRoleResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the normalized.
        /// </summary>
        /// <value>
        /// The name of the normalized.
        /// </value>
        public String NormalizedName { get; set; }

        #endregion
    }
}