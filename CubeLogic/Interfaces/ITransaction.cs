namespace CubeLogic.Interfaces
{
    public interface ITransaction
    {
        public string Type { get; set; }
        public int OrderId { get; set; }
        public int InstrumentId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Price { get; set; }
    }
}