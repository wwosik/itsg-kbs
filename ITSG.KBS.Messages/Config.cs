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

public class ConfigItem
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public string Description { get; set; } = "";
    public string Type { get; set; } = "";
    public bool IsForFrontend { get; set; }
}

public class GetConfigRequest { }

public class GetConfigResponse : IResponse
{
    public ConfigItem[] Items { get; set; } = Array.Empty<ConfigItem>();
    public string? Error { get; set; }

}

public class UpdateConfigRequest
{
    public ConfigItemValue[] Items { get; set; } = Array.Empty<ConfigItemValue>();


}

public class UpdateConfigResponse : IResponse
{
    public string? Error { get; set; }
}