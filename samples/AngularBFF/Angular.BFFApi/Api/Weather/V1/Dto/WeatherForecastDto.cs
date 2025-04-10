namespace AngularBFF.Net8.Api.Weather.V1.Dto
{
    public class WeatherForecastDto
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public string? Summary { get; set; }
    }
}
