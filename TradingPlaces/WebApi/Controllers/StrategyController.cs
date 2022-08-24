using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using TradingPlaces.WebApi.Dtos;
using TradingPlaces.WebApi.Services;

namespace TradingPlaces.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class StrategyController : ControllerBase
    {
        private readonly IHostedServiceAccessor<IStrategyManagementService> _strategyManagementService;
        private readonly ILogger<StrategyController> _logger;

        public StrategyController(IHostedServiceAccessor<IStrategyManagementService> strategyManagementService, ILogger<StrategyController> logger)
        {
            _strategyManagementService = strategyManagementService;
            _logger = logger;
        }

        [HttpPost]
        [SwaggerOperation(nameof(RegisterStrategy))]
        [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(string))]
        public IActionResult RegisterStrategy(StrategyDetailsDto strategyDetails)
        {
            try
            {
                if(strategyDetails == null) 
                    throw new ArgumentNullException(nameof(strategyDetails));
                if(string.IsNullOrWhiteSpace(strategyDetails.Ticker)) 
                    throw new ArgumentNullException(nameof(strategyDetails.Ticker));
                if (strategyDetails.Quantity == 0)
                    throw new ArgumentNullException(nameof(strategyDetails.Quantity));

                return Ok(_strategyManagementService.Service.AddStrategy(strategyDetails));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to register strategy for ticker '{strategyDetails.Ticker}'");
                throw;
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(nameof(UnregisterStrategy))]
        [SwaggerResponse(StatusCodes.Status200OK, "OK")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public IActionResult UnregisterStrategy(string id)
        {
            try
            {
                _strategyManagementService.Service.RemoveStrategy(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to remove strategy '{id}'");
                throw;
            }
        }
    }
}
