using Blazored.LocalStorage;
using FarmStation.DataLayer;
using FarmStation.Services;
using FarmStationDb.DataLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;

namespace FarmStation;

public class Program
{
	public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
	   .SetBasePath(Directory.GetCurrentDirectory())
	   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	   .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? ""}.json", reloadOnChange: true, optional: true)
	   .AddEnvironmentVariables(prefix: "farmstation_")
	   .Build();

	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);


		// Add services to the container.
		//builder.Services.AddDbContext<FarmrContext>(options =>
		//{
		//    options.UseMySql(Configuration["ConnectionStrings:MariaDbConnectionString"],
		//    ServerVersion.Parse("10.6.11-mariadb"));
		//}, ServiceLifetime.Transient);
		//https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#avoiding-dbcontext-threading-issues
		// it was throwing this exception, so changed to transient per MSFT's recommendation.
		// System.InvalidOperationException: A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913.

		//https://stackoverflow.com/questions/58046008/blazor-a-second-operation-started-on-this-context-before-a-previous-operation-co

		builder.Services.AddDbContextFactory<FarmrContext>(options =>
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


		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();
		builder.Services.AddSingleton<WeatherForecastService>();

		// this seemed to not work
		//builder.Services.AddTransient<FarmerService>(); // transient to avoid the 2 concurrent calls to dbContext.
		builder.Services.AddScoped<FarmerRepository>();
		builder.Services.AddScoped<FarmerProvider>();
		builder.Services.AddScoped<FiatService>();
		builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
		builder.Services.AddBlazoredLocalStorage();
		builder.Services.AddAutoMapper(typeof(Program));
		builder.Services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders =
				ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
			//ForwardedHeaders.All;
		});


		//https://learn.microsoft.com/en-us/answers/questions/830958/how-to-implement-google-authetication-in-blazor-se
		builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
			//options =>
			//{
			//	options.CookieManager = new ChunkingCookieManager(); 
			//	options.Cookie.HttpOnly = true;
			//	options.Cookie.SameSite = SameSiteMode.None;
			//	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
				
			//}
			);

	//https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/?view=aspnetcore-7.0&tabs=visual-studio
		builder.Services.AddAuthentication().AddGoogle(options =>
		{
			options.ClientId = Configuration["Google:ClientId"]!;
			options.ClientSecret = Configuration["Google:ClientSecret"]!;

			//options.Events.OnTicketReceived = async context =>
			//{
			//	var host = context.HttpContext.Request.Host.Host;
			//	var forwardedHost = context.HttpContext.Request.Headers["X-Forwarded-Host"].ToString();
			//	if (!string.IsNullOrEmpty(forwardedHost))
			//	{
			//		context.ReturnUri = context.ReturnUri.Replace(host, forwardedHost);
			//	}
			//	await Task.FromResult(0);
			//};
			//options.CallbackPath = "https://jiaf.zapto.org/signin-google";
			//https://stackoverflow.com/questions/62559600/set-openid-connect-callbackpath-to-https			
			//options.Events.OnRedirectToAuthorizationEndpoint = async (context) =>
			//{
			//	context.RedirectUri = "https://jiaf.zapto.org/signin-google";
			
				
			//	await Task.FromResult(0);
			//};
			//options.Events = new OpenIdConnectEvents();
			options.ClaimActions.MapJsonKey("urn:google:profile", "link");
			options.ClaimActions.MapJsonKey("urn:google:image", "picture");
			
		});

		builder.Services.AddHttpContextAccessor();
		builder.Services.AddScoped<HttpContextAccessor>();
		builder.Services.AddHttpClient();
		builder.Services.AddScoped<HttpClient>();
		builder.Services.AddScoped<TokenProvider>();
		builder.Services.AddLazyCache();
		builder.Services.AddHealthChecks();

		var app = builder.Build();

		app.AutoApplyDbMigrations(); 

		//https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-7.0
		// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-7.0&tabs=linux-ubuntu#configure-nginx
		app.Use((context, next) =>
		{
			context.Request.Scheme = "https";
			return next(context);
		});
		app.UseForwardedHeaders(); // we should only need this to the reverse proxy nginx, but I was forced to add the https
		// middleware above. 

		//app.Use((context, next) =>
		//{
		//	context.Request.Scheme = "https";
		//	context.Request.Host = new HostString("jiaf.zapto.org");
		//	return next();
		//});

		//app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		//app.UseHttpsRedirection();

		app.UseStaticFiles();
		app.UseCookiePolicy();

		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();

		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");
		app.MapHealthChecks("/health"); 
		

		app.Run();
	}
}