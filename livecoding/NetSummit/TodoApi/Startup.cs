using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using TodoApi.Database;
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

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			var idSrvConfig = Configuration.GetSection("IdentityServer");
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.Authority = idSrvConfig.GetValue<string>("Url");
					options.Audience = idSrvConfig.GetValue<string>("Audience");
					options.RequireHttpsMetadata = false; // Do NOT do this in Prod!!!
				});

			services.AddResponseCompression();

			services.AddDbContext<TodoDbContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("sqlserver"));
				options.EnableSensitiveDataLogging();
			});

			services.AddScoped<TodoService>();

			services
				.AddMemoryCache()
				.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"))
				.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"))
				.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>()
				.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

			services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new Info() {
					Title = "TT Todo API",
					Version = "v1",
				});

				c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "TodoApi.xml"));

				c.AddSecurityDefinition("oauth2", new OAuth2Scheme() { 
					Type = "oauth2",
					Flow = "password",
					TokenUrl = $"{idSrvConfig.GetValue<string>("Url")}/connect/token",
					Scopes = new Dictionary<string, string>()
					{ 
						{ "todoapi", "Access the Todo Api" }
					}
				});
				c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>()
				{
					{
						"oauth2", new []{ "todoapi" }
					}
				});
			});

			services.Configure<PushServerSettings>(Configuration.GetSection("PushServer"));
			services.Configure<IdentityServerSettings>(Configuration.GetSection("IdentityServer"));

			services.AddSingleton<PushService>();
			services.AddSingleton<IPushService>(ctx => ctx.GetRequiredService<PushService>());
			services.AddSingleton<IHostedService>(ctx => ctx.GetRequiredService<PushService>());
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
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHttpsRedirection();
				app.UseHsts();
			}

			app.UseIpRateLimiting();
			app.UseCors(builder => builder
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials()
				.SetIsOriginAllowed(origin => true)
			);

			app.UseResponseCompression();
			app.UseAuthentication();
			app.UseMvc();

			app.UseSwagger();
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "TT Todo Api");
				c.OAuthClientId("guiclient");
				c.OAuthClientSecret("guisecret");
				c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
				});
		}
	}
}
