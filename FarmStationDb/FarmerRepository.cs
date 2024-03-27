using FarmStation.Models;
using FarmStationDb.Models.FarmDataJson;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FarmStationDb.DataLayer;

public class FarmerRepository
{
    private readonly IDbContextFactory<FarmrContext> _dbContextFactory;

    public FarmerRepository(IDbContextFactory<FarmrContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Balance>> GetBalancesAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Balances.ToListAsync();
    }

    public async Task<FarmData> GetFarmDataAsync(string farmId, string userId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        Farm farm = await dbContext.Farms
            .FirstAsync(f => f.User.ToLower() == userId.ToLower() && f.Id == farmId);

        var farmData = Deserialize(farm.Id, farm.Data, options);

        return farmData;
    }

    public async Task<List<FarmData>> GetFarmsDataAsync(string userId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var farmData = await dbContext.Farms
            .Where(f => f.User.ToLower() == userId.ToLower() && !string.IsNullOrEmpty(f.Data))
            .Select(farm => Deserialize(farm.Id, farm.Data, options))
            .ToListAsync();

        return farmData;
    }

    //public async Task UpdateFarmAsync(Farm farm)	
    //{
    //	//This doesn't work because the context is not tracking changes
    //	using var dbContext = _dbContextFactory.CreateDbContext();


    //	var entry = dbContext.Entry(farm);

    //	foreach (var prop in entry.Properties
    //		.Where(prop => prop.IsModified)
    //		.Select(prop => new
    //		{
    //			Property = prop.Metadata,
    //			Value = prop.CurrentValue,
    //			OldValue = prop.OriginalValue
    //		}))
    //	{
    //		Console.WriteLine($"{prop.Property.Name}: {prop.OldValue} => {prop.Value}");
    //	}

    //	await dbContext.SaveChangesAsync();
    //   }

    public async Task UpdateFarmStatusAsync(string farmId,
                    string farmingStatus,
                    DateTime LastOfflineNotification)
    {
        // https://stackoverflow.com/questions/3642371/how-to-update-only-one-field-using-entity-framework
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.Farms
            .Where(f => f.Id == farmId)
            .ExecuteUpdateAsync(b => b
                .SetProperty(f => f.FarmingStatus, farmingStatus)
                .SetProperty(f => f.LastStatusNotificationTimestamp, LastOfflineNotification)
                .SetProperty(f => f.LastNotificationFarmingStatus, farmingStatus) 
                //.SetProperty(f => f.LastUpdated, DateTime.UtcNow)
                .SetProperty(f => f.FarmingStatusTimestamp, DateTime.UtcNow)
            ); 
    }

    public async Task<List<Farm>> GetFarmsNotFarmingOrUpdatedSinceDateAsync(DateTime dateTimeUtc, bool includeOnlyFarmsEnabledForOfflineNotification = true)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var farmData = await dbContext.Farms
            .Where(f => 
                (f.LastUpdated < dateTimeUtc && (!includeOnlyFarmsEnabledForOfflineNotification || f.NotifyWhenOffline))
                || ((f.LastFarming ?? f.LastUpdated ) < dateTimeUtc)                
                )
            .ToListAsync();

        return farmData;
    }
	public async Task<List<Farm>> GetFarmsChangedStatusAsync(DateTime dateTimeUtc, bool includeOnlyFarmsEnabledForOfflineNotification = true)
	{
		using var dbContext = _dbContextFactory.CreateDbContext();

		var farmData = await dbContext.Farms
			.Where(f =>
				 (f.FarmingStatus != f.LastNotificationFarmingStatus)
				)
			.ToListAsync();

		return farmData;
	}

	private static FarmData Deserialize(string id, string data, JsonSerializerOptions options)
    {
        var farmData = JsonSerializer.Deserialize<FarmData>(data, options)!;
        farmData.Id = id;
        return farmData;
    }
}
