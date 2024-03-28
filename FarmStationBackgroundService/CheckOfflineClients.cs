using FarmStation.Models.Db;
using FarmStationDb;
using FarmStationDb.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmStationBackgroundService;

public class CheckOfflineClients
{
    private readonly FarmerRepository _farmerRepository;
    private readonly NotificationRepository _notificationRepository;
	private readonly StatusesRepository _statusesRepository;

	public CheckOfflineClients(FarmerRepository farmerRepository
                               , NotificationRepository notificationRepository
                               , StatusesRepository statusesRepository)
    {
        _farmerRepository = farmerRepository;
        _notificationRepository = notificationRepository;
		_statusesRepository = statusesRepository;
	}

    public async Task CheckClientsAsync()
    {
        var farms = (await _farmerRepository.GetFarmsNotFarmingOrUpdatedSinceDateAsync(DateTime.UtcNow.AddMinutes(-120)))
            .Where(f => f.User != "none");
            //.Where(f => f.User == "josearaujof@gmail.com");

        if (!farms.Any() ) {
            Console.WriteLine($"All farms online!");
        }
        
        foreach (var farm in farms)
        {
            Console.WriteLine($"Found offline farm: {farm.Id}. (last updated {farm.LastUpdated})");
            await _statusesRepository.SetFarmStatusAsync(farm.Id, 2);
            
            if (!farm.LastStatusNotificationTimestamp.HasValue || ( 
                (DateTime.UtcNow - farm.LastStatusNotificationTimestamp.Value).TotalDays > 1
                && ((int)(DateTime.UtcNow - farm.LastStatusNotificationTimestamp.Value).TotalDays) % 3 == 0)
				)
            {
                var notification = new Notification
                {
                    User = farm.User,
                    Type = "offline",
                    Name = farm.Id
                };                
                await _notificationRepository.AddNotificationAsync(notification);
                //farm.LastOfflineNotification = DateTime.UtcNow;
                //farm.FarmingStatus = "OFFLINE";
                //await _farmerRepository.UpdateFarmAsync(farm); 
                await _farmerRepository.UpdateFarmStatusAsync(farm.Id, "OFFLINE", DateTime.UtcNow);
            }else if (farm.FarmingStatus != "OFFLINE")
			{
				await _farmerRepository.UpdateFarmStatusAsync(farm.Id, "OFFLINE", DateTime.UtcNow);
			}
		}
    }
	public async Task CheckFarmsChangedStatusAsync()
	{
		var farms = (await _farmerRepository.GetFarmsChangedStatusAsync(DateTime.UtcNow.AddMinutes(-120)))
			.Where(f => f.User != "none");
		//.Where(f => f.User == "josearaujof@gmail.com");

		if (!farms.Any())
		{
			Console.WriteLine($"No farms with changed status!");
		}

		foreach (var farm in farms)
		{
			Console.WriteLine($"Found changed status farm: {farm.Id} - {farm.LastNotificationFarmingStatus} => {farm.FarmingStatus}. (last updated {farm.LastUpdated})");

            string notType = "";
            switch(farm.FarmingStatus)
            {
                case "FARMING": notType ="started";
                    break;
                case "OFFLINE": notType = "offline";
                    break;
                default:
                    notType = "stopped";
                    break;
			}

			var notification = new Notification
			{
				User = farm.User,
				Type = notType,
				Name = farm.Id
			};
			await _notificationRepository.AddNotificationAsync(notification);
			//farm.LastOfflineNotification = DateTime.UtcNow;
			//farm.FarmingStatus = "OFFLINE";
			//await _farmerRepository.UpdateFarmAsync(farm); 
			await _farmerRepository.UpdateFarmStatusAsync(farm.Id, farm.FarmingStatus, DateTime.UtcNow);
		}
	}

}
