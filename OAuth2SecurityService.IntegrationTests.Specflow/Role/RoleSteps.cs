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

namespace OAuth2SecurityService.IntegrationTests.Specflow.Role
{
    [Binding]
    public class RoleSteps : GenericSteps
    {
        public RoleSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {

        }

        [Given(@"the Security Service is running")]
        public void GivenTheSecurityServiceIsRunning()
        {
            RunSystem(this.ScenarioContext.ScenarioInfo.Title);
        }
        
        [Given(@"The details of a role I want to create")]
        public void GivenTheDetailsOfARoleIWantToCreate()
        {
            this.ScenarioContext["CreateRoleRequest"] = IntegrationTestsTestData.GetCreateRoleRequest;
        }
        
        [When(@"I request to create the role")]
        public async Task WhenIRequestToCreateTheRole()
        {
            var request = this.ScenarioContext.Get<CreateRoleRequest>("CreateRoleRequest");

            String requestSerialised = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:{this.SecurityServicePort}");

                this.ScenarioContext["CreateRoleHttpResponse"] = await client
                    .PostAsync("/api/role/", content, CancellationToken.None).ConfigureAwait(false);
            }
        }
        
        [Then(@"the role should be created")]
        public void ThenTheRoleShouldBeCreated()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("CreateRoleHttpResponse");
            httpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
        
        [Then(@"the new role Id will be returned")]
        public async Task ThenTheNewRoleIdWillBeReturned()
        {
            var httpResponse = this.ScenarioContext.Get<HttpResponseMessage>("CreateRoleHttpResponse");

            var responseData = JsonConvert.DeserializeObject<CreateRoleResponse>(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            responseData.RoleId.ShouldNotBe(Guid.Empty);
        }
    }
}
