using EntangoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace EntangoApi.Endpoints
{
    public static class RandomDataEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/banks", /*[Authorize]*/ async (int size) =>
            {
                var client = new HttpClient();

                string URI = "https://random-data-api.com/api/bank/random_bank?size=" + size.ToString();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(URI),
                    //Headers =
                    //{
                    //    { "Accept", "application/json" },
                    //},
                };

                request.Headers.Add("Accept", "application/json");

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    response.Headers.Add("Accept", "application/json");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //var body = await response.Content.ReadAsStringAsync();
                    //var body = await response.Content.ReadFromJsonAsync<Bank[]>();
                    Bank[]? banks = await response.Content.ReadFromJsonAsync<Bank[]>();

                    //string result = System.Text.Json.JsonSerializer.Serialize(body);
                    //Console.WriteLine(body);
                    //return body;
                    return banks;
                }
            }

            ).WithTags("RandomData");
        }
    }
}
