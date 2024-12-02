using CubeLogic.Interfaces;

namespace CubeLogic.Models
{
    public class Config : IConfig
    {
        public required string Timezone { get; set; }
        public List<Instrument>? Instruments { get; set; }
    }
}
