using Reutberg;
using System.Collections.Generic;

namespace TradingPlaces.Tests
{
    public class ReutbergTestService : IReutbergService
    {
        Dictionary<string, decimal> prices = new Dictionary<string, decimal>();

        internal Queue<(string Ticker, int Quantity)> sell = new Queue<(string, int)>();
        internal Queue<(string Ticker, int Quantity)> buy = new Queue<(string, int)>();

        public void SetQuote(string ticker, decimal price)
        {
            prices[ticker.ToLower()] = price;
        }

        public decimal Buy(string ticker, int quantity)
        {
            buy.Enqueue((ticker, quantity));
            return prices[ticker.ToLower()];
        }

        public decimal GetQuote(string ticker) => prices[ticker.ToLower()];

        public decimal Sell(string ticker, int quantity)
        {
            sell.Enqueue((ticker, quantity));
            return prices[ticker.ToLower()];
        }
    }
}
