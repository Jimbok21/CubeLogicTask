using CubeLogic.Interfaces;
namespace CubeLogic.Models
{
    public class Transaction : ITransaction
    {
        public required string Type { get; set; }
        public required int OrderId { get; set; }
        public int InstrumentId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Price { get; set; }
    }
}
