using Powerplants.Model;
using Powerplants.Models;

namespace Powerplants.Dto
{
    public record PowerPlantDto
    {
        public string Name { get; set; }
        public PowerPlantType Type { get; set; }
        public double Efficiency { get; set; }
        public double Pmin { get; set; }
        public double Pmax { get; set; }
        public double CostPer1Mwh { get; set; }

        public static PowerPlantDto FromModel(PowerPlant model)
        {
            return new PowerPlantDto
            {
                Name = model.Name,
                Type = Enum.Parse<PowerPlantType>(model.Type, true),
                Efficiency = model.Efficiency,
                Pmin = model.Pmin,
                Pmax = model.Pmax,
                CostPer1Mwh = model.CostPer1Mwh
            };
        }
    }
}
