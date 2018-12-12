using System;
using System.Collections.Generic;
using System.IO;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using TodoApi.Models;
using TodoApi.Services;
using TodoApi.Settings;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace TodoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IdentityServerSettings>(Configuration.GetSection("IdentityServer"));
            services.Configure<PushServerSettings>(Configuration.GetSection("PushServer"));
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetSection("IdentityServer").GetValue<string>("Url");
                    options.Audience = "todoapi";
                    options.RequireHttpsMetadata = false; // do not do this in production!
                });

            services
                .AddMemoryCache()
                .Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"))
                .Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"))
                .AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>()
                .AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>()
                .AddResponseCompression()
                .AddScoped<TodoService>()
                .AddDbContext<TodoContext>(options => options.UseSqlServer(Configuration.GetConnectionString("sqlserver")))
                .AddSingleton<PushService>()
                .AddSingleton<IPushService>(ctx => ctx.GetRequiredService<PushService>())
                .AddSingleton<IHostedService>(ctx => ctx.GetRequiredService<PushService>());
            ;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info()
                {
                    Title = "TT Todo Api",
                    Version = "v1",
                });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "TodoApi.xml"));
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme()
                {
                    Type = "oauth2",
                    Flow = "password",
                    TokenUrl = $"{Configuration.GetSection("IdentityServer").GetValue<string>("Url")}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "todoapi", "Access the Todo APi" },
                    },
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>()
                {
                    {
                        "oauth2", new[] { "todoapi" }
                    },
                });
            });

            services.AddMvc(options => { options.RespectBrowserAcceptHeader = true; })
                .AddXmlDataContractSerializerFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<IdentityServerSettings> settingsAccessor)
        {
            var settings = settingsAccessor.Value;
            
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                // In Production, use something more resilient
                scope.ServiceProvider.GetRequiredService<TodoContext>().Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials()); // Do NOT do this in production
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TT Todo API v1");
                c.OAuthClientId(settings.SwaggerClientId);
                c.OAuthClientSecret(settings.SwaggerClientSecret);
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });
        }
    }
}
