// WeatherForecastController.cs
using Microsoft.AspNetCore.Mvc;
using Day3;
[ApiController]
[Route("api/v1/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherDataService _weatherDataService;
    private readonly HttpClient _httpClient;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherDataService weatherDataService)
    {
        _logger = logger;
        _weatherDataService = weatherDataService;
        _httpClient = new HttpClient();
    }

    [HttpPost("SaveWeatherData")]
    public IActionResult SaveWeatherData([FromBody] WeatherForecast weatherData)
    {
        try
        {
            if (weatherData == null)
            {
                return BadRequest("Invalid data.");
            }
            _weatherDataService.SaveWeatherData(weatherData);
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving WeatherForecast data: {ex.Message}");
            return BadRequest(false);
        }
    }

    [HttpGet("GetSavedWeather")]
    public IActionResult GetSavedWeather()
    {
        try
        {
            var data = _weatherDataService.GetSavedWeatherData();
            if (string.IsNullOrEmpty(data))
            {
                return NotFound("No saved weather data found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading saved weather data: {ex.Message}");
            return BadRequest(false);
        }
    }

    [HttpGet("GetWeatherByCity/{city}")]
    public async Task<IActionResult> GetWeatherByCity(string city)
    {
        if (!string.Equals(city, "Ahmedabad", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Access restricted: Only Ahmedabad city weather data is available.");
        }

        try
        {
            string apiKey = "9448f3bfa283ae7d07e3ab9a4d50c698";
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q=Ahmedabad&appid={apiKey}&units=metric";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(apiUrl);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching Ahmedabad weather: {ex.Message}");
            return StatusCode(500, "Error fetching weather data.");
        }
    }

}
