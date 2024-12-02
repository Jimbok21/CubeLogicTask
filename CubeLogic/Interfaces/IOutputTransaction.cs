using CubeLogic.Models;

namespace CubeLogic.Interfaces
{
    public interface IOutputTransaction
    {
        public int OrderId { get; set; } 
        public string Type { get; set; }
        public int Revision { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public decimal Price { get; set; }
        public string Country { get; set; }
        public string InstrumentName { get; set; }
    }
}
