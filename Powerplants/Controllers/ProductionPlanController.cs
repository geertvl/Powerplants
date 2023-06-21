using Microsoft.AspNetCore.Mvc;
using Powerplants.Models;
using Powerplants.Models.Response;
using Powerplants.Services;

namespace Powerplants.Controllers
{
    [Route("productionplan")]
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        private readonly ILogger<ProductionPlanController> _logger;
        private readonly IProductionPlanCalculator _productionPlanCalculator;
        public ProductionPlanController(ILogger<ProductionPlanController> logger, IProductionPlanCalculator productionPlanCalculator)
        {
            _logger = logger;
            _productionPlanCalculator = productionPlanCalculator;
        }

        [HttpPost]
        public ActionResult<IEnumerable<ProducedPower>> CreateProductionPlan([FromBody] Payload payload)
        {
            _logger.LogInformation("Start creation of production plan");

            if (!ModelState.IsValid)
            {
                // If the payload is invalid, return a BadRequest response with error details
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(errors);
            }

            try
            {
                var productionPlans = _productionPlanCalculator.CalculateProductionPlan(payload);
                return Ok(productionPlans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred while creating the production plan.");
            }
        }
    }
}