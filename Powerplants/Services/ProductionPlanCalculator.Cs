using Powerplants.Models;
using Powerplants.Models.Response;

namespace Powerplants.Services
{
    public interface ICostCalculator
    {
        void CalculateCostPer1MWh(Fuel fuels, List<PowerPlant> powerplants);
    }

    public class CostCalculator : ICostCalculator
    {
        public void CalculateCostPer1MWh(Fuel fuels, List<PowerPlant> powerplants)
        {
            CalculateGasFiredPowerPlantCost(fuels, powerplants);
            CalculateTurbojetPowerPlantCost(fuels, powerplants);
            CalculateWindTurbinePowerPlantCost(fuels, powerplants);
        }

        private void CalculateGasFiredPowerPlantCost(Fuel fuels, List<PowerPlant> powerplants)
        {
            powerplants
                .Where(p => p.Type == PowerPlantType.GASFIRED.ToString().ToLower())
                .ToList()
                .ForEach(powerPlant => powerPlant.CostPer1Mwh = 1 / powerPlant.Efficiency * fuels.GasEuroPerMWh);
        }

        private void CalculateTurbojetPowerPlantCost(Fuel fuels, List<PowerPlant> powerplants)
        {
            powerplants
                .Where(p => p.Type == PowerPlantType.TURBOJET.ToString().ToLower())
                .ToList()
                .ForEach(powerPlant => powerPlant.CostPer1Mwh = 1 / powerPlant.Efficiency * fuels.KerosineEuroPerMWh);
        }

        private void CalculateWindTurbinePowerPlantCost(Fuel fuels, List<PowerPlant> powerplants)
        {
            powerplants
                .Where(p => p.Type == PowerPlantType.WINDTURBINE.ToString().ToLower())
                .ToList()
                .ForEach(powerPlant =>
                {
                    powerPlant.CostPer1Mwh = 0;
                    powerPlant.Pmax *= fuels.WindPercentage / 100;
                });
        }
    }

    public interface IProductionPlanCalculator
    {
        IEnumerable<ProducedPower> CalculateProductionPlan(Payload payload);
    }

    public class ProductionPlanCalculator : IProductionPlanCalculator
    {
        private readonly ILogger<ProductionPlanCalculator> _logger;
        private readonly ICostCalculator _costCalculator;

        public ProductionPlanCalculator(
            ILogger<ProductionPlanCalculator> logger,
            ICostCalculator costCalculator)
        {
            _logger = logger;
            _costCalculator = costCalculator;
        }

        public IEnumerable<ProducedPower> CalculateProductionPlan(Payload payload)
        {
            _logger.LogInformation("Calculating production plan");

            _costCalculator.CalculateCostPer1MWh(payload.Fuels, payload.PowerPlants);
            payload.PowerPlants = payload.PowerPlants.OrderBy(p => p.CostPer1Mwh).ToList();

            return CalculatePowerPerPowerplant(payload);
        }

        private IEnumerable<ProducedPower> CalculatePowerPerPowerplant(Payload payload)
        {
            var producedPowers = payload.PowerPlants.Select(powerPlant =>
            {
                var power = Math.Min(powerPlant.Pmax, payload.Load);
                payload.Load -= power;
                return new ProducedPower
                {
                    Name = powerPlant.Name,
                    Power = power
                };
            });

            return producedPowers;
        }
    }
}
