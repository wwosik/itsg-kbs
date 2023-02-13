using EasyNetQ;
using ITSG.KBS.Messages.Users;
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
