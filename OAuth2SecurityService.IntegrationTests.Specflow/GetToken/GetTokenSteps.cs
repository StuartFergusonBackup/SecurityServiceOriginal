using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OAuth2SecurityService.IntegrationTests.Specflow.Common;
using Shouldly;
using TechTalk.SpecFlow;

namespace OAuth2SecurityService.IntegrationTests.Specflow
{
    [Binding]
    [Scope(Tag = "gettoken")]
    public class GetTokenSteps : GenericSteps
    {
        public GetTokenSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {

        }

        [Given(@"the Security Service is running")]
        public void GivenTheSecurityServiceIsRunning()
        {
            RunSystem(this.ScenarioContext.ScenarioInfo.Title);
        }

        [AfterScenario()]
        public void AfterScenario()
        {
            StopSystem();
        }
        
        [Given(@"I have the Client Id '(.*)'")]
        public void GivenIHaveTheClientId(String clientId)
        {
            this.ScenarioContext["ClientId"] = clientId;
        }
        
        [Given(@"the secret '(.*)'")]
        public void GivenTheSecret(String clientSecret)
        {
            this.ScenarioContext["ClientSecret"] = clientSecret;
        }
        
        [When(@"I request a client token")]
        public async Task WhenIRequestAClientToken()
        {
            var clientId = this.ScenarioContext.Get<String>("ClientId");
            var clientSecret = this.ScenarioContext.Get<String>("ClientSecret");

            StringBuilder queryString = new StringBuilder();
            queryString.Append("grant_type=client_credentials");
            queryString.Append($"&client_id={clientId}");
            queryString.Append($"&client_secret={clientSecret}");

            HttpContent content = new StringContent(queryString.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.SecurityServicePort}");

                    this.ScenarioContext["ClientTokenHttpResponse"] = await client
                        .PostAsync("/connect/token", content, CancellationToken.None).ConfigureAwait(false);
            }
        }
        
        [Then(@"my client token request is successful")]
        public void ThenMyClientTokenRequestIsSuccessful()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("ClientTokenHttpResponse");
            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }


        [Then(@"a client token is returned to me")]
        public async Task ThenAClientTokenIsReturnedToMe()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("ClientTokenHttpResponse");

            var responseData = JsonConvert.DeserializeObject<ClientTokenReponse>(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            responseData.ShouldNotBeNull();
            responseData.AccessToken.ShouldNotBeNullOrEmpty();
            responseData.ExpiresIn.ShouldBeGreaterThan(0);
            responseData.TokenType.ShouldBe("bearer", StringCompareShould.IgnoreCase);
        }

        [Given(@"the username '(.*)'")]
        public void GivenTheUsername(String username)
        {
            this.ScenarioContext["Username"] = username;
        }
        
        [Given(@"the password '(.*)'")]
        public void GivenThePassword(String password)
        {
            this.ScenarioContext["Password"] = password;
        }
        
        [When(@"I request a password token")]
        public async Task WhenIRequestAPasswordToken()
        {
            var clientId = this.ScenarioContext.Get<String>("ClientId");
            var clientSecret = this.ScenarioContext.Get<String>("ClientSecret");
            var username = this.ScenarioContext.Get<String>("Username");
            var password = this.ScenarioContext.Get<String>("Password");

            StringBuilder queryString = new StringBuilder();
            queryString.Append("grant_type=password");
            queryString.Append($"&client_id={clientId}");
            queryString.Append($"&client_secret={clientSecret}");
            queryString.Append($"&username={username}");
            queryString.Append($"&password={password}");

            HttpContent content = new StringContent(queryString.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.SecurityServicePort}");
                
                this.ScenarioContext["PasswordTokenHttpResponse"] = await client.PostAsync("/connect/token", content, CancellationToken.None).ConfigureAwait(false);
            }
        }
        
        [Then(@"my password token request is successful")]
        public void ThenMyPasswordTokenRequestIsSuccessful()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("PasswordTokenHttpResponse");
            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        
        [Then(@"a password token is returned to me")]
        public async Task ThenAPasswordTokenIsReturnedToMe()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("PasswordTokenHttpResponse");

            var responseData = JsonConvert.DeserializeObject<UserTokenReponse>(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            responseData.ShouldNotBeNull();
            responseData.AccessToken.ShouldNotBeNullOrEmpty();
            responseData.ExpiresIn.ShouldBeGreaterThan(0);
            responseData.TokenType.ShouldBe("bearer", StringCompareShould.IgnoreCase);
        }

    }
}
