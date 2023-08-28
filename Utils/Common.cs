namespace shopDev.Utils;

public class Common
{
    public static Dictionary<T, object> GetInfoData<T>(Dictionary<T, object> obj, List<T> fields)
    {
        return obj.Where(entry => fields.Contains(entry.Key))
            .ToDictionary(entry => entry.Key, entry => entry.Value);
    }
}