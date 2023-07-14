using FarmStationDb.DataLayer;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace FarmStation.DataLayer;

public static class AutoApplyDbMigrationsExtension
{
	public static void AutoApplyDbMigrations(this IHost host)
	{
		//https://stackoverflow.com/questions/36265827/entity-framework-automatic-apply-migrations/62225872#62225872

		using var scope = host.Services.CreateScope();
		var services = scope.ServiceProvider;

		var dbContext = services.GetRequiredService<FarmrContext>();
		dbContext.Database.Migrate(); 
	}
}
