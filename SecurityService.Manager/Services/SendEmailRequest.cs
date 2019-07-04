﻿using System;
using System.Collections.Generic;

namespace OAuth2SecurityService.Manager.Services
{
    public class SendEmailRequest
    {
        /// <summary>
        /// Gets or sets from address.
        /// </summary>
        /// <value>
        /// From address.
        /// </value>
        public String FromAddress { get; set; }

        /// <summary>
        /// Gets or sets to addresses.
        /// </summary>
        /// <value>
        /// To addresses.
        /// </value>
        public List<String> ToAddresses { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public String Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is HTML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsHtml { get; set; }
    }
}