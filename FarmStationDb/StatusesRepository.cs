using FarmStationDb.DataLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmStationDb;

public class StatusesRepository
{
	private readonly IDbContextFactory<FarmrContext> _dbContextFactory;

	public StatusesRepository(IDbContextFactory<FarmrContext> dbContextFactory)
	{
		_dbContextFactory = dbContextFactory;
	}

	public async Task SetFarmStatusAsync(string farmId, int status)
	{
		// https://stackoverflow.com/questions/3642371/how-to-update-only-one-field-using-entity-framework
		using var dbContext = _dbContextFactory.CreateDbContext();
		await dbContext.Statuses
			.Where(f => f.Id == farmId)
			.ExecuteUpdateAsync(b => b
				.SetProperty(s => s.Isfarming , status)
			);
	}
}
