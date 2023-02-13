using EasyNetQ;
using ITSG.KBS.Web.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ITSG.KBS.Web;

[Route("admin/roles")]
public class RolesController : Controller
{
    private readonly ILogger<RolesController> logger;
    private readonly IBus bus;
    private readonly Timeouts timeouts;

    public RolesController(ILogger<RolesController> logger, IBus bus, IOptions<Timeouts> timeoutConfig)
    {
        this.logger = logger;
        this.bus = bus;
        this.timeouts = timeoutConfig.Value;        
    }

    [Route("permissions")]
    [HttpGet]
    public async Task<Permission[]> EnumeratePermissionsAsync()
    {        
        var response = await this.bus.Rpc.RequestAsync<EnumeratePermissionsRequest, EnumeratePermissionsResponse>(new EnumeratePermissionsRequest(), o => o.WithExpiration(timeouts.GeneralBackendRequestTimeout));
        return response.Permissions;
    }
}

public class EnumeratePermissionsRequest
{
}

public class EnumeratePermissionsResponse
{
    public Permission[] Permissions { get; set; } = Array.Empty<Permission>();
}

public class Permission
{
    public string Key { get; set; } = "";
    public string Label { get; set; } = "";
}