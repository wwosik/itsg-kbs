namespace ITSG.KBS.General;

using System.Data;
using System.Threading;
using ITSG.KBS.Messages.Texts;

public class TextsProcessor : IProcessor<GetTextsRequest, GetTextsResponse>
{
    private readonly IDbConnection db;
    private readonly ILogger<TextsProcessor> logger;

    public TextsProcessor(IDbConnection db, ILogger<TextsProcessor> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    public async Task<GetTextsResponse> ProcessAsync(GetTextsRequest request, CancellationToken cancellationToken)
    {
        var items = await this.db.QueryAsync<AppText>($@"
            SELECT Key, IsMarkdown {(request.IncludeDescription ? ", Description" : "")}, Value FROM Texts
            ");
        return new GetTextsResponse { Items = items.ToArray() };

    }
}