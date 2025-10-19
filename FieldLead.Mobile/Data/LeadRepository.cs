using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FieldLead.Mobile.Models;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;
using SQLite;

namespace FieldLead.Mobile.Data;

public class LeadRepository
{
    private readonly string _databasePath;
    private readonly Lazy<SQLiteAsyncConnection> _connectionLazy;
    private bool _initialized;

    public LeadRepository(IFileSystem fileSystem)
    {
        _databasePath = Path.Combine(fileSystem.AppDataDirectory, "fieldlead.db3");
        _connectionLazy = new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache));
    }

    private SQLiteAsyncConnection Connection => _connectionLazy.Value;

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await Connection.CreateTableAsync<Site>();
        await Connection.CreateTableAsync<Lead>();
        await Connection.CreateTableAsync<LeadPhoto>();
        await Connection.CreateTableAsync<OutboxItem>();

        _initialized = true;
    }

    public async Task<IReadOnlyList<Site>> GetNearbySitesAsync(Location location, double radiusMeters = 1000)
    {
        await InitializeAsync();

        // simple bounding box filter before doing distance calculations
        var latitudeDelta = radiusMeters / 111_320d;
        var longitudeDelta = radiusMeters / (111_320d * Math.Cos(location.Latitude * Math.PI / 180));

        var minLat = location.Latitude - latitudeDelta;
        var maxLat = location.Latitude + latitudeDelta;
        var minLon = location.Longitude - longitudeDelta;
        var maxLon = location.Longitude + longitudeDelta;

        var sites = await Connection.Table<Site>()
            .Where(s => s.Latitude >= minLat && s.Latitude <= maxLat && s.Longitude >= minLon && s.Longitude <= maxLon)
            .ToListAsync();

        return sites;
    }

    public Task<int> UpsertSiteAsync(Site site) => Connection.InsertOrReplaceAsync(site);

    public Task<int> UpsertLeadAsync(Lead lead) => Connection.InsertOrReplaceAsync(lead);

    public Task<int> InsertLeadPhotoAsync(LeadPhoto photo) => Connection.InsertAsync(photo);

    public Task<int> EnqueueOutboxItemAsync(OutboxItem item) => Connection.InsertAsync(item);

    public Task<OutboxItem?> PeekOutboxAsync() => Connection.Table<OutboxItem>().OrderBy(o => o.CreatedAt).FirstOrDefaultAsync();

    public Task<int> DeleteOutboxItemAsync(int id) => Connection.DeleteAsync<OutboxItem>(id);

    public async Task<IReadOnlyList<LeadPhoto>> GetLeadPhotosAsync(string leadId)
    {
        await InitializeAsync();
        return await Connection.Table<LeadPhoto>().Where(p => p.LeadId == leadId).ToListAsync();
    }
}
