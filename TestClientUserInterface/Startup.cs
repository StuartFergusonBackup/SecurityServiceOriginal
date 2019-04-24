using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestClientUserInterface
{
    using System.IdentityModel.Tokens.Jwt;
    using IdentityModel;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.IdentityModel.Tokens;
    using TokenManagement;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHttpClient();

            services.AddAuthentication(options =>
                                       {
                                           options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                           options.DefaultChallengeScheme = "oidc";
                                       })
                    .AddCookie(options =>
                               {
                                   options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                                   options.Cookie.Name = "mvchybridautorefresh";
                               })
                    .AddAutomaticTokenManagement()
                    .AddOpenIdConnect("oidc", options =>
                                              {
                                                  options.Authority = "http://localhost:5001";
                                                  options.RequireHttpsMetadata = false;

                                                  options.ClientSecret = "golfhandicap.adminwebsite";
                                                  options.ClientId = "golfhandicap.adminwebsite";

                                                  options.ResponseType = "code id_token";

                                                  options.Scope.Clear();
                                                  options.Scope.Add("openid");
                                                  options.Scope.Add("profile");
                                                  options.Scope.Add("email");
                                                  options.Scope.Add("offline_access");

                                                  options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                                                  options.GetClaimsFromUserInfoEndpoint = true;
                                                  options.SaveTokens = true;

                                                  options.TokenValidationParameters = new TokenValidationParameters
                                                                                      {
                                                                                          NameClaimType = JwtClaimTypes.Name,
                                                                                          RoleClaimType = JwtClaimTypes.Role,
                                                                                      };
                                              });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
