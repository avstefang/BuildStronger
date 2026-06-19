using MApp.Models;
using SQLite;

namespace MApp.Services;

/// <summary>
/// Thin wrapper around the on-device SQLite database. The connection and tables
/// are created lazily on first use, so nothing touches the disk until a page
/// actually asks for cached data.
/// </summary>
public class LocalDbService
{
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_db is not null)
            return _db;

        // AppDataDirectory is the per-app private folder on every platform.
        var path = Path.Combine(FileSystem.AppDataDirectory, $"bs.db3");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<CachedSubscriptionPlan>();
        await _db.CreateTableAsync<MemberProfile>();
        return _db;
    }

    public async Task<List<CachedSubscriptionPlan>> GetSubscriptionPlansAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<CachedSubscriptionPlan>().ToListAsync();
    }

    public async Task<MemberProfile?> GetAthleteAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<MemberProfile>().FirstOrDefaultAsync();
    }

    /// <summary>Replaces the cached plans with a fresh copy from the API.</summary>
    public async Task SaveSubscriptionPlansAsync(IEnumerable<CachedSubscriptionPlan> plans)
    {
        var db = await GetConnectionAsync();
        await db.DeleteAllAsync<CachedSubscriptionPlan>();
        await db.InsertAllAsync(plans);
    }

    /// <summary>Replaces the cached plans with a fresh copy from the API.</summary>
    public async Task SaveAthleteAsync(MemberProfile athlete)
    {
        var db = await GetConnectionAsync();
        await db.DeleteAllAsync<MemberProfile>();
        await db.InsertAsync(athlete);
    }
}
