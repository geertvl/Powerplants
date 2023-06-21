using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Powerplants.Model
{
    public class Fuel
    {
        [JsonPropertyName("gas(euro/MWh)")]
        public double GasEuroPerMWh { get; set; }

        [JsonPropertyName("kerosine(euro/MWh)")]
        public double KerosineEuroPerMWh { get; set; }

        [JsonPropertyName("co2(euro/ton)")]
        public double Co2EuroPerTon { get; set; }

        [JsonPropertyName("wind(%)")]
        public double WindPercentage { get; set; }
    }
}
