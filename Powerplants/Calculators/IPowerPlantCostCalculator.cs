using Powerplants.Dto;
using Powerplants.Model;

namespace Powerplants.Calculators
{
    public interface IPowerPlantCostCalculator
    {
        double CalculateCostPer1Mwh(PowerPlantDto powerPlant, Fuel fuelPrices);
    }
}