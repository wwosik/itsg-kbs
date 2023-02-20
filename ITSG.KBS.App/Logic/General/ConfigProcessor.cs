namespace ITSG.KBS.General;

using System.Data;
using System.Threading;
using ITSG.KBS.Messages.Config;

public class ConfigProcessor : IProcessor<GetFrontendConfigRequest, GetFrontendConfigResponse>, IProcessor<GetConfigRequest, GetConfigResponse>, IProcessor<UpdateConfigRequest, UpdateConfigResponse>
{
    private readonly IDbConnection db;
    private readonly ILogger<ConfigProcessor> logger;

    public ConfigProcessor(IDbConnection db, ILogger<ConfigProcessor> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    public async Task<GetFrontendConfigResponse> ProcessAsync(GetFrontendConfigRequest request, CancellationToken cancellationToken)
    {
        var items = await this.db.QueryAsync<ConfigItemValue>("SELECT Name, Value FROM Config WHERE IsForFrontend = 'Y'");
        return new GetFrontendConfigResponse { Items = items.ToArray() };

    }

    public async Task<GetConfigResponse> ProcessAsync(GetConfigRequest request, CancellationToken cancellationToken)
    {
        this.logger.LogDebug("GetConfig started");
        var items = await this.db.QueryAsync<ConfigItem>("SELECT Name, Value, Description, Type, IsForFrontend FROM Config");
        var result = new GetConfigResponse { Items = items.ToArray() };
        this.logger.LogDebug("GetConfig completed: {result}", result);

        return result;
    }

    public async Task<UpdateConfigResponse> ProcessAsync(UpdateConfigRequest request, CancellationToken cancellationToken)
    {
        await db.ExecuteAsync(@"
        UPDATE Config 
        SET Value = @Value 
            , LastChangedOn = CURRENT_TIMESTAMP
        WHERE Name = @Name and Value <> @Value", request.Items);

        return new UpdateConfigResponse();
    }
}