using Battleships;
using Battleships.Services;
using Battleships.Services.Interfaces;
using NLog;
using NLog.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        try
        {
            logger.Debug("init main");
            
            var builder = CreateBuilder(args);
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

    public static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Settings
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

        // Definition service
        builder.Services.AddSingleton<IShipsDefinitionService, ShipsDefinitionService>();

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Logger
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        return builder;
    }
}
