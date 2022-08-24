using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reutberg;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.Collections;
using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.Services
{
    internal class StrategyManagementService : TradingPlacesBackgroundServiceBase, IStrategyManagementService
    {
        private const int TickFrequencyMilliseconds = 1000;

        private readonly IReutbergService reutbergService;
        private readonly StrategyCollection collection;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public StrategyManagementService(StrategyCollection collection, IReutbergService reutbergService, ILogger<StrategyManagementService> logger)
            : base(TimeSpan.FromMilliseconds(TickFrequencyMilliseconds), logger)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            this.reutbergService = reutbergService ?? throw new ArgumentNullException(nameof(reutbergService));
        }

        public string AddStrategy(StrategyDetailsDto strategyDetails)
        {
            var currentPrice = reutbergService.GetQuote(strategyDetails.Ticker);
            return collection.AddStrategy(strategyDetails, currentPrice);
        }

        public void RemoveStrategy(string id)
        {
            collection.RemoveStrategy(id);
        }

        protected override async Task CheckStrategies()
        {
            try
            {
                await semaphore.WaitAsync();
                collection.ExecuteStrategies(BuySell.Buy, reutbergService);
                collection.ExecuteStrategies(BuySell.Sell, reutbergService);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
