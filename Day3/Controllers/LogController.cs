using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class LogController : ControllerBase
{
    private readonly IWeatherDataService _weatherDataService;
    private readonly ILogger<LogController> _logger;

    public LogController(IWeatherDataService weatherDataService, ILogger<LogController> logger)
    {
        _weatherDataService = weatherDataService;
        _logger = logger;
    }

    [HttpGet("GetLogs")]
    public IActionResult GetLogs()
    {
        try
        {
            var logData = _weatherDataService.GetLogData();
            if (string.IsNullOrEmpty(logData))
            {
                return NotFound("No logs found.");
            }

            return Ok(new { logs = logData.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries) });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading log data: {ex.Message}");
            return StatusCode(500, "Internal server error while fetching logs.");
        }
    }
}
