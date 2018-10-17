using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services
                .AddIdentityServer()
                .AddInMemoryClients(GetClients())
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddTestUsers(GetTestUsers())
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()); // Don't use in production
            app.UseIdentityServer();
        }

        private IEnumerable<Client> GetClients()
        {
            yield return new Client()
            {
                ClientId = "guiclient",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = new List<Secret>()
                {
                    new Secret("guisecret".Sha256()), // Don't use in production, get it from e.g. environment 
                },
                AllowedScopes = new List<string>()
                {
                    "api1",
                    "pushapi",
                },
            };
            
            yield return new Client()
            {
                ClientId = "todoclient",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret>()
                {
                    new Secret("todosecret".Sha256()), // Don't use in production, get it from e.g. environment
                },
                AllowedScopes = new List<string>()
                {
                    "pushapi",
                },
            };
        }

        private IEnumerable<ApiResource> GetApiResources()
        {
            yield return new ApiResource()
            {
                Name = "api1",
                Scopes = new List<Scope>()
                {
                    new Scope() { Name = "api1" },
                }
            };

            yield return new ApiResource()
            {
                Name = "pushapi",
                Scopes = new List<Scope>()
                {
                    new Scope() { Name = "pushapi" },
                }
            };
        }

        private IEnumerable<IdentityResource> GetIdentityResources()
        {
            yield return new IdentityResources.OpenId();
            yield return new IdentityResources.Profile();
            yield return new IdentityResources.Email();
        }

        private List<TestUser> GetTestUsers()
        {
            return new List<TestUser>()
            {
                new TestUser()
                {
                    SubjectId = "affeaffe-dead-beef-affe-affeaffeaffe",
                    Username = "Alice",
                    Password = "alice",
                    Claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.Email, "alice@dotnetsummit.de"),
                    },
                },
                new TestUser()
                {
                    SubjectId = "affeaffe-dead-beef-dead-affeaffeaffe",
                    Username = "Bob",
                    Password = "bob",
                    Claims = new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.Email, "bob@dotnetsummit.de"),
                    },
                },
            };
        }
    }
}
