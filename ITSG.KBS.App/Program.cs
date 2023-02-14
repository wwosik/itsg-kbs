using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
builder.Configuration.AddEnvironmentVariables();

var serilogLogger = new Serilog.LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(serilogLogger, true));
builder.Services.AddSingleton<IDiagnosticContext>(services => new DiagnosticContext(serilogLogger));

builder.Logging.ClearProviders();
if (System.Diagnostics.Debugger.IsAttached) builder.Logging.AddDebug();


builder.Services.AddSingleton(sp =>
{
    var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMQ");
    if (string.IsNullOrEmpty(rabbitConnectionString)) throw new Exception("Missing connection string RabbitMQ");

    var bus = EasyNetQ.RabbitHutch.CreateBus(rabbitConnectionString, serviceRegister =>
    {
        serviceRegister.EnableSystemTextJson();
    });
    sp.GetRequiredService<ILoggerFactory>().CreateLogger("PROGRAM").LogInformation("RabbitMQ bus established...");

    return bus;
});

builder.Services.AddSingleton<Processors>();

var cancelSource = new CancellationTokenSource();

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("PROGRAM");

host.Services.GetRequiredService<Processors>().EnableProcessors(cancelSource.Token);

Console.CancelKeyPress += (s, e) =>
{
    logger.LogInformation("Program stopping...");
    cancelSource.Cancel();
    host.StopAsync();
};

host.Run();


