using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TinkOff.Domain;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace TinkOff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TinkoffController : ControllerBase
    {
        private readonly Context _context;

        public TinkoffController()
        {
            var connection = ConnectionFactory.GetConnection(TinkOff.Properties.TinkoffTokken.Tokken);

            _context = connection.Context;
        }

        [HttpGet]
        public async Task<Portfolio> Get()
        {
            return await _context.PortfolioAsync();
        }

        [HttpGet("Stock")]
        public async Task<IEnumerable<Stock>> GetStock()
        {
            var currentPortfolioStocks = (await _context.PortfolioAsync()).Positions
                .Where(position => position.InstrumentType == InstrumentType.Stock)
                .Select(position => new Stock
                {
                    AveragePositionPrice = position.AveragePositionPrice.Value,
                    CurrentPrice = _context.MarketOrderbookAsync(position.Figi, 0).Result.LastPrice,
                    Lots = position.Lots,
                    Name = position.Name,
                    PercentagesPerDay = Decimal.Round(_context
                        .MarketCandlesAsync(position.Figi, DateTime.Today, DateTime.Now, CandleInterval.Day).Result
                        .Candles.Select(x => x.Close * 100 / x.Open).First() - 100, 2)
                });

            return currentPortfolioStocks;
        }

        [HttpGet("StockPerDay")]
        public async Task<IEnumerable<Stock>> GetStockPerDay()
        {
            var currentPortfolioStocks = (await _context.PortfolioAsync()).Positions
                .Where(position => position.InstrumentType == InstrumentType.Stock)
                .Select(position => new Stock
                {
                    AveragePositionPrice = position.AveragePositionPrice.Value,
                    CurrentPrice = _context.MarketOrderbookAsync(position.Figi, 0).Result.LastPrice,
                    Lots = position.Lots,
                    Name = position.Name,
                });

            return currentPortfolioStocks;
        }

        [HttpGet("Currencies")]
        public async Task<IEnumerable<Currencies>> GetCurrencies()
        {
            return (await _context.PortfolioCurrenciesAsync()).Currencies.Select(currency => new Currencies
            {
                Balance = currency.Balance,
                Currency = (int) currency.Currency
            });
        }
    }
}