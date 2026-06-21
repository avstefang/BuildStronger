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
        await _db.CreateTableAsync<CachedGymClass>();
        await _db.CreateTableAsync<CachedBooking>();
        return _db;
    }

    public async Task<List<CachedGymClass>> GetLessonsAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<CachedGymClass>().ToListAsync();
    }

    /// <summary>Replaces the cached planning lessons with a fresh copy from the API.</summary>
    public async Task SaveLessonsAsync(IEnumerable<CachedGymClass> lessons)
    {
        var db = await GetConnectionAsync();
        await db.DeleteAllAsync<CachedGymClass>();
        await db.InsertAllAsync(lessons);
    }

    public async Task<List<CachedBooking>> GetBookingsAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<CachedBooking>().ToListAsync();
    }

    /// <summary>Replaces the cached bookings with a fresh copy from the API.</summary>
    public async Task SaveBookingsAsync(IEnumerable<CachedBooking> bookings)
    {
        var db = await GetConnectionAsync();
        await db.DeleteAllAsync<CachedBooking>();
        await db.InsertAllAsync(bookings);
    }

    /// <summary>Drops the cached bookings so the next load fetches fresh (after a book or cancel).</summary>
    public async Task ClearBookingsAsync()
    {
        var db = await GetConnectionAsync();
        await db.DeleteAllAsync<CachedBooking>();
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

    public async Task ClearAthleteAsync()
    {
        var db = await GetConnectionAsync();
        await db.DeleteAllAsync<MemberProfile>();
    }
}
