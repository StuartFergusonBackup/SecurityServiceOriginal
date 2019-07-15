namespace SecurityService.IntegrationTests.User
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using DataTransferObjects;
    using Newtonsoft.Json;
    using Shouldly;
    using TechTalk.SpecFlow;

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
            this.RunSystem(this.ScenarioContext.ScenarioInfo.Title);
        }

        [AfterScenario()]
        public void AfterScenario()
        {
            this.StopSystem();
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
            RegisterUserRequest request = this.ScenarioContext.Get<RegisterUserRequest>("RegisterUserRequest");
            String password = this.ScenarioContext.Get<String>("Password");

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
            HttpResponseMessage httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("RegisterUserHttpResponse");
            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Then(@"my new User Id will be returned")]
        public async Task ThenMyNewUserIdWillBeReturned()
        {
            HttpResponseMessage httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("RegisterUserHttpResponse");

            RegisterUserResponse responseData = JsonConvert.DeserializeObject<RegisterUserResponse>(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            responseData.UserId.ShouldNotBe(Guid.Empty);
            this.ScenarioContext["UserId"] = responseData.UserId;
        }

        [Given(@"I have the Id of a registered user")]
        public void GivenIHaveTheIdOfARegisteredUser()
        {
            Guid userId = this.ScenarioContext.Get<Guid>("UserId");
            userId.ShouldNotBe(Guid.Empty);
        }

        [When(@"I get the users details by Id")]
        public async Task WhenIGetTheUsersDetailsById()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.SecurityServicePort}");
                Guid userId = this.ScenarioContext.Get<Guid>("UserId");

                this.ScenarioContext["GetUserHttpResponse"] = await client
                                                                         .GetAsync($"/api/user?userId={userId}", CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Then(@"the user details will be returned")]
        public async Task ThenTheUserDetailsWillBeReturned()
        {
            HttpResponseMessage httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("RegisterUserHttpResponse");
            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            GetUserResponse responseData = JsonConvert.DeserializeObject<GetUserResponse>(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            responseData.ShouldNotBeNull();
        }


    }
}
