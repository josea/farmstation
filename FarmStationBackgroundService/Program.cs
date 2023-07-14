// See https://aka.ms/new-console-template for more information



using FarmStationBackgroundService;
using FarmStationDb;
using FarmStationDb.DataLayer;
using FarmStationMessager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

public class Program
{
    private async static Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection();
        serviceProvider.AddDbContextFactory<FarmrContext>(options =>
            {
                var cnnStrBuilder = new MySqlConnector.MySqlConnectionStringBuilder(Configuration["ConnectionStrings:MariaDbConnectionString"]!);

                // define in powershell if env variable => $env: farmerstation_ConnectionStrings__dbuser = 'devuser'
                cnnStrBuilder.UserID = Configuration["ConnectionStrings:dbuser"];
                cnnStrBuilder.Password = Configuration["ConnectionStrings:dbpassword"];
                cnnStrBuilder.Server = Configuration["ConnectionStrings:dbserver"];
                cnnStrBuilder.ConvertZeroDateTime = true;
                options.UseMySql(cnnStrBuilder.ConnectionString, ServerVersion.Parse("10.6.11-mariadb"));
				options.ConfigureWarnings(wc => wc.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.BoolWithDefaultWarning));
			});
        //serviceProvider.AddDbContext<FarmrContext>(options =>
        //{
        //    var cnnStrBuilder = new MySqlConnector.MySqlConnectionStringBuilder(Configuration["ConnectionStrings:MariaDbConnectionString"]!);

        //    // define in powershell if env variable => $env: farmerstation_ConnectionStrings__dbuser = 'devuser'
        //    cnnStrBuilder.UserID = Configuration["ConnectionStrings:dbuser"];
        //    cnnStrBuilder.Password = Configuration["ConnectionStrings:dbpassword"];
        //    cnnStrBuilder.Server = Configuration["ConnectionStrings:dbserver"];
        //    cnnStrBuilder.ConvertZeroDateTime = true;
        //    options.UseMySql(cnnStrBuilder.ConnectionString, ServerVersion.Parse("10.6.11-mariadb"));
        //});
        serviceProvider.AddScoped<NotificationRepository>();
        serviceProvider.AddScoped<NotificationSender>();
        serviceProvider.AddSingleton(new NetworkCredential(Configuration["Email:user"], Configuration["Email:pwd"]));
        serviceProvider.AddScoped<FarmerRepository>();
        serviceProvider.AddScoped<StatusesRepository>();
        serviceProvider.AddScoped<CheckOfflineClients>();

        var services = serviceProvider.BuildServiceProvider();

		var offlineChecker = services.GetRequiredService<CheckOfflineClients>();
		await offlineChecker.CheckClientsAsync();

		var sender = services.GetRequiredService<NotificationSender>();
        await sender.SendNotificationsAsync();       
    }


    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? ""}.json", reloadOnChange: true, optional: true)
       .AddEnvironmentVariables(prefix: "farmstation_")
       .Build();
}