namespace ITSG.KBS.Messages.Texts;

public class AppText
{
    public string? Key { get; set; }
    public string? IsMarkdown { get; set; }
    public string? Description { get; set; }
    public string? Value { get; set; }
}

public class GetTextsRequest
{
    public bool IncludeDescription { get; set; }
}

public class GetTextsResponse : IResponse
{
    public AppText[] Items { get; set; } = Array.Empty<AppText>();
    public string? Error { get; set; }
}