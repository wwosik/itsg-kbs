
using ITSG.DFV.Common.EasyNetQBus;
using ITSG.KBS.Web;
using ITSG.KBS.Web.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o => { o.SingleLine = true; });
if (System.Diagnostics.Debugger.IsAttached) builder.Logging.AddDebug();


builder.Services.Configure<Timeouts>(builder.Configuration.GetSection("Timeouts"));
builder.Services.AddTransient<IBusInterceptor, BusInterceptor>();

builder.Services.AddSingleton(sp =>
{
    var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMQ");
    if (string.IsNullOrEmpty(rabbitConnectionString)) throw new Exception("Missing connection string RabbitMQ");

    var bus = EasyNetQ.RabbitHutch.CreateBus(rabbitConnectionString);
    sp.GetRequiredService<ILoggerFactory>().CreateLogger("PROGRAM").LogInformation("RabbitMQ bus established...");

    return bus;
});

builder.Services.AddSingleton<ITSG.DFV.Common.Interfaces.IBus, EasyNetQBusAdapter>();

builder.Services.AddControllers();

var app = builder.Build();

var requestCounter = 0;
app.Use(async (context, next) =>
{
    var myId = Interlocked.Increment(ref requestCounter);
    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    var httpLogger = loggerFactory.CreateLogger("HTTP");

    try
    {
        httpLogger.LogDebug("[{id}] {method} {url}", myId, context.Request.Method, context.Request.Path);
        await next();
        httpLogger.LogDebug("[{id}] {method} {url} DONE: {StatusCode}", myId, context.Request.Method, context.Request.Path, context.Response.StatusCode);
    }
    catch (Exception ex)
    {
        var exceptionLogger = loggerFactory.CreateLogger("EXCEPTION");
        exceptionLogger.LogError(ex, "[{id}] {method} {url}: {message}", myId, context.Request.Method, context.Request.Path, ex.GetBaseException().Message);
        throw;
    }
});

app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "Hello World!");


//app.HandleDevTimeResponses();

app.Run();
