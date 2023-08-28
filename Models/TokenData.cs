namespace shopDev.Models;

public class TokenData<T>
{
    public T Data { get; set; }
    public KeyTokenPair TokenPair { get; set; }

    public TokenData()
    {
    }

    public TokenData(T data, KeyTokenPair tokenPair)
    {
        Data = data;
        TokenPair = tokenPair;
    }
}