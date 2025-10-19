using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FieldLead.Mobile.Services;

public class SyncBackgroundService : BackgroundService
{
    private readonly SyncService _syncService;
    private readonly ILogger<SyncBackgroundService> _logger;

    public SyncBackgroundService(SyncService syncService, ILogger<SyncBackgroundService> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sync background service starting");
        await _syncService.RunAsync(stoppingToken);
        _logger.LogInformation("Sync background service stopping");
    }
}
