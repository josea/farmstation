﻿using FarmStation.Models.Db;
using FarmStationDb.DataLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmStationDb;

public class NotificationRepository
{
    private readonly IDbContextFactory<FarmrContext> _dbContextFactory;

    public NotificationRepository(IDbContextFactory<FarmrContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Notification>> GetNotificationsAsync()
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Notifications.ToListAsync();
    }

    public async Task RemoveNotification(Notification notification)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.Notifications.Remove(notification);

        await dbContext.SaveChangesAsync();
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        await dbContext.Notifications.AddAsync(notification);

        await dbContext.SaveChangesAsync();
    }

}
