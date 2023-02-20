using Microsoft.AspNetCore.Mvc;
using ITSG.KBS.Messages.Config;
using Microsoft.Extensions.Options;
using ITSG.KBS.Web.Config;

namespace ITSG.KBS.Web;

public class ConfigController : Controller
{
    private readonly ILogger logger;
    private readonly IBus bus;
    private readonly Timeouts timeouts;
    private static ConfigItemValue[]? configCache;
    private static readonly object configCacheLock = new();

    public ConfigController(ILogger<ConfigController> logger, IBus bus, IOptions<Timeouts> timeoutConfig)
    {
        this.logger = logger;
        this.bus = bus;
        this.timeouts = timeoutConfig.Value;
    }

    [Route("/api/config")]
    [HttpGet]
    public async Task<ConfigItemValue[]> GetConfig([FromQuery] string? forceReload)
    {
        if (string.IsNullOrEmpty(forceReload))
        {
            var config = configCache;
            if (config != null)
            {
                return config;
            }
        }

        var response = await this.bus.Rpc.RequestAsync<GetFrontendConfigRequest, GetFrontendConfigResponse>(new GetFrontendConfigRequest(), o => o.WithExpiration(timeouts.GeneralBackendRequestTimeout));
        configCache = response.Items;
        return response.Items;
    }

    [HttpGet]
    [Route("/api/admin/config")]
    public async Task<ConfigItem[]> GetConfigForAdministration()
    {
        var response = await this.bus.Rpc.RequestAsync<GetConfigRequest, GetConfigResponse>(new GetConfigRequest(), o => o.WithExpiration(timeouts.GeneralBackendRequestTimeout));
        return response.Items;
    }

    [HttpPost]
    [Route("/api/admin/config")]
    public async Task SaveConfig([FromBody] ConfigItem[] items)
    {
        var request = new UpdateConfigRequest { Items = items.Select(i => new ConfigItemValue { Name = i.Name, Value = i.Value }).ToArray() };
        await this.bus.Rpc.RequestAsync<UpdateConfigRequest, UpdateConfigResponse>(request, o => o.WithExpiration(timeouts.GeneralBackendRequestTimeout));
        configCache = null;
    }
}