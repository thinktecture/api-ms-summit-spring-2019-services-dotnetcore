using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace TodoApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var host = CreateWebHostBuilder(args).Build())
			{
				var config = host.Services.GetService<IConfiguration>();

				var loggerConfig = new LoggerConfiguration()
					.Enrich.FromLogContext()
					.Enrich.WithThreadId()
					.Enrich.WithProcessId()
					.Enrich.WithProcessName()
					.Enrich.WithAssemblyName()
					.Enrich.WithAssemblyVersion()
					.Enrich.WithMachineName()
					.Enrich.WithProperty("Application", "TodoApi")
					.ReadFrom.Configuration(config)
					.WriteTo.Console()
					.WriteTo.Seq("http://localhost:5341");

				Log.Logger = loggerConfig.CreateLogger();

				try
				{
					host.Run();
				}
				finally
				{
					Log.CloseAndFlush();
				}
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSerilog();
	}
}
