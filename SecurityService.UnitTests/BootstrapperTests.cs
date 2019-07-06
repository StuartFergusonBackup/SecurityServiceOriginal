namespace SecurityService.UnitTests
{
    using System;
    using System.Collections.Generic;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Service;
    using StructureMap;
    using Xunit;

    public class BootstrapperTests
    {
        [Fact]
        public void VerifyBootstrapperIsValid_Development()
        {
            ServiceCollection servicesCollection = new ServiceCollection();
            Mock<IHostingEnvironment> hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(he => he.EnvironmentName).Returns("Development");

            Startup.Configuration = this.SetupMemoryConfiguration();

            IContainer container = Startup.GetConfiguredContainer(servicesCollection, hostingEnvironment.Object);

            this.AddTestRegistrations(container);

            container.AssertConfigurationIsValid();
        }

        [Fact]
        public void VerifyBootstrapperIsValid_Production()
        {
            ServiceCollection servicesCollection = new ServiceCollection();
            Mock<IHostingEnvironment> hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(he => he.EnvironmentName).Returns("Production");

            Startup.Configuration = this.SetupMemoryConfiguration();

            IContainer container = Startup.GetConfiguredContainer(servicesCollection, hostingEnvironment.Object);

            this.AddTestRegistrations(container);

            container.AssertConfigurationIsValid();
        }

        private IConfigurationRoot SetupMemoryConfiguration()
        {
            Dictionary<String, String> configuration = new Dictionary<String, String>();

            
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(configuration);

            return builder.Build();
        }

        private void AddTestRegistrations(IContainer container)
        {
            Mock<IIdentityServerInteractionService> service = new Mock<IIdentityServerInteractionService>();

            container.Configure(c => c.For<IIdentityServerInteractionService>().Use(() => service.Object));
            container.Configure(c=> c.For<IClientStore>().Use<InMemoryClientStore>());
            container.Configure(c=> c.For<IUserStore<IdentityUser>>().Use<UserStore<IdentityUser>>());
            container.Configure(c=> c.For<IResourceStore>().Use<InMemoryResourcesStore>());
            container.Configure(c => c.For<ILoggerFactory>().Use<LoggerFactory>());
            container.Configure(c=> c.For<IAuthenticationSchemeProvider>().Use<AuthenticationSchemeProvider>());
            container.Configure(c => c.For<IHttpContextAccessor>().Use<HttpContextAccessor>());
            container.Configure(c=> c.For<DbContextOptions>().Use<DbContextOptions<DbContext>>());
            container.Configure(c => c.For<ILookupNormalizer>().Use<UpperInvariantLookupNormalizer>());
            container.Configure(c => c.For<ILogger<UserManager<IdentityUser>>>().Use<Logger<UserManager<IdentityUser>>>());
            container.Configure(c=> c.For<IRoleStore<IdentityRole>>().Use<RoleStore<IdentityRole>>());
            container.Configure(c => c.For<ILogger<RoleManager<IdentityRole>>>().Use<Logger<RoleManager<IdentityRole>>>());
            container.Configure(c=> c.For<IDataProtectionProvider>().Use<EphemeralDataProtectionProvider>());
            
            container.Configure(c => c.For<IOptions<PasswordHasherOptions>>().Use<OptionsManager<PasswordHasherOptions>>());
            container.Configure(c => c.For<IOptionsFactory<PasswordHasherOptions>>().Use<OptionsFactory<PasswordHasherOptions>>());
            container.Configure(c => c.For<IOptions<AuthenticationOptions>>().Use<OptionsManager<AuthenticationOptions>>());
            container.Configure(c => c.For<IOptionsFactory<AuthenticationOptions>>().Use<OptionsFactory<AuthenticationOptions>>());
            container.Configure(c => c.For<IOptions<IdentityOptions>>().Use<OptionsManager<IdentityOptions>>());
            container.Configure(c => c.For<IOptionsFactory<IdentityOptions>>().Use<OptionsFactory<IdentityOptions>>());
            container.Configure(c => c.For<IOptions<CookieTempDataProviderOptions>>().Use<OptionsManager<CookieTempDataProviderOptions>>());
            container.Configure(c => c.For<IOptionsFactory<CookieTempDataProviderOptions>>().Use<OptionsFactory<CookieTempDataProviderOptions>>());
            //container.Configure(c => c.For<IOptions<EmailOptions>>().Use<OptionsManager<EmailOptions>>());
            //container.Configure(c => c.For<IOptionsFactory<EmailOptions>>().Use<OptionsFactory<EmailOptions>>());
            //container.Configure(c => c.For<IOptions<ServiceOptions>>().Use<OptionsManager<ServiceOptions>>());
            //container.Configure(c => c.For<IOptionsFactory<ServiceOptions>>().Use<OptionsFactory<ServiceOptions>>());
        }
    }
}