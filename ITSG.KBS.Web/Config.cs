using System.ComponentModel.DataAnnotations;

namespace ITSG.KBS.Web.Config;

public class Timeouts
{
    [Range(0, 60_000)]
    public int GeneralBackendRequestTimeoutMs { get; set; } = 2000;
    public TimeSpan GeneralBackendRequestTimeout => TimeSpan.FromMilliseconds(GeneralBackendRequestTimeoutMs);
}