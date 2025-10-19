using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FieldLead.Mobile.Data;
using FieldLead.Mobile.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Networking;

namespace FieldLead.Mobile.Services;

public class SyncService
{
    private readonly LeadRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;
    private readonly IConnectivity _connectivity;
    private readonly ILogger<SyncService> _logger;

    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

    public SyncService(LeadRepository repository, IHttpClientFactory httpClientFactory, AuthService authService, IConnectivity connectivity, ILogger<SyncService> logger)
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
        _authService = authService;
        _connectivity = connectivity;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await _repository.InitializeAsync();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!IsOnline())
                {
                    _logger.LogDebug("Sync skipped - offline");
                    await Task.Delay(PollInterval, cancellationToken);
                    continue;
                }

                await PushOutboxAsync(cancellationToken);

                // TODO: add pull sync for sites/leads when API is available
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running sync loop");
            }

            await Task.Delay(PollInterval, cancellationToken);
        }
    }

    private bool IsOnline()
    {
        var access = _connectivity.NetworkAccess;
        return access == NetworkAccess.Internet || access == NetworkAccess.ConstrainedInternet;
    }

    private async Task PushOutboxAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("api");
        var token = await _authService.GetAccessTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var nextItem = await _repository.PeekOutboxAsync();
        if (nextItem is null)
        {
            return;
        }

        _logger.LogInformation("Outbox item {Id} ready for upload", nextItem.Id);

        // TODO: implement multipart upload to /leads endpoint
        _logger.LogInformation("TODO: upload payload {Type} ({EntityId})", nextItem.EntityType, nextItem.EntityId);
    }
}
