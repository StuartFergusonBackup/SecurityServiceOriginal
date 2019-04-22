using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OAuth2SecurityService.DataTransferObjects;
using OAuth2SecurityService.IntegrationTests.Specflow.Common;
using Shouldly;
using TechTalk.SpecFlow;


namespace OAuth2SecurityService.IntegrationTests.Specflow.User
{
    [Binding]
    [Scope(Tag = "user")]
    public class UserSteps : GenericSteps
    {
        public UserSteps(ScenarioContext scenarioContext) : base(scenarioContext)
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

        [Given(@"I have my user details to register")]
        public void GivenIHaveMyUserDetailsToRegister()
        {
            this.ScenarioContext["RegisterUserRequest"] = IntegrationTestsTestData.GetRegisterUserRequest;
        }
        
        [Given(@"I have provided a password")]
        public void GivenIHaveProvidedAPassword()
        {
            this.ScenarioContext["Password"] = IntegrationTestsTestData.Password;
        }
        
        [When(@"I register")]
        public async Task WhenIRegister()
        {
            var request = this.ScenarioContext.Get<RegisterUserRequest>("RegisterUserRequest");
            var password = this.ScenarioContext.Get<String>("Password");

            // Update the request with the password
            request.Password = password;

            String requestSerialised = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.SecurityServicePort}");

                this.ScenarioContext["RegisterUserHttpResponse"] = await client
                    .PostAsync("/api/user/", content, CancellationToken.None).ConfigureAwait(false);
            }
        }
        
        [Then(@"my details should be registered")]
        public void ThenMyDetailsShouldBeRegistered()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("RegisterUserHttpResponse");
            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Then(@"my new User Id will be returned")]
        public async Task ThenMyNewUserIdWillBeReturned()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("RegisterUserHttpResponse");

            var responseData = JsonConvert.DeserializeObject<RegisterUserResponse>(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            responseData.UserId.ShouldNotBe(Guid.Empty);
        }

    }
}
