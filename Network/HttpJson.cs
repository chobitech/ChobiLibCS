using System.Text;
using Newtonsoft.Json;

namespace ChobiLib.Network;


/*
public static class HttpJson
{
    public static async Task<Dictionary<string, dynamic>?> SendAndReceiveAsync(string url, Dictionary<string, dynamic>? sendJson = null, Encoding? encoding = null)
    {
        string json = (sendJson != null) ? JsonConvert.SerializeObject(sendJson) : "{}";

        var content = new StringContent(json, encoding ?? Encoding.UTF8, "application/json");

        using var client = new HttpClient();

        var request = await client.PostAsync(url, content);

        if (request.IsSuccessStatusCode)
        {
            request.res
        }
        else
        {
            return null;
        }
    }
}
*/
