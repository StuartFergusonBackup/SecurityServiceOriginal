using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace OAuth2SecurityService.IntegrationTests
{
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

    public class IntegrationTests
    {
        public const String TestClientId = "integrationTestClient";
        public const String TestClientSecret = "integrationTestClientSecret";
        public const String BaseAddress = "http://localhost:5001";

        [Fact]
        public async Task OAuth2SecurityService_GetClientToken_TokenReceived()
        {
            StringBuilder queryString = new StringBuilder();
            queryString.Append("grant_type=client_credentials");
            queryString.Append($"&client_id={TestClientId}");
            queryString.Append($"&client_secret={TestClientSecret}");

            HttpContent content = new StringContent(queryString.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                
                var response = await client.PostAsync("/connect/token", content, CancellationToken.None);

                response.EnsureSuccessStatusCode();

                var clientToken = JsonConvert.DeserializeObject<ClientTokenReponse>(await response.Content.ReadAsStringAsync());

                clientToken.ShouldNotBeNull();
                clientToken.AccessToken.ShouldNotBeNullOrEmpty();
                clientToken.ExpiresIn.ShouldBeGreaterThan(0);
                clientToken.TokenType.ShouldBe("bearer", StringCompareShould.IgnoreCase);
            }
        }
    }
}
