using Powerplants.Dto;
using Powerplants.Model;
using Powerplants.Models;
using Powerplants.Models.Response;

namespace Powerplants.Services
{
    public interface ICostCalculator
    {
        void CalculateCostPer1MWh(Fuel fuels, IEnumerable<PowerPlantDto> powerplants);
    }

    public class CostCalculator : ICostCalculator
    {
        public void CalculateCostPer1MWh(Fuel fuels, IEnumerable<PowerPlantDto> powerplants)
        {
            CalculateGasFiredPowerPlantCost(fuels, powerplants);
            CalculateTurbojetPowerPlantCost(fuels, powerplants);
            CalculateWindTurbinePowerPlantCost(fuels, powerplants);
        }

        private void CalculateGasFiredPowerPlantCost(Fuel fuels, IEnumerable<PowerPlantDto> powerplants)
        {
            powerplants
                .Where(p => p.Type == PowerPlantType.GASFIRED)
                .ToList()
                .ForEach(powerPlant => powerPlant.CostPer1Mwh = 1 / powerPlant.Efficiency * fuels.GasEuroPerMWh);
        }

        private void CalculateTurbojetPowerPlantCost(Fuel fuels, IEnumerable<PowerPlantDto> powerplants)
        {
            powerplants
                .Where(p => p.Type == PowerPlantType.TURBOJET)
                .ToList()
                .ForEach(powerPlant => powerPlant.CostPer1Mwh = 1 / powerPlant.Efficiency * fuels.KerosineEuroPerMWh);
        }

        private void CalculateWindTurbinePowerPlantCost(Fuel fuels, IEnumerable<PowerPlantDto> powerplants)
        {
            powerplants
                .Where(p => p.Type == PowerPlantType.WINDTURBINE)
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
        IEnumerable<ProducedPower> CalculateProductionPlan(Fuel fuel, double load, IEnumerable<PowerPlantDto> powerPlants);
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

        public IEnumerable<ProducedPower> CalculateProductionPlan(Fuel fuel, double load, IEnumerable<PowerPlantDto> powerPlants)
        {
            _logger.LogInformation("Calculating production plan");

            _costCalculator.CalculateCostPer1MWh(fuel, powerPlants);
            return CalculatePowerPerPowerplant(load, powerPlants.OrderBy(p => p.CostPer1Mwh));
        }

        private IEnumerable<ProducedPower> CalculatePowerPerPowerplant(double load, IEnumerable<PowerPlantDto> powerPlants)
        {
            var producedPowers = powerPlants.Select(powerPlant =>
            {
                var power = Math.Min(powerPlant.Pmax, load);
                load -= power;
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
