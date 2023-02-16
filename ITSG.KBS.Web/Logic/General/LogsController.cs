using System.Text.Json.Nodes;
using ITSG.KBS.Messages.Users;
using ITSG.KBS.Web.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ITSG.KBS.Web;

public class LogsController : Controller
{
    private readonly ILogger logger;

    public LogsController(ILoggerFactory loggerFactory)
    {
        this.logger = loggerFactory.CreateLogger("ClientSide");
    }

    [Route("/api/logs")]
    [HttpPost]
    public void AcceptLogs([FromBody] ClientLogEntry[] entries)
    {
        foreach (var entry in entries)
        {
            var level = entry.GetLogLevel();
            this.logger.Log(level, "{message}", entry.Message);
            if (entry.Items != null)
            {
                foreach (var item in entry.Items)
                {
                    this.logger.Log(level, "{value}", item.ToJsonString());
                }
            }
            
        }
    }
}

public class ClientLogEntry
{
    public ClientLogLevel Level { get; set; }
    public string? Message { get; set; }
    public JsonObject[]? Items { get; set; }

    public LogLevel GetLogLevel()
    {
        return this.Level switch
        {
            ClientLogLevel.Debug => LogLevel.Debug,
            ClientLogLevel.Info => LogLevel.Information,
            ClientLogLevel.Warn => LogLevel.Warning,
            ClientLogLevel.Error => LogLevel.Error,
            _ => LogLevel.Information
        };
    }
}

public enum ClientLogLevel
{
    Debug = 10,
    Info = 20,
    Warn = 30,
    Error = 40
}

