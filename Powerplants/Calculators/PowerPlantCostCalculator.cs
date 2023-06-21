using Powerplants.Dto;
using Powerplants.Model;

namespace Powerplants.Calculators
{
    /// <summary>
    /// GVL: separation in different classes is not necessary for this simple implementation. 
    ///     But for demonstration purposes it shows you a separation of concern.
    ///     If the logic becomes complex or this class is been used in multiple places,
    ///     it is the right way to separate it.
    /// </summary>
    public class PowerPlantCostCalculator : IPowerPlantCostCalculator
    {
        public double CalculateCostPer1Mwh(PowerPlantDto powerPlant, Fuel fuelPrices)
        {
            // GVL: this is the old way of doing it with imperative programming

            //double costPerMwh = 0;

            //switch (powerPlant.Type)
            //{
            //    case PowerPlantType.GASFIRED:
            //        costPerMwh = fuelPrices.GasEuroPerMWh;
            //        break;
            //    case PowerPlantType.TURBOJET:
            //        costPerMwh = fuelPrices.KerosineEuroPerMWh;
            //        break;
            //    case PowerPlantType.WINDTURBINE:
            //        costPerMwh = 0; // Wind turbines have zero cost
            //        break;
            //}
            //return costPerMwh / powerPlant.Efficiency;

            // GVL: this is the "new" way of doing it with functional programming
            double costPerMwh = powerPlant.Type switch
            {
                PowerPlantType.GASFIRED => fuelPrices.GasEuroPerMWh,
                PowerPlantType.TURBOJET => fuelPrices.KerosineEuroPerMWh,
                PowerPlantType.WINDTURBINE => 0,
                _ => throw new InvalidOperationException("Invalid power plant type.")
            };

            return costPerMwh / powerPlant.Efficiency;
        }
    }
}
