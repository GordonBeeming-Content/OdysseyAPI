using Microsoft.AspNetCore.Mvc;
using OdysseyAPI.Models;

namespace OdysseyAPI.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ContactsController : ControllerBase
{
  private static readonly string[] _summaries = new[]
  {
      "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
  };

  private readonly ILogger<ContactsController> _logger;

  public ContactsController(ILogger<ContactsController> logger)
  {
    _logger = logger;
  }

  [HttpGet(Name = "GetWeatherForecast")]
  public IEnumerable<WeatherForecast> Get()
  {
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
      Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
      TemperatureC = Random.Shared.Next(-20, 55),
      Summary = _summaries[Random.Shared.Next(_summaries.Length)]
    })
    .ToArray();
  }
}
