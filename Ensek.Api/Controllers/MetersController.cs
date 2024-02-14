using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ensek.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetersController : ControllerBase
    {
        [HttpPost("/meter-reading-uploads")]
        public async Task<ActionResult> UploadMeterReadings([Required] IFormFile file)
        {
            return Ok(new
            {
                successfulReadings = 5,
                failedReadings = 3
            });
        }
    }
}
