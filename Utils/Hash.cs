namespace shopDev.Utils;

public class Hash
{
    public static string GenerateKey()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }
}