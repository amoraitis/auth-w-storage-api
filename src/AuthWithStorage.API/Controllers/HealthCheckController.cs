using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AuthWithStorage.API.Controllers
{
    [Authorize("Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealthAsync()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            var result = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
            };

            return report.Status == HealthStatus.Healthy
                ? Ok(result)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
        }
    }
}