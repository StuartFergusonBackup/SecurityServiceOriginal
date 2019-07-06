namespace SecurityService.IntegrationTests.Common
{
    using System;
    using Newtonsoft.Json;

    public class ClientTokenReponse
    {
        #region Constructors

        #endregion

        #region Properties

        [JsonProperty("access_token")]
        public String AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public Int32 ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public String TokenType { get; set; }

        #endregion
    }
}