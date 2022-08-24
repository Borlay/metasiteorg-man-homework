using Reutberg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.Collections
{
    public static class StrategyExtensions
    {
        public static Strategy ToStrategy(this StrategyDetailsDto details, decimal currentPrice)
        {
            var priceDifference = currentPrice * details.PriceMovement / 100;
            return new Strategy()
            {
                Id = Guid.NewGuid().ToString().ToLower(),
                ExecutePrice = details.Instruction == BuySell.Buy ? currentPrice - priceDifference : currentPrice + priceDifference,
                Details = details
            };
        }

        public static void ExecuteStrategies(this StrategyCollection collection, BuySell instruction, IReutbergService reutbergService)
        {
            foreach (var tickerStrategies in collection.GetStrategiesToExecute(instruction, ticker => reutbergService.GetQuote(ticker)))
            {
                if (instruction == BuySell.Buy)
                    reutbergService.Buy(tickerStrategies.Ticker, tickerStrategies.Strategies.Sum(s => s.Details.Quantity));
                else
                    reutbergService.Sell(tickerStrategies.Ticker, tickerStrategies.Strategies.Sum(s => s.Details.Quantity));

                foreach (var strategy in tickerStrategies.Strategies)
                    collection.RemoveStrategy(strategy.Id);
            }
        }
    }
}
