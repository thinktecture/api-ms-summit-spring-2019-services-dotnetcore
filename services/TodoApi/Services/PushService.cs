using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace TodoApi.Services
{
	public class PushService : BackgroundService
	{
		private readonly ILogger<PushService> _logger;
		private readonly IConfiguration _config;
		private readonly ILoggerFactory _loggerFactory;

		private CancellationToken _token;
		private HubConnection _connection;

		public PushService(ILogger<PushService> logger, IConfiguration config, ILoggerFactory loggerFactory)
		{
			_logger = logger;
			_config = config;
			_loggerFactory = loggerFactory;
		}

		public async Task SendListCreated(int listId, string listName)
		{
			_logger?.LogDebug($"{nameof(SendListCreated)}: {{ListId}}, {{ListName}}", listId, listName);
			if (await EnsureConnected(_token))
			{
				await _connection.SendAsync("ListAdded", listId, listName, _token);
			}
		}

		public async Task SendListRenamed(int listId, string newName)
		{
			_logger?.LogDebug($"{nameof(SendListRenamed)}: {{ListId}}, {{ListName}}", listId, newName);
			if (await EnsureConnected(_token))
			{
				await _connection.SendAsync("ListRenamed", listId, newName, _token);
			}
		}

		public async Task SendListDeleted(int listId)
		{
			_logger?.LogDebug($"{nameof(SendListDeleted)}: {{ListId}}", listId);
			if (await EnsureConnected(_token))
			{
				await _connection.SendAsync("ListDeleted", listId, _token);
			}
		}

		public async Task SendItemAdded(int listId, int itemId, string itemName)
		{
			_logger?.LogDebug($"{nameof(SendItemAdded)}: {{ListId}}, {{ItemId}}, {{ItemName}}", listId, itemId, itemName);
			if (await EnsureConnected(_token))
			{
				await _connection.SendAsync("ItemAdded", listId, itemId, itemName, _token);
			}
		}

		public async Task SendItemNameChanged(int listId, int itemId, string newName)
		{
			_logger?.LogDebug($"{nameof(SendItemNameChanged)}: {{ListId}}, {{ItemId}}, {{ItemName}}", listId, itemId, newName);
			if (await EnsureConnected(_token))
			{
				await _connection.SendAsync("ItemNameChanged", listId, itemId, newName, _token);
			}
		}

		public async Task SendItemDoneChanged(int listId, int itemId, bool done)
		{
			_logger?.LogDebug($"{nameof(SendItemDoneChanged)}: {{ListId}}, {{ItemId}}, {{Done}}", listId, itemId, done);
			if (await EnsureConnected(_token))
			{
				await _connection.SendAsync("ItemDoneChanged", listId, itemId, done, _token);
			}
		}

		public async Task SendItemDeleted(int listId, int itemId)
		{
			_logger?.LogDebug($"{nameof(SendItemDeleted)}: {{ListId}}, {{ItemId}}", listId, itemId);
			if (await EnsureConnected(_token))
			{
				await _connection.SendAsync("ItemDeleted", listId, itemId, _token);
			}
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_token = stoppingToken;
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			await base.StartAsync(cancellationToken);
			await Connect(cancellationToken);
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await _connection?.StopAsync();

			_connection = null;

			await base.StopAsync(cancellationToken);
		}

		private async Task<bool> EnsureConnected(CancellationToken token)
		{
			if (_connection != null)
				return true;

			await Connect(token);
			return (_connection != null);
		}

		private async Task Connect(CancellationToken cancellationtoken)
		{
			_logger?.LogDebug($"{nameof(Connect)}");
			var token = await GetToken(cancellationtoken);

			if (String.IsNullOrWhiteSpace(token))
				return;

			var connection = new HubConnectionBuilder()
				.ConfigureLogging(lb => lb.AddSerilog())
				.WithUrl($"{_config.GetSection("PushServer").GetValue<string>("Url")}/hubs/list?token=" + token)
				.Build();

			connection.Closed += async (e) => { _connection = null; };

			_logger?.LogDebug($"{nameof(Connect)}: Trying to connect with token {{Token}}", token);

			await connection.StartAsync(cancellationtoken).ConfigureAwait(false);
			_connection = connection;
		}

		private async Task<string> GetToken(CancellationToken token)
		{
			var client = new HttpClient();

			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials"),
				new KeyValuePair<string, string>("scopes", "pushapi"),
				new KeyValuePair<string, string>("client_id", _config.GetSection("IdentityServer").GetValue<string>("PushClientId")),
				new KeyValuePair<string, string>("client_secret", _config.GetSection("IdentityServer").GetValue<string>("PushClientSecret")),
			});

			var response = await client.PostAsync($"{_config.GetSection("IdentityServer").GetValue<string>("Url")}/connect/token", content, token);
			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				dynamic tokenResponse = JsonConvert.DeserializeObject(responseContent);

				return tokenResponse.access_token;
			}

			return null;
		}
	}
}
