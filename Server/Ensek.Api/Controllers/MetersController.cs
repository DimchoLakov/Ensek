using Ensek.Api.Contracts.Requests;
using Ensek.Api.Validation;
using Ensek.Meters.Domain.Services.Meters;
using Microsoft.AspNetCore.Mvc;

namespace Ensek.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetersController : ControllerBase
    {
        private readonly IRequestValidationService _validationService;
        private readonly IMeterService _meterService;

        public MetersController(
            IRequestValidationService validationService,
            IMeterService meterService)
        {
            _validationService = validationService;
            _meterService = meterService;
        }

        [HttpPost("/meter-reading-uploads")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UploadMeterReadings(
            UploadMeterReadingsRequest uploadMeterReadingsRequest,
            CancellationToken cancellationToken)
        {
            await _validationService.Validate(uploadMeterReadingsRequest);

            var result = await _meterService.ProcessReadings(uploadMeterReadingsRequest, cancellationToken);

            return Ok(result);
        }
    }
}
