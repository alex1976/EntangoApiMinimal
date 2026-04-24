namespace EntangoApi.Models;

public class OpenMeteoResponse
{
    // API returns numeric values for latitude/longitude/generationtime
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }
    public double GenerationTime_ms { get; set; }
    public HourlyData Hourly { get; set; } = new();
    public HourlyUnits Hourly_units { get; set; } = new();
}

public class HourlyData
{
    public string[] Time { get; set; } = Array.Empty<string>();
    public float[] Temperature_2m { get; set; } = Array.Empty<float>();
    public int[] Precipitation_probability { get; set; } = Array.Empty<int>();
}

public class HourlyUnits
{
    public string Time { get; set; } = string.Empty;
    public string Temperature_2m { get; set; } = string.Empty;
    public string Precipitation_probability { get; set; } = string.Empty;
}

public class WeatherForecastResponse
{
    public DateTimeOffset DateTime { get; set; }
    public float Temperature { get; set; }
    public int PrecipitationProbability { get; set; }
    public string Location { get; set; } = string.Empty;
}