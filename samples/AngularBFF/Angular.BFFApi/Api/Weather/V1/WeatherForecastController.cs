using AngularBFF.Net8.Api.Weather.V1.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AngularBFF.Net8.Api.Weather.V1
{
    [ApiController]
    [Route("/bff/v1/weatherforecast")]
    public class WeatherForecastController(IWeatherForecastService weatherService) : ControllerBase
    {
        public IWeatherForecastService WeatherService { get; } = weatherService;

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecastDto>?> Get()
        {
            var weather = await WeatherService.GetWeather();

            return weather?.Select(w => new WeatherForecastDto() { Date = w.Date, Summary = w.Summary, TemperatureC = w.TemperatureC, TemperatureF = w.TemperatureF });
        }
    }
}
