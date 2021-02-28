using dic.sso.webapp.clienteweb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SampleMvcApp.Support;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace dic.sso.webapp.clienteweb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICompanyHttpClient, CompanyHttpClient>();

            #region Autenticacion con is4



            ////JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //services.AddAuthentication(opt =>
            //{
            //    opt.DefaultScheme = "Cookies";
            //    opt.DefaultChallengeScheme = "oidc";
            //})
            //.AddCookie("Cookies", opt =>
            //{
            //    opt.AccessDeniedPath = "/Account/AccessDenied";
            //})
            //.AddOpenIdConnect("oidc", opt =>
            // {
            //     opt.SignInScheme = "Cookies";
            //     opt.Authority = "http://localhost:5001";
            //     opt.ClientId = "mvc-client";
            //     opt.ResponseType = "code id_token";
            //     opt.SaveTokens = true;
            //     opt.ClientSecret = "MVCSecret";
            //     opt.RequireHttpsMetadata = false;
            //     opt.GetClaimsFromUserInfoEndpoint = true;

            //     opt.ClaimActions.DeleteClaim("sid");
            //     opt.ClaimActions.DeleteClaim("idp");
            //     // or
            //     //opt.ClaimActions.DeleteClaims(new string[] { "sid", "idp" });
            //     opt.Scope.Add("address");
            //     //opt.Scope.Add("email");
            //     //opt.ClaimActions.MapUniqueJsonKey("email", "email");

            //     opt.Scope.Add("roles");
            //     opt.ClaimActions.MapUniqueJsonKey("role", "role");
            //     opt.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         RoleClaimType = "role",
            //         NameClaimType = "email"
            //     };

            //     opt.Scope.Add("companyApi");
            //     opt.Scope.Add("position");
            //     opt.Scope.Add("country");
            //     opt.ClaimActions.MapUniqueJsonKey("position", "position");
            //     opt.ClaimActions.MapUniqueJsonKey("country", "country");

            // });


            //services.AddAuthorization(authOpt =>
            //{
            //    authOpt.AddPolicy("CanCreateAndModifyData", policyBuilder =>
            //    {
            //        policyBuilder.RequireAuthenticatedUser();
            //        policyBuilder.RequireClaim("position", "Administrator");
            //        policyBuilder.RequireClaim("country", "USA");
            //    });
            //});




            #endregion


            #region demo con Auth0
            services.ConfigureSameSiteNoneCookies();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddOpenIdConnect("Auth0", options =>
                {
                    // Set the authority to your Auth0 domain
                    options.Authority = $"https://{Configuration["Auth0:Domain"]}";

                    // Configure the Auth0 Client ID and Client Secret
                    options.ClientId = Configuration["Auth0:ClientId"];
                    options.ClientSecret = Configuration["Auth0:ClientSecret"];
                    // Set response type to code
                    options.RequireHttpsMetadata = false;
                    options.ResponseType = "code";

                    // Configure the scope
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");

                    // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
                    // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
                    //options.CallbackPath = new PathString("/siging-auth0");
                    options.CallbackPath = new PathString("/siging-auth0");

                    // Configure the Claims Issuer to be Auth0
                    options.ClaimsIssuer = "Auth0";

                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        //RoleClaimType = "https://schemas.quickstarts.com/roles" --- para trabajar con roles....
                    };

                    options.Events = new OpenIdConnectEvents
                    {
                        // handle the logout redirection 
                        OnRedirectToIdentityProviderForSignOut = (context) =>
                        {
                            var logoutUri = $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

                            var postLogoutUri = context.Properties.RedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = context.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                }
                                logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                            }

                            context.Response.Redirect(logoutUri);
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };

                });


            //services.AddAuthentication(opt =>
            //{
            //    opt.DefaultScheme = "Cookies";
            //    opt.DefaultChallengeScheme = "oidc";
            //}).AddOpenIdConnect("oidc", opt =>
            //{
            //    opt.Authority = "https://danielitolozano85.us.auth0.com";
            //    opt.CallbackPath = new PathString("/siging-auth0");
            //    opt.RequireHttpsMetadata = false;
            //    opt.ClientId = "5Zq0iow9cLOzaj7Ornk3YLlA4XaNrrqx";
            //    opt.ClientSecret = "DA09PiSlduSK4LInnWbUqKwEJ_7iTnNDsBXjm5j2MyHuwRnZlrzFgmyTumLtDX4f";
            //    opt.SaveTokens = true;
            //    opt.ResponseType = "id_token code";
            //    opt.Scope.Add("socialnetwork");
            //    opt.Scope.Add("openid");
            //    opt.Scope.Add("offline_access");
            //    opt.SignInScheme = "Cookies";

            //});


            #endregion


            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
            //    await next();
            //});

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapDefaultControllerRoute();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}
