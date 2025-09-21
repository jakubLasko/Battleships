using Battleships;
using Battleships.Services;
using Battleships.Services.Interfaces;
using Battleships.Storages;
using Battleships.Storages.Interfaces;
using NLog;
using NLog.Web;

public class Program
{
    /// <summary>
    /// The entry point of the application. Configures and starts the web application.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        try
        {
            logger.Debug("init main");
            
            var builder = WebApplication.CreateBuilder(args);

            // Settings
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

            // Definition service (loads ship templates from configuration)
            builder.Services.AddSingleton<IShipsDefinitionService, ShipsDefinitionService>();

            // Battleships service (main service to handle game logic)
            builder.Services.AddSingleton<IBattleshipsService, BattleshipsService>();

            // In-memory game storage
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IGameStorage, GameStorage>();

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Logger
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
        catch (Exception exception)
        {
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit
            LogManager.Shutdown();
        }
    }
}
