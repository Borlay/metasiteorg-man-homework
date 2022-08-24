using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.Collections
{
    public class Strategy
    {
        public string Id { get; set; }
        public decimal ExecutePrice { get; set; }
        public StrategyDetailsDto Details { get; set; }
    }
}
