using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityService.DataTransferObjects
{
    public class PasswordToken
    {
        #region Constructors

        #endregion

        #region Properties

        [JsonProperty("access_token")]
        public String AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public Int32 ExpiresIn { get; set; }

        #endregion
    }
}
