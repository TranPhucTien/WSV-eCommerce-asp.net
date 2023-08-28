namespace shopDev.Models.Enums;

public enum EApiKeyPermissions
{
    [StringValue("0000")]
    Default
}

public class StringValueAttribute : Attribute
{
    public string Value { get; }

    public StringValueAttribute(string value)
    {
        Value = value;
    }
}