namespace shopDev.Configurations;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = "";
    public string DatabaseName { get; set; } = "";
    public string ShopsCollectionName { get; set; } = "";
    public string KeyTokensCollectionName { get; set; } = "";
    public string ApiKeysCollectionName { get; set; } = "";
}
