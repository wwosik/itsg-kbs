namespace ITSG.KBS.Messages.Config;

public class ConfigItemValue
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
}

public class GetFrontendConfigRequest { }

public class GetFrontendConfigResponse : IResponse
{
    public ConfigItemValue[] Items { get; set; } = Array.Empty<ConfigItemValue>();
    public string? Error { get; set; }
}

/// <summary>
/// Config item for administration screen
/// </summary>
public class ConfigItem
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public string Description { get; set; } = "";
    public string Type { get; set; } = "";
    public string IsForFrontend { get; set; } = "N";
    public bool CanBeManaged { get; set; }
}

public class GetConfigRequest { }

public class GetConfigResponse : IResponse
{
    public ConfigItem[] Items { get; set; } = Array.Empty<ConfigItem>();
    public string? Error { get; set; }

    public override string ToString()
    {
        if (this.Error != null) return Error;
        return JsonSerializer.Serialize(Items);
    }
}

public class UpdateConfigRequest
{
    public ConfigItemValue[] Items { get; set; } = Array.Empty<ConfigItemValue>();
}

public class UpdateConfigResponse : IResponse
{
    public string? Error { get; set; }
}