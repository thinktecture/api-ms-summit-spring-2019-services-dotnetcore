using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PushApi.Hubs;

namespace PushApi
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

			services.AddCors();
			
			var idSrvConfig = Configuration.GetSection("IdentityServer");
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.Authority = idSrvConfig.GetValue<string>("Url");
					options.Audience = idSrvConfig.GetValue<string>("Audience");
					options.RequireHttpsMetadata = false; // Do NOT do this in Prod!!!

					options.Events = new JwtBearerEvents()
					{
						OnMessageReceived = context =>
						{
							if (context.Request.Path.Value.StartsWith("/hubs") && context.Request.Query.TryGetValue("token", out var token))
							{
								context.Token = token;
							}

							return Task.CompletedTask;
						}
					};
				});

			services.AddSignalR();
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
			
			app.UseCors(builder => builder
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials()
				.SetIsOriginAllowed(origin => true)
			);
			app.UseAuthentication();

			app.UseSignalR(routes => {
				routes.MapHub<ListHub>("/hubs/list");
			});
		}
	}
}
