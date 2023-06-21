using Powerplants.Dto;
using Powerplants.Model;
using Powerplants.Models.Response;

namespace Powerplants.Calculators
{
    public interface IPowerPlantCalculator
    {
        List<ProducedPower> Calculate(double load, IEnumerable<PowerPlantDto> powerPlants, Fuel fuelPrices);
    }
}
