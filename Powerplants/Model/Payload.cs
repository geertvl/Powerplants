namespace Powerplants.Model
{
    public class Payload
    {
        public double Load { get; set; }
        public Fuel Fuels { get; set; }
        public List<PowerPlant> PowerPlants { get; set; }
    }
}
