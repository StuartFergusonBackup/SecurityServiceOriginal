namespace SecurityService.IntegrationTests.Role
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
    public class RoleSteps : GenericSteps
    {
        #region Constructors

        public RoleSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        #endregion

        #region Methods

        [Given(@"A role with then name '(.*)' has been created")]
        public async Task GivenARoleWithThenNameHasBeenCreated(String roleName)
        {
            CreateRoleRequest createRoleRequest = new CreateRoleRequest
                                                  {
                                                      RoleName = roleName
                                                  };

            String requestSerialised = JsonConvert.SerializeObject(createRoleRequest);
            StringContent content = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

            using(HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.SecurityServicePort}");

                HttpResponseMessage response = await client.PostAsync("/api/role/", content, CancellationToken.None).ConfigureAwait(false);
                response.IsSuccessStatusCode.ShouldBeTrue();
            }
        }

        [Given(@"The details of a role I want to create")]
        public void GivenTheDetailsOfARoleIWantToCreate()
        {
            this.ScenarioContext["CreateRoleRequest"] = IntegrationTestsTestData.GetCreateRoleRequest;
        }

        [Given(@"the Security Service is running")]
        public void GivenTheSecurityServiceIsRunning()
        {
            this.RunSystem(this.ScenarioContext.ScenarioInfo.Title);
        }

        [Then(@"the new role Id will be returned")]
        public async Task ThenTheNewRoleIdWillBeReturned()
        {
            HttpResponseMessage httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("CreateRoleHttpResponse");

            CreateRoleResponse responseData = JsonConvert.DeserializeObject<CreateRoleResponse>(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            responseData.RoleId.ShouldNotBe(Guid.Empty);
        }

        [Then(@"the role details should be returned")]
        public async Task ThenTheRoleDetailsShouldBeReturned()
        {
            HttpResponseMessage httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("GetRoleHttpResponse");
            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            GetRoleResponse responseData = JsonConvert.DeserializeObject<GetRoleResponse>(await httpResponse.Content.ReadAsStringAsync());
        }

        [Then(@"the role should be created")]
        public void ThenTheRoleShouldBeCreated()
        {
            HttpResponseMessage httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("CreateRoleHttpResponse");
            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [When(@"I request to create the role")]
        public async Task WhenIRequestToCreateTheRole()
        {
            CreateRoleRequest request = this.ScenarioContext.Get<CreateRoleRequest>("CreateRoleRequest");

            String requestSerialised = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

            using(HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.SecurityServicePort}");

                this.ScenarioContext["CreateRoleHttpResponse"] = await client.PostAsync("/api/role/", content, CancellationToken.None).ConfigureAwait(false);
            }
        }

        [When(@"I request to retrieve the role '(.*)'")]
        public async Task WhenIRequestToRetrieveTheRole(String roleName)
        {
            using(HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.SecurityServicePort}");

                this.ScenarioContext["GetRoleHttpResponse"] = await client.GetAsync($"/api/role?roleName={roleName}", CancellationToken.None).ConfigureAwait(false);
            }
        }

        #endregion
    }
}