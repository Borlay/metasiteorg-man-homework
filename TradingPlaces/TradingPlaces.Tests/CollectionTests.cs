using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.Collections;

namespace TradingPlaces.Tests
{
    [TestClass]
    public class CollectionTests
    {
        private void FillDefaultCollection(StrategyCollection collection, decimal currentPrice)
        {
            collection.AddStrategy(new WebApi.Dtos.StrategyDetailsDto()
            {
                Instruction = BuySell.Buy,
                PriceMovement = 2,
                Quantity = 10,
                Ticker = "TK",
            }, currentPrice);

            collection.AddStrategy(new WebApi.Dtos.StrategyDetailsDto()
            {
                Instruction = BuySell.Buy,
                PriceMovement = 3,
                Quantity = 15,
                Ticker = "TK",
            }, currentPrice);

            collection.AddStrategy(new WebApi.Dtos.StrategyDetailsDto()
            {
                Instruction = BuySell.Sell,
                PriceMovement = 2,
                Quantity = 11,
                Ticker = "TK",
            }, currentPrice);

            collection.AddStrategy(new WebApi.Dtos.StrategyDetailsDto()
            {
                Instruction = BuySell.Sell,
                PriceMovement = 3,
                Quantity = 16,
                Ticker = "TK",
            }, currentPrice);
        }

        [TestMethod]
        public void Execute1BuyStrategy()
        {
            var service = new ReutbergTestService();
            StrategyCollection collection = new StrategyCollection();

            service.SetQuote("TK", 100);
            FillDefaultCollection(collection, 100);

            collection.ExecuteStrategies(BuySell.Buy, service);
            collection.ExecuteStrategies(BuySell.Sell, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            service.SetQuote("TK", 98);

            collection.ExecuteStrategies(BuySell.Sell, service);
            collection.ExecuteStrategies(BuySell.Buy, service);

            Assert.AreEqual(1, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            Assert.AreEqual(10, service.buy.ToArray().Sum(s => s.Quantity));
        } 

        [TestMethod]
        public void Execute2BuyStrategies()
        {
            var service = new ReutbergTestService();
            StrategyCollection collection = new StrategyCollection();

            service.SetQuote("TK", 100);
            FillDefaultCollection(collection, 100);

            collection.ExecuteStrategies(BuySell.Buy, service);
            collection.ExecuteStrategies(BuySell.Sell, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            service.SetQuote("TK", 96);

            collection.ExecuteStrategies(BuySell.Sell, service);
            collection.ExecuteStrategies(BuySell.Buy, service);

            Assert.AreEqual(1, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            Assert.AreEqual(25, service.buy.ToArray().Sum(s => s.Quantity));
        }

        [TestMethod]
        public void Execute1SellStrategy()
        {
            var service = new ReutbergTestService();
            StrategyCollection collection = new StrategyCollection();

            service.SetQuote("TK", 100);
            FillDefaultCollection(collection, 100);

            collection.ExecuteStrategies(BuySell.Buy, service);
            collection.ExecuteStrategies(BuySell.Sell, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            service.SetQuote("TK", 102);

            collection.ExecuteStrategies(BuySell.Sell, service);
            collection.ExecuteStrategies(BuySell.Buy, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(1, service.sell.Count);

            Assert.AreEqual(11, service.sell.ToArray().Sum(s => s.Quantity));
        }

        [TestMethod]
        public void Execute2SellStrategies()
        {
            var service = new ReutbergTestService();
            StrategyCollection collection = new StrategyCollection();

            service.SetQuote("TK", 100);
            FillDefaultCollection(collection, 100);

            collection.ExecuteStrategies(BuySell.Buy, service);
            collection.ExecuteStrategies(BuySell.Sell, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            service.SetQuote("TK", 103);

            collection.ExecuteStrategies(BuySell.Sell, service);
            collection.ExecuteStrategies(BuySell.Buy, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(1, service.sell.Count);

            Assert.AreEqual(27, service.sell.ToArray().Sum(s => s.Quantity));
        }

        [TestMethod]
        public void Execute2Buy2SellStrategies()
        {
            var service = new ReutbergTestService();
            StrategyCollection collection = new StrategyCollection();

            service.SetQuote("TK", 100);
            FillDefaultCollection(collection, 100);

            service.SetQuote("TK", 94);
            FillDefaultCollection(collection, 94);

            service.SetQuote("TK", 97);

            collection.ExecuteStrategies(BuySell.Buy, service);
            collection.ExecuteStrategies(BuySell.Sell, service);

            Assert.AreEqual(1, service.buy.Count);
            Assert.AreEqual(1, service.sell.Count);

            Assert.AreEqual(25, service.buy.ToArray().Sum(s => s.Quantity));
            Assert.AreEqual(27, service.sell.ToArray().Sum(s => s.Quantity));
        }

        [TestMethod]
        public void ExecuteStrategyTwice()
        {
            var service = new ReutbergTestService();
            StrategyCollection collection = new StrategyCollection();

            service.SetQuote("TK", 100);
            FillDefaultCollection(collection, 100);

            collection.ExecuteStrategies(BuySell.Buy, service);
            collection.ExecuteStrategies(BuySell.Sell, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            service.SetQuote("TK", 98);

            collection.ExecuteStrategies(BuySell.Sell, service);
            collection.ExecuteStrategies(BuySell.Buy, service);

            Assert.AreEqual(1, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            Assert.AreEqual(10, service.buy.ToArray().Sum(s => s.Quantity));
            service.buy.Clear();

            collection.ExecuteStrategies(BuySell.Sell, service);
            collection.ExecuteStrategies(BuySell.Buy, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);

            Assert.AreEqual(0, service.buy.ToArray().Sum(s => s.Quantity));
        }

        [TestMethod]
        public void RemoveAndExecuteStrategy()
        {
            var service = new ReutbergTestService();
            StrategyCollection collection = new StrategyCollection();

            service.SetQuote("TK", 100);

            var strategyId = collection.AddStrategy(new WebApi.Dtos.StrategyDetailsDto()
            {
                Instruction = BuySell.Buy,
                PriceMovement = 2,
                Quantity = 10,
                Ticker = "TK",
            }, 100);

            Assert.IsTrue(collection.RemoveStrategy(strategyId));
            service.SetQuote("TK", 97);


            collection.ExecuteStrategies(BuySell.Buy, service);
            collection.ExecuteStrategies(BuySell.Sell, service);

            Assert.AreEqual(0, service.buy.Count);
            Assert.AreEqual(0, service.sell.Count);
        }
    }
}
