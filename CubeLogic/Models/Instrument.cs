using CubeLogic.Interfaces;
namespace CubeLogic.Models
{
    public class Instrument : IInstrument
    {
        public int InstrumentId { get; set; }
        public required string InstrumentName { get; set; }
        public required string Country { get; set; }
    }
}
