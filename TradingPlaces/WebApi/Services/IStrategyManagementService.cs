using Microsoft.Extensions.Hosting;
using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.Services
{
    public interface IStrategyManagementService : IHostedService
    {
        string AddStrategy(StrategyDetailsDto strategyDetails);
        void RemoveStrategy(string id);
    }
}