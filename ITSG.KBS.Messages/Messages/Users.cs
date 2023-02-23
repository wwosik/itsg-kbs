namespace ITSG.KBS.Messages.Users;


public class EnumeratePermissionsRequest
{
}

public class EnumeratePermissionsResponse : IResponse
{
    public Permission[] Permissions { get; set; } = Array.Empty<Permission>();
    public string? Error { get; set; }
}

public class Permission
{
    public string Key { get; set; } = "";
    public string Label { get; set; } = "";
}