using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace WPFBlackjack;

public static class RandomInt
{
    private static readonly HttpClient HttpClient = new();

    private const string ApiUrl = "https://api.random.org/json-rpc/4/invoke";
    
    private const string ApiKey = "";

    private const int NumberOfIntegers = 52;
    private const int MinValue = 1;
    private const int MaxValue = 52;

    public static async Task<int[]> RandomOrderIntArrayApi()
    {
        if (string.IsNullOrEmpty(ApiKey)) return null!;

        var requestObject = new
        {
            jsonrpc = "2.0",
            id = DateTime.UtcNow.Ticks,
            method = "generateIntegers",
            @params = new
            {
                apiKey = ApiKey,
                n = NumberOfIntegers,
                min = MinValue,
                max = MaxValue,
                replacement = false,
                @base = 10,
                pregeneratedRandomization = (object)null!
            }
        };

        return await MakeAPIRequest(requestObject);
        
    }

    private static async Task<int[]> MakeAPIRequest(object requestObject)
    {
        try
        {
            HttpResponseMessage response = await HttpClient.PostAsJsonAsync(ApiUrl, requestObject);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);

            if (jsonResponse != null && jsonResponse!.error != null)
            {
                Console.WriteLine(jsonResponse!.error);
                return null!;
            }

            int[] dataArray = jsonResponse!.result.random.data.ToObject<int[]>();

            return dataArray;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine($"StackTrace: {e.StackTrace}");
            return null!;
        }
    }
}