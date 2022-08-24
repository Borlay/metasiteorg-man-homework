using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reutberg;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.Collections
{
    public class StrategyCollection
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Strategy>>[] strategies = new ConcurrentDictionary<string, ConcurrentDictionary<string, Strategy>>[2];
        private readonly ConcurrentDictionary<string, Strategy> strategiesById = new ConcurrentDictionary<string, Strategy>();

        public StrategyCollection()
        {
            strategies[(int)BuySell.Buy] = new ConcurrentDictionary<string, ConcurrentDictionary<string, Strategy>>();
            strategies[(int)BuySell.Sell] = new ConcurrentDictionary<string, ConcurrentDictionary<string, Strategy>>();
        }

        public string AddStrategy(StrategyDetailsDto strategyDetails, decimal currentPrice)
        {
            if (strategyDetails.Instruction != BuySell.Buy && strategyDetails.Instruction != BuySell.Sell)
                throw new ArgumentException(nameof(strategyDetails.Instruction));

            var tickerStrategies = strategies[(int)strategyDetails.Instruction]
                .GetOrAdd(strategyDetails.Ticker.ToLower(), new ConcurrentDictionary<string, Strategy>());

            var strategy = strategyDetails.ToStrategy(currentPrice);
            tickerStrategies[strategy.Id] = strategy;
            strategiesById[strategy.Id] = strategy;
            return strategy.Id;
        }

        public bool RemoveStrategy(string id)
        {
            if (strategiesById.TryRemove(id.ToLower(), out var strategy))
            {
                var tickerStrategies = strategies[(int)strategy.Details.Instruction]
                    .GetOrAdd(strategy.Details.Ticker.ToLower(), new ConcurrentDictionary<string, Strategy>());
                return tickerStrategies.Remove(id.ToLower(), out _);
            }
            return false;
        }

        public IEnumerable<(string Ticker, Strategy[] Strategies)> GetStrategiesToExecute(BuySell instruction, Func<string, decimal> priceProvider)
        {
            var instructionStrategies = strategies[(int)instruction];
            foreach (var tickerStrategies in instructionStrategies)
            {
                var currentPrice = priceProvider.Invoke(tickerStrategies.Key);
                Queue<Strategy> strategiesToExecute = new Queue<Strategy>();

                foreach (var strategy in tickerStrategies.Value)
                {
                    if (instruction == BuySell.Buy && currentPrice <= strategy.Value.ExecutePrice)
                        strategiesToExecute.Enqueue(strategy.Value);
                    else if (instruction == BuySell.Sell && currentPrice >= strategy.Value.ExecutePrice)
                        strategiesToExecute.Enqueue(strategy.Value);
                }

                if (strategiesToExecute.Count > 0)
                {
                    yield return (tickerStrategies.Key, strategiesToExecute.ToArray());
                }
            }
        }
    }
}
