using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddCors();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetSection("IdentityServer").GetValue<string>("Url");
                    options.Audience = Configuration.GetSection("IdentityServer").GetValue<string>("Audience");
                    options.RequireHttpsMetadata = false; // Never do this in production
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Path.Value.StartsWith("/hubs"))
                            {
                                // Compatibility version with old @aspnet/signalr client side lib
                                if (context.Request.Query.TryGetValue("token", out var token))
                                {
                                    context.Token = token;
                                }
                                
                                // New SignalR client-side library
                                if (context.Request.Query.TryGetValue("access_token", out var accessToken))
                                {
                                    context.Token = accessToken;
                                }
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
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(s => true)
                .AllowCredentials()
            ); // Do NOT do this in production
            app.UseAuthentication();
            app.UseSignalR(routes => { routes.MapHub<ListHub>("/hubs/list"); });
        }
    }
}