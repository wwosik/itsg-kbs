namespace ITSG.KBS.Web;

public static class DevTimeResponses
{
    public static void HandleDevTimeResponses(this WebApplication app)
    {
        var bus = app.Services.GetRequiredService<EasyNetQ.IBus>();

        bus.Rpc.RespondAsync<EnumeratePermissionsRequest, EnumeratePermissionsResponse>((req, token) =>
        {
            return Task.FromResult(new EnumeratePermissionsResponse
            {
                Permissions = new[]{
            new Permission { Key = "Admin:Translations:List", Label = "Anzeige der Übersetzungen" }
        }
            });
        }, o => { });
    }
}