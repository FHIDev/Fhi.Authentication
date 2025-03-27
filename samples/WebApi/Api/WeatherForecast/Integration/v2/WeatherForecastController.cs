using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Api.WeatherForecast.Integration.v2
{

    [ApiController]
    [Route("api/v2/integration/weatherforcasts")]
    public class WeatherForecastController
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForcastService _weatherForcastService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForcastService weatherForcastService)
        {
            _logger = logger;
            _weatherForcastService = weatherForcastService;
        }

        /// <summary>
        /// Accepting DPoP authentication scheme
        /// </summary>
        /// <returns></returns>
        [Authorize("dpop")]
        [HttpGet]
        public IEnumerable<WeatherForecast2> GetWithDPoP()
        {
            var summaries = _weatherForcastService.GetSummaries();
            return Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast2(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]))
            .ToArray();
        }
    }
}
