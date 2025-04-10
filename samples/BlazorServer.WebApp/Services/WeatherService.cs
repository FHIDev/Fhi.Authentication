using System.Text.Json;

namespace BlazorServerWebApp.Services
{
    public record WeatherForcastModel(DateOnly Date, int TemperatureC, int TemperatureF, string? Summary);

    public class WeatherService
    {
        private readonly IHttpClientFactory _factory;

        public WeatherService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<WeatherForcastModel>?> GetWeatherForcast()
        {
            var client = _factory.CreateClient("WebApi");
            var weatherApiResponse = await client.GetAsync("/api/v1/integration/weatherforcasts");
            weatherApiResponse.EnsureSuccessStatusCode();

            var content = await weatherApiResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<WeatherForcastModel>?>(content);
        }
    }
}
