using dic.sso.identityserver.oauth.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace dic.sso.identityserver.oauth
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public IConfigurationRoot miConfigRoot { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        //public Startup(IConfiguration  configuration)
        //{
        //    this.configuration = configuration;
        //    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        //}
        /*
         PM> Add-Migration InitialPersistedGranMigration -c PersistedGrantDbContext -o Migrations/IdentityServer/PersistedGrantDb
         PM> Add-Migration InitialConfigurationMigration -c ConfigurationDbContext -o Migrations/IdentityServer/ConfigurationDb
         */
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //services.AddIdentityServer(options =>
            //{
            //    options.Csp.AddDeprecatedHeader = true;
            //})
            services.AddIdentityServer()
                .AddDeveloperSigningCredential() //not something we want to use in a production environment;
                .AddTestUsers(InMemoryConfig.GetUsers())
                //.AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
                //.AddInMemoryClients(InMemoryConfig.GetClients())
                //.AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
                //.AddInMemoryApiResources(InMemoryConfig.GetApiResources())
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = c => c.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                        sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = o => o.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                        sql => sql.MigrationsAssembly(migrationAssembly));
                });



            services.AddControllersWithViews();

        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Otra opcion
            //MigrateInMemoryDataToSqlServer(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
            //    await next();
            //});
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();


            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }


        public void MigrateInMemoryDataToSqlServer(IApplicationBuilder app)
        {

        }

    }
}
