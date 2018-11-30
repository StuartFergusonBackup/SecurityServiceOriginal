using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using MySql.Data.MySqlClient;
using TechTalk.SpecFlow;

namespace OAuth2SecurityService.IntegrationTests.Specflow.Common
{
    [Binding]
    public class GenericSteps
    {
        protected ScenarioContext ScenarioContext;
        protected IContainerService SecurityServiceContainer;
        protected INetworkService TestNetwork;
        protected Int32 SecurityServicePort;
        
        protected GenericSteps(ScenarioContext scenarioContext) 
        {
            this.ScenarioContext = scenarioContext;
        }
        
        protected void RunSystem(String testFolder)
        {
            String securityServiceContainerName = $"securityService{Guid.NewGuid()}";
            
            // Build a network
            this.TestNetwork = new Builder().UseNetwork($"test-network-{Guid.NewGuid()}").Build();
            
            // Security Service Container
            this.SecurityServiceContainer = new Builder()
                .UseContainer()
                .WithName(securityServiceContainerName)
                .WithEnvironment("SeedingType=IntegrationTest", "ASPNETCORE_ENVIRONMENT=IntegrationTest")
                .UseImage("oauth2securityserviceservice")
                .ExposePort(5001)
                .UseNetwork(this.TestNetwork)                    
                .Mount($"D:\\temp\\docker\\{testFolder}", "/home", MountType.ReadWrite)
                .WaitForPort("5001/tcp", 30000)
                .Build()
                .Start();

            // Cache the ports
            this.SecurityServicePort = this.SecurityServiceContainer.ToHostExposedEndpoint("5001/tcp").Port;            
        }

        protected void StopSystem()
        {
            if (this.SecurityServiceContainer != null)
            {
                this.SecurityServiceContainer.Stop();
                this.SecurityServiceContainer.Remove(true);
            }

            if (this.TestNetwork != null)
            {
                this.TestNetwork.Stop();
                this.TestNetwork.Remove(true);
            }
        }        
    }
}
