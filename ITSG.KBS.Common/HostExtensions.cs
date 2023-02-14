using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;

namespace ITSG.KBS;

public static class HostExtensions
{
    public static void UseSerilog(this HostApplicationBuilder builder)
    {
        UseSerilog(builder.Configuration, builder.Services);
    }

    public static void UseSerilog(this WebApplicationBuilder builder)
    {
        UseSerilog(builder.Configuration, builder.Services);
    }

    private static void UseSerilog(IConfiguration configuration, IServiceCollection services)
    {
        var serilogLogger = new Serilog.LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(serilogLogger, true));
        services.AddSingleton<IDiagnosticContext>(services => new DiagnosticContext(serilogLogger));
    }

    public static void AddRabbitMQ(this IServiceCollection services, string connectionStringName)
    {
        services.AddSingleton(sp =>
        {
            var rabbitConnectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString(connectionStringName);
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("RABBITMQ");
            if (string.IsNullOrEmpty(rabbitConnectionString))
            {
                logger.LogCritical("Missing connection string '{connectionStringName}'", connectionStringName);
                throw new Exception($"Missing connection string '{connectionStringName}'.");
            }

            var bus = EasyNetQ.RabbitHutch.CreateBus(rabbitConnectionString, serviceRegister =>
            {
                serviceRegister.EnableSystemTextJson();
            });
            logger.LogInformation("RabbitMQ bus established...");

            return bus;
        });
    }
}