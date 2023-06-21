using Microsoft.AspNetCore.Mvc;
using Powerplants.Calculators;
using Powerplants.Dto;
using Powerplants.Model;
using Powerplants.Models;
using Powerplants.Models.Response;
using Powerplants.Services;
using System.Numerics;
using System.Reflection;
using System.Runtime.ConstrainedExecution;

namespace Powerplants.Controllers
{
    [Route("productionplan")]
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        private readonly ILogger<ProductionPlanController> _logger;
        private readonly IProductionPlanCalculator _productionPlanCalculator;
        private readonly IPowerPlantCalculator _powerPlantCalculator;

        public ProductionPlanController(
            ILogger<ProductionPlanController> logger, 
            IProductionPlanCalculator productionPlanCalculator,
            IPowerPlantCalculator powerPlantCalculator)
        {
            _logger = logger;
            _productionPlanCalculator = productionPlanCalculator;
            _powerPlantCalculator = powerPlantCalculator;
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
                // TODO : we can use a mapper package instead of doing manually. 
                var productionPlans = _productionPlanCalculator.CalculateProductionPlan(
                    payload.Fuels,
                    payload.Load,
                    payload.PowerPlants.Select(x => PowerPlantDto.FromModel(x))
                );
                return Ok(productionPlans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred while creating the production plan.");
            }
        }

        [HttpPost("myversion")]
        public IActionResult CalculateProductionPlan([FromBody] Payload payload)
        {
            _logger.LogInformation("Start creation of production plan");

            // GVL: Before you can use it you need to test if it is valid
            //      You can use the standard validation rules or the ones of FluentValidation
            //      I would use FluentValidation because it is more flexible and easier to read.

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
                // GVL: I would use a mapper package instead of doing manually if it becomes more complex.
                var dto = payload.PowerPlants.Select(x => PowerPlantDto.FromModel(x));
                var result = _powerPlantCalculator.Calculate(payload.Load, dto, payload.Fuels);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}