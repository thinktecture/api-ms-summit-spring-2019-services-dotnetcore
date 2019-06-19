using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using TodoApi.Database;

namespace TodoApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var host = CreateWebHostBuilder(args).Build();

				var config = host.Services.GetService<IConfiguration>();
				var lls = new LoggingLevelSwitch(LogEventLevel.Debug);

				var loggerConfig = new LoggerConfiguration()
					.Enrich.FromLogContext()
					.Enrich.WithThreadId()
					.Enrich.WithThreadName()
					.Enrich.WithProcessId()
					.Enrich.WithProcessName()
					.Enrich.WithMachineName()
					.Enrich.WithProperty("Application", "ToDo API")
					.ReadFrom.Configuration(config)
					.MinimumLevel.ControlledBy(lls)
					.WriteTo.Console()
					.WriteTo.Seq("http://localhost:5341", apiKey: "dLwlJ0CLnjAkdUDL1w1Y", controlLevelSwitch: lls);

				Log.Logger = loggerConfig.CreateLogger();

				using (var scope = host.Services.CreateScope())
				{
					var dbCtx = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
					dbCtx.Database.Migrate();
				}

				host.Run();
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error caused service to crash: " + ex.Message);
			}
			finally
			{
				Log.CloseAndFlush();
			}

		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseSerilog()
				.UseStartup<Startup>();
	}
}
