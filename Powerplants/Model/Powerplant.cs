namespace Powerplants.Model
{
    public class PowerPlant
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double Efficiency { get; set; }
        public double Pmin { get; set; }
        public double Pmax { get; set; }
        public double CostPer1Mwh { get; set; }
    }
}
