namespace ITSG.KBS.Messages.Config;

public class ConfigItemValue
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
}

public class GetFrontendConfigRequest { }

public class GetFrontendConfigResponse
{
    public ConfigItemValue[] Items { get; set; } = Array.Empty<ConfigItemValue>();
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

public class GetConfigResponse
{
    public ConfigItem[] Items { get; set; } = Array.Empty<ConfigItem>();
}

public class UpdateConfigRequest
{
    public ConfigItemValue[] Items { get; set; } = Array.Empty<ConfigItemValue>();
}

public class UpdateConfigResponse { }