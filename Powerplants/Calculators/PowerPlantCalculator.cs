using Powerplants.Dto;
using Powerplants.Model;
using Powerplants.Models.Response;

namespace Powerplants.Calculators
{
    /// <summary>
    /// GVL: I have rewritten this class to use functional programming so it is compliant for concurrent or
    ///      parallel processing when we have huge load. This is the "new" way of doing it.
    /// </summary>
    public class PowerPlantCalculator : IPowerPlantCalculator
    {
        private readonly IPowerPlantCostCalculator _powerPlantCostCalculator;

        public PowerPlantCalculator(IPowerPlantCostCalculator powerPlantCostCalculator)
        {
            _powerPlantCostCalculator = powerPlantCostCalculator;
        }

        //public static List<ProducedPower> Calculate(double load, 
        //    IEnumerable<PowerPlantDto> powerPlants, Fuel fuelPrices, double windPercentage)
        //{
        //    var availablePowerPlants = powerPlants.ToList();
        //    var producedPower = new List<ProducedPower>();
        //    while (load > 0 && availablePowerPlants.Count > 0)
        //    {
        //        availablePowerPlants.Sort(
        //            (x, y) => CalculateCostPer1Mwh(x, fuelPrices).CompareTo(CalculateCostPer1Mwh(y, fuelPrices)));

        //        var powerPlant = availablePowerPlants[0];
        //        var powerPlantProduction = CalculatePowerPlantProduction(powerPlant, load, windPercentage);

        //        if (powerPlantProduction >= powerPlant.Pmin)
        //        {
        //            // Power plant can generate enough power, assign load to it
        //            producedPower.Add(new ProducedPower
        //            {
        //                Name = powerPlant.Name,
        //                Power = powerPlantProduction
        //            });
        //            load -= powerPlantProduction;
        //        }

        //        availablePowerPlants.RemoveAt(0);
        //    }

        //    return producedPower;
        //}

        // GVL: this is the "new" way of doing it with functional programming
        public List<ProducedPower> Calculate(double load, IEnumerable<PowerPlantDto> powerPlants, Fuel fuelPrices)
        {
            var sortedPowerPlants = powerPlants
                .OrderBy(pp => _powerPlantCostCalculator.CalculateCostPer1Mwh(pp, fuelPrices))
                .ToList();

            return CalculatePowerProduction(load, sortedPowerPlants, fuelPrices);
        }

        // GVL: this is the "new" way of doing it with functional programming
        private List<ProducedPower> CalculatePowerProduction(double remainingLoad, List<PowerPlantDto> powerPlants, Fuel fuelPrices)
        {
            if (remainingLoad <= 0 || powerPlants.Count == 0)
                return new List<ProducedPower>();

            var powerPlant = powerPlants.First();
            var powerPlantProduction = CalculatePowerPlantProduction(powerPlant, remainingLoad, fuelPrices.WindPercentage);

            if (powerPlantProduction >= powerPlant.Pmin)
            {
                var producedPower = new ProducedPower
                {
                    Name = powerPlant.Name,
                    Power = powerPlantProduction
                };

                var remainingPowerPlants = powerPlants.Skip(1).ToList();
                var remainingLoadAfterProduction = remainingLoad - powerPlantProduction;

                var remainingProducedPower = CalculatePowerProduction(remainingLoadAfterProduction, remainingPowerPlants, fuelPrices);
                return new List<ProducedPower> { producedPower }.Concat(remainingProducedPower).ToList();
            }
            else
            {
                return CalculatePowerProduction(remainingLoad, powerPlants.Skip(1).ToList(), fuelPrices);
            }
        }

        private  double CalculatePowerPlantProduction(PowerPlantDto powerPlant, double remainingLoad, double windPercentage)
        {
            // GVL: this is the old way of doing it with imperative programming

            //double powerPlantProduction = 0;

            //if (powerPlant.Type == PowerPlantType.WINDTURBINE)
            //{
            //    var maxWindProduction = powerPlant.Pmax * windPercentage / 100;
            //    powerPlantProduction = Math.Min(maxWindProduction, remainingLoad);
            //}
            //else
            //{
            //    powerPlantProduction = Math.Min(powerPlant.Pmax, remainingLoad);
            //}

            //return powerPlantProduction;

            // GVL: this is the "new" way of doing it with functional programming
            return powerPlant.Type == PowerPlantType.WINDTURBINE
                ? Math.Min(powerPlant.Pmax * windPercentage / 100, remainingLoad)
                : Math.Min(powerPlant.Pmax, remainingLoad);
        }
    }
}
