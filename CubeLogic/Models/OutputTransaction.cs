using CubeLogic.Interfaces;
namespace CubeLogic.Models
{
    public class OutputTransaction : IOutputTransaction
    {
        public int OrderId { get; set; } = 0;
        public string Type { get; set; } = "";
        public int Revision { get; set; }
        public DateTime DateTimeUtc { get; set; } = DateTime.Today;
        public decimal Price { get; set; }
        public string Country { get; set; } = "";
        public string InstrumentName { get; set; } = "";
    }
}
