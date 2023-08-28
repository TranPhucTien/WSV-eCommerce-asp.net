namespace shopDev.Utils;

public class Constants
{
    public static class RequestItem
    {
        public static readonly string ApiKeyObj = "ApiKeyObj";
        public static readonly string KeyTokenStore = "KeyTokenStore";
        public static readonly string User = "User";
    }
    public static class Header
    {
        public static readonly string ApiKey = "x-api-key"; 
        public static readonly string Authorization = "authorization"; 
        public static readonly string ClientId = "x-client-id"; 
        public static readonly string RefreshToken = "x-refresh-token";
    }
}