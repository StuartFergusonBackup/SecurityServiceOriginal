using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using OAuth2SecurityService.Manager.DbContexts;
using OAuth2SecurityService.Manager.DbContexts.SeedData;
using Shared.General;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;

namespace OAuth2SecurityService.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        #region Properties
 
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public static IConfigurationRoot Configuration { get; set; }
 
        /// <summary>
        /// Gets or sets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public static IHostingEnvironment HostingEnvironment { get; set; }

        /// <summary>
        /// The authentication conenction string
        /// </summary>
        private static String AuthenticationConenctionString;

        /// <summary>
        /// The configuration connection string
        /// </summary>
        private static String ConfigurationConnectionString;

        /// <summary>
        /// The persisted grant store conenction string
        /// </summary>
        private static String PersistedGrantStoreConenctionString;
 
        #endregion
        
        #region Constructors
 
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The env.</param>
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder =
                new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional:true, reloadOnChange:true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional:true)
                    .AddEnvironmentVariables();
 
            Startup.Configuration = builder.Build();
            Startup.HostingEnvironment = env;

            // Get the DB Connection Strings
            PersistedGrantStoreConenctionString = Startup.Configuration.GetConnectionString(nameof(PersistedGrantDbContext));
            ConfigurationConnectionString = Startup.Configuration.GetConnectionString(nameof(ConfigurationDbContext));
            AuthenticationConenctionString = Startup.Configuration.GetConnectionString(nameof(AuthenticationDbContext));
        }
 
        #endregion

        #region Public Methods

        #region public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)        

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            String nlogConfigFilename = $"nlog.config";
            if (String.Compare(HostingEnvironment.EnvironmentName, "Development", true) == 0)
            {
                nlogConfigFilename = $"nlog.{HostingEnvironment.EnvironmentName}.config";
            }
 
            loggerFactory.AddConsole();
            loggerFactory.ConfigureNLog(Path.Combine(HostingEnvironment.ContentRootPath, nlogConfigFilename));
            loggerFactory.AddNLog();
 
            ILogger logger = loggerFactory.CreateLogger("Security Service");
            
            Logger.Initialise(logger);
 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
 
            app.UseStaticFiles();            

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();

            // Setup the database
            this.InitialiseDatabase(app, env).Wait();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "OAuth2 Security Service v1"); });
        }
        #endregion

        #endregion

        #region public IServiceProvider ConfigureServices(IServiceCollection services)        
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ConfigureMiddlewareServices(services);
 
            IContainer container = GetConfiguredContainer(services, HostingEnvironment);
 
            return container.GetInstance<IServiceProvider>();
        }
        #endregion

        #region public static IContainer GetConfiguredContainer(IServiceCollection services, IHostingEnvironment hostingEnvironment)        
        /// <summary>
        /// Gets the configured container.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <returns></returns>
        public static IContainer GetConfiguredContainer(IServiceCollection services, IHostingEnvironment hostingEnvironment)
        {
            ConfigureCommonServices(services);
 
            var container = new Container();
            container.Configure(ConfigureCommonContainer);
 
            if (hostingEnvironment.IsDevelopment())
            {
                container.Configure(ConfigureDevelopmentContainer);
            }
            else
            {
                container.Configure(ConfigureProductionContainer);
            }
 
            container.Populate(services);
 
            return container;
        }
        #endregion

        #region Private Methods
 
        #region private static void ConfigureMiddlewareServices(IServiceCollection services)        
        /// <summary>
        /// Configures the middleware services.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void ConfigureMiddlewareServices(IServiceCollection services)
        {            
            services.AddMvc();
            
            String migrationsAssembly = typeof(AuthenticationDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ConfigurationDbContext>(builder =>
                    builder.UseMySql(ConfigurationConnectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                .AddTransient<ConfigurationDbContext>();

            services.AddDbContext<PersistedGrantDbContext>(builder =>
                    builder.UseMySql(PersistedGrantStoreConenctionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                .AddTransient<PersistedGrantDbContext>();

            services.AddDbContext<AuthenticationDbContext>(builder =>
                    builder.UseMySql(AuthenticationConenctionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                .AddTransient<AuthenticationDbContext>();
            
            services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = Configuration.GetValue<Boolean>("IdentityOptions:PasswordOptions:RequireDigit");
                o.Password.RequireLowercase =
                    Configuration.GetValue<Boolean>("IdentityOptions:PasswordOptions:RequireLowercase");
                o.Password.RequireUppercase =
                    Configuration.GetValue<Boolean>("IdentityOptions:PasswordOptions:RequireUppercase");
                o.Password.RequireNonAlphanumeric =
                    Configuration.GetValue<Boolean>("IdentityOptions:PasswordOptions:RequireNonAlphanumeric");
                o.Password.RequiredLength = Configuration.GetValue<Int32>("IdentityOptions:PasswordOptions:RequiredLength");
            }).AddEntityFrameworkStores<AuthenticationDbContext>().AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
                    {
                        options.Events.RaiseSuccessEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseErrorEvents = true;
                        options.PublicOrigin = Configuration.GetValue<String>("PublicOrigin");
                    })
                .AddConfigurationStore()
                .AddOperationalStore()
                .AddDeveloperSigningCredential()
                .AddIdentityServerStorage(ConfigurationConnectionString)
                .AddAspNetIdentity<IdentityUser>()
                .AddJwtBearerClientAuthentication();

            services.AddCors();
            
            // Read the authentication configuration
            //var securityConfig = new SecurityServiceConfiguration();
            //Configuration.GetSection("SecurityConfiguration").Bind(securityConfig);

            //services.AddAuthentication("Bearer")
            //    .AddIdentityServerAuthentication("token", options =>
            //    {
            //        options.Authority = securityConfig.SecurityService;
            //        options.RequireHttpsMetadata = false;
            //        options.ApiName = securityConfig.ApiName;
            //    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "OAuth2 Security Service", Version = "v1" });
            });
        }
        #endregion

        #region private static void ConfigureCommonServices(IServiceCollection services)        
        /// <summary>
        /// Configures the common services.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void ConfigureCommonServices(IServiceCollection services)
        {
        }
        #endregion

        #region private static void ConfigureCommonContainer(ConfigurationExpression configurationExpression)        
        /// <summary>
        /// Configures the common container.
        /// </summary>
        /// <param name="configurationExpression">The configuration expression.</param>
        private static void ConfigureCommonContainer(ConfigurationExpression configurationExpression)
        {
        }
        #endregion

        #region private static void ConfigureDevelopmentContainer(ConfigurationExpression configurationExpression)        
        /// <summary>
        /// Configures the development container.
        /// </summary>
        /// <param name="configurationExpression">The configuration expression.</param>
        private static void ConfigureDevelopmentContainer(ConfigurationExpression configurationExpression)
        {
           
        }
        #endregion
 
        #region private static void ConfigureProductionContainer(ConfigurationExpression configurationExpression)        
        /// <summary>
        /// Configures the production container.
        /// </summary>
        /// <param name="configurationExpression">The configuration expression.</param>
        private static void ConfigureProductionContainer(ConfigurationExpression configurationExpression)
        {
            
        }
        #endregion

        #region private async Task InitialiseDatabase(IApplicationBuilder app, IHostingEnvironment environment)
        /// <summary>
        /// Initialises the database.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="environment">The environment.</param>
        private async Task InitialiseDatabase(IApplicationBuilder app, IHostingEnvironment environment)
        {
            using(IServiceScope scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                PersistedGrantDbContext persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                ConfigurationDbContext configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                AuthenticationDbContext authenticationDbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();

                var seedingType = Configuration.GetValue<SeedingType>("SeedingType");

                if (seedingType == SeedingType.Production)
                {
                    throw new NotImplementedException("Production setup not complete yet");
                }

                DatabaseSeeding.InitialiseDatabase(configurationDbContext, persistedGrantDbContext, authenticationDbContext, seedingType);                
            }
        }

        #endregion

        #endregion
    }
}