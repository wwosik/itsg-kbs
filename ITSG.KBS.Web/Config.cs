using System.ComponentModel.DataAnnotations;

namespace ITSG.KBS.Web.Config;

public class Timeouts
{
    [Range(0, 60_000)]
    public int GeneralBackendRequestTimeoutMs { get; set; } = 500;
    public TimeSpan GeneralBackendRequestTimeout => TimeSpan.FromMilliseconds(GeneralBackendRequestTimeoutMs);
}