using CubeLogic.Models;

namespace CubeLogic.Interfaces
{
    public interface IConfig
    {
        string Timezone { get; set; }
        List<Instrument>? Instruments { get; set; }
    }
}
