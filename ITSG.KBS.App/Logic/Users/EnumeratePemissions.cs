using ITSG.KBS.Messages.Users;

namespace ITSG.KBS.Users;

public class EnumeratePermissionsProcessor : IProcessor<EnumeratePermissionsRequest, EnumeratePermissionsResponse>
{
    private readonly ILogger<EnumeratePermissionsProcessor> logger;

    public EnumeratePermissionsProcessor(ILogger<EnumeratePermissionsProcessor> logger)
    {
        this.logger = logger;
    }

    public Task<EnumeratePermissionsResponse> ProcessAsync(EnumeratePermissionsRequest request, CancellationToken cancellationToken)
    {
        this.logger.LogDebug("Got request");

        return Task.FromResult(new EnumeratePermissionsResponse
        {
            Permissions = new[]{
            new Permission { Key = "Admin:Translations:List", Label = "Anzeige der Übersetzungen" }
        }
        });
    }
}

