namespace CubeLogic.Interfaces
{
    public interface IInstrument
    {
        public int InstrumentId { get; set; }
        public string InstrumentName { get; set; }
        public string Country { get; set; }
    }
}
