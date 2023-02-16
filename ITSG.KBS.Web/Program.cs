
using ITSG.DFV.Common.EasyNetQBus;
using ITSG.KBS.Web;
using ITSG.KBS.Web.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
builder.Configuration.AddEnvironmentVariables();

builder.UseSerilog();

builder.Services.Configure<Timeouts>(builder.Configuration.GetSection("Timeouts"));

builder.Services.AddRabbitMQ("RabbitMQ");

builder.Services.AddSingleton<ITSG.DFV.Common.Interfaces.IBus, EasyNetQBusAdapter>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseForwardedHeaders(new() { ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All });

var requestCounter = 0;
app.UseWhen(hc => hc.Request.Path.StartsWithSegments("/api"), a => a.Use(async (context, next) =>
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
}));

app.UseRouting();
app.MapControllers();

app.UseWhen(hc => !hc.Request.Path.StartsWithSegments("/api"), spaApp => spaApp.UseSpa(o =>
{
    o.UseProxyToSpaDevelopmentServer("http://localhost:4200");
}));

app.MapGet("*", async context => { context.Response.StatusCode = 404; await context.Response.WriteAsync("Webseite nicht verfügbar."); });


app.Run();
