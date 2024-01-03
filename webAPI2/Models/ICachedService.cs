using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace webAPI2.Models
{
    public interface ICachedService 
    {
        T GetData<T>(String Key);

        bool SetData<T>(string key,T Value,DateTimeOffset expirationTime);

        object Removedata(string key);

    }
}
