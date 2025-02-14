using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Shrike.Models;

public class HummingbirdService
{
    public static async Task<Hummingbird?> FetchFromSystemInfo(string address)
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetStringAsync($"{address}/systeminfo");
            return JsonSerializer.Deserialize<Hummingbird>(response);
        }
        catch
        {
            return null; 
        }
    }
}
