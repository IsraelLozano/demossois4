using dic.sso.webapp.clienteweb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICompanyHttpClient, CompanyHttpClient>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = "Cookies";
                opt.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", opt =>
            {
                opt.AccessDeniedPath = "/Account/AccessDenied";
            })
            .AddOpenIdConnect("oidc", opt =>
             {
                 opt.SignInScheme = "Cookies";
                 opt.Authority = "http://localhost:5001";
                 opt.ClientId = "mvc-client";
                 opt.ResponseType = "code id_token";
                 opt.SaveTokens = true;
                 opt.ClientSecret = "MVCSecret";
                 opt.RequireHttpsMetadata = false;
                 opt.GetClaimsFromUserInfoEndpoint = true;

                 opt.ClaimActions.DeleteClaim("sid");
                 opt.ClaimActions.DeleteClaim("idp");
                 // or
                 //opt.ClaimActions.DeleteClaims(new string[] { "sid", "idp" });
                 opt.Scope.Add("address");
                 //opt.Scope.Add("email");
                 //opt.ClaimActions.MapUniqueJsonKey("email", "email");

                 opt.Scope.Add("roles");
                 opt.ClaimActions.MapUniqueJsonKey("role", "role");
                 opt.TokenValidationParameters = new TokenValidationParameters
                 {
                     RoleClaimType = "role",
                     NameClaimType = "email"
                 };

                 opt.Scope.Add("companyApi");
                 opt.Scope.Add("position");
                 opt.Scope.Add("country");
                 opt.ClaimActions.MapUniqueJsonKey("position", "position");
                 opt.ClaimActions.MapUniqueJsonKey("country", "country");

             });


            services.AddAuthorization(authOpt =>
            {
                authOpt.AddPolicy("CanCreateAndModifyData", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireClaim("position", "Administrator");
                    policyBuilder.RequireClaim("country", "USA");
                });
            });

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
            }
            app.UseStaticFiles();

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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
