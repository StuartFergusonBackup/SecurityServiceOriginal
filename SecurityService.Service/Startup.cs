namespace SecurityService.Service
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using IdentityServer4.EntityFramework.DbContexts;
    using Manager;
    using Manager.DbContexts;
    using Manager.DbContexts.SeedData;
    using Manager.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using Shared.General;
    using StructureMap;
    using Swashbuckle.AspNetCore.Swagger;

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
            Startup.PersistedGrantStoreConenctionString = Startup.Configuration.GetConnectionString(nameof(PersistedGrantDbContext));
            Startup.ConfigurationConnectionString = Startup.Configuration.GetConnectionString(nameof(ConfigurationDbContext));
            Startup.AuthenticationConenctionString = Startup.Configuration.GetConnectionString(nameof(AuthenticationDbContext));
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
            if (String.Compare(Startup.HostingEnvironment.EnvironmentName, "Development", true) == 0)
            {
                nlogConfigFilename = $"nlog.{Startup.HostingEnvironment.EnvironmentName}.config";
            }
 
            loggerFactory.AddConsole();
            loggerFactory.ConfigureNLog(Path.Combine(Startup.HostingEnvironment.ContentRootPath, nlogConfigFilename));
            loggerFactory.AddNLog();
 
            ILogger logger = loggerFactory.CreateLogger("Security Service");
            
            Logger.Initialise(logger);
 
            ConfigurationReader.Initialise(Startup.Configuration);

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
            if (!Startup.HostingEnvironment.IsEnvironment("IntegrationTest"))
            {
                this.InitialiseDatabase(app, env).Wait();
            }

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
            Startup.ConfigureMiddlewareServices(services);
 
            IContainer container = Startup.GetConfiguredContainer(services, Startup.HostingEnvironment);
 
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
            Startup.ConfigureCommonServices(services);
 
            var container = new Container();
            container.Configure(Startup.ConfigureCommonContainer);
 
            if (hostingEnvironment.IsDevelopment())
            {
                container.Configure(Startup.ConfigureDevelopmentContainer);
            }
            else
            {
                container.Configure(Startup.ConfigureProductionContainer);
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);;
            
            services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = Startup.Configuration.GetValue<Boolean>("IdentityOptions:PasswordOptions:RequireDigit");
                o.Password.RequireLowercase =
                    Startup.Configuration.GetValue<Boolean>("IdentityOptions:PasswordOptions:RequireLowercase");
                o.Password.RequireUppercase =
                    Startup.Configuration.GetValue<Boolean>("IdentityOptions:PasswordOptions:RequireUppercase");
                o.Password.RequireNonAlphanumeric =
                    Startup.Configuration.GetValue<Boolean>("IdentityOptions:PasswordOptions:RequireNonAlphanumeric");
                o.Password.RequiredLength = Startup.Configuration.GetValue<Int32>("IdentityOptions:PasswordOptions:RequiredLength");
            }).AddEntityFrameworkStores<AuthenticationDbContext>().AddDefaultTokenProviders();
            
            if (Startup.HostingEnvironment.IsEnvironment("IntegrationTest"))
            {
                services.AddIdentityServer(options =>
                    {
                        options.Events.RaiseSuccessEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseErrorEvents = true;
                        options.PublicOrigin = Startup.Configuration.GetValue<String>("ServiceOptions:PublicOrigin");
                        options.IssuerUri = Startup.Configuration.GetValue<String>("ServiceOptions:PublicOrigin");
                    })
                    .AddDeveloperSigningCredential()
                    .AddAspNetIdentity<IdentityUser>()
                    .AddJwtBearerClientAuthentication()
                    .AddIntegrationTestConfiguration();                  

                services.AddDbContext<AuthenticationDbContext>(builder =>
                        builder.UseInMemoryDatabase("Authentication"))
                    .AddTransient<AuthenticationDbContext>();

                services.AddDbContext<ConfigurationDbContext>(builder =>
                        builder.UseInMemoryDatabase("Configuration"))
                    .AddTransient<ConfigurationDbContext>();

                services.AddDbContext<PersistedGrantDbContext>(builder =>
                        builder.UseInMemoryDatabase("PersistedGrantStore"))
                    .AddTransient<PersistedGrantDbContext>();
            }
            else
            {
                String migrationsAssembly = typeof(AuthenticationDbContext).GetTypeInfo().Assembly.GetName().Name;

                services.AddDbContext<ConfigurationDbContext>(builder =>
                        builder.UseSqlServer(Startup.ConfigurationConnectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                    .AddTransient<ConfigurationDbContext>();

                services.AddDbContext<PersistedGrantDbContext>(builder =>
                        builder.UseSqlServer(Startup.PersistedGrantStoreConenctionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                    .AddTransient<PersistedGrantDbContext>();

                services.AddDbContext<AuthenticationDbContext>(builder =>
                        builder.UseSqlServer(Startup.AuthenticationConenctionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)))
                    .AddTransient<AuthenticationDbContext>();

                services.AddIdentityServer(options =>
                        {
                            options.Events.RaiseSuccessEvents = true;
                            options.Events.RaiseFailureEvents = true;
                            options.Events.RaiseErrorEvents = true;
                            options.PublicOrigin = Startup.Configuration.GetValue<String>("ServiceOptions:PublicOrigin");
                            options.IssuerUri = Startup.Configuration.GetValue<String>("ServiceOptions:PublicOrigin");
                        })
                    .AddConfigurationStore()
                    .AddOperationalStore()
                    .AddDeveloperSigningCredential()
                    .AddIdentityServerStorage(Startup.ConfigurationConnectionString)
                    .AddAspNetIdentity<IdentityUser>()
                    .AddJwtBearerClientAuthentication();                
            }
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
                c.SwaggerDoc("v1", new Info { Title = "Security Service", Version = "v1" });
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
            services.AddSingleton<ISecurityServiceManager, SecurityServiceManager>();
            services.AddSingleton<IPasswordHasher<IdentityUser>, PasswordHasher<IdentityUser>>();
            services.AddSingleton<IUserClaimsPrincipalFactory<IdentityUser>, UserClaimsPrincipalFactory<IdentityUser>>();
            services.AddSingleton<ILogger<SignInManager<IdentityUser>>, Logger<SignInManager<IdentityUser>>>();

            Boolean useDummyMessagingService = Startup.Configuration.GetValue<Boolean>("ServiceOptions:UseDummyMessagingService");
            if (useDummyMessagingService)
            {
                services.AddSingleton<IMessagingService, DummyMessagingService>();
            }
            else
            {
                services.AddSingleton<IMessagingService, MessagingService>();
            }            
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

                var seedingType = Startup.Configuration.GetValue<SeedingType>("SeedingType");

                if (seedingType == SeedingType.Production)
                {
                    throw new NotImplementedException("Production setup not complete yet");
                }

                DatabaseSeeding.InitialisePersistedGrantDatabase(persistedGrantDbContext, seedingType);
                DatabaseSeeding.InitialiseConfigurationDatabase(configurationDbContext, seedingType);
                DatabaseSeeding.InitialiseAuthenticationDatabase(authenticationDbContext, seedingType);
            }
        }

        #endregion

        #endregion
    }
}