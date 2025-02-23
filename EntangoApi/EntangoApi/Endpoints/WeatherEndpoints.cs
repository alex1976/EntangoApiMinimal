using Azure.Core;
using Azure;

namespace EntangoApi.Endpoints
{
    public static class WeatherEndpoints
    {
        public static void Map(WebApplication app)
        {
            //GET by location (on Weatherstack)
            app.MapGet("/wheather_by_location", /*[Authorize]*/ async (string location) =>
            {
                var client = new HttpClient();

                string URI = "http://api.weatherstack.com/current?access_key=890e4a0529372bf75bd9c52968447a26" + "&query=" + location.ToString();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(URI),
                    //Headers =
                    //{
                    //    { "access_key", "890e4a0529372bf75bd9c52968447a26" },
                    //    { "query", "Milan" },
                    //},
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(body);
                    return body;
                }
            }

            ).WithTags("Weather");

            //GET by location (on Weatherapi)
            //app.MapGet("/wheather_by_location2", /*[Authorize]*/ async (string location) =>
            //{
            //    var client = new HttpClient();

            //    string URI = "http://api.weatherapi.com/v1/current.json?key=<YOUR_API_KEY>" + "&q=" + location.ToString();

            //    var request = new HttpRequestMessage
            //    {
            //        Method = HttpMethod.Get,
            //        RequestUri = new Uri(URI),
            //        //Headers =
            //        //{
            //        //    { "access_key", "890e4a0529372bf75bd9c52968447a26" },
            //        //    { "query", "Milan" },
            //        //},
            //    };
            //    using (var response = await client.SendAsync(request))
            //    {
            //        response.EnsureSuccessStatusCode();
            //        var body = await response.Content.ReadAsStringAsync();
            //        Console.WriteLine(body);
            //        return body;
            //    }
            //}

            //).WithTags("Weather");
        }
    }
}
