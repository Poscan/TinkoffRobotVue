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
            ;
        }

        [HttpGet("Stock")]
        public async Task<IEnumerable<Stock>> GetStock()
        {
            var currentPortfolioStocks = (await _context.PortfolioAsync()).Positions
                .Where(position => position.InstrumentType == InstrumentType.Stock)
                .Select(position => new Stock
                {
                    AveragePositionPrice = position.AveragePositionPrice.Value,
                    ExpectedYield = position.ExpectedYield.Value,
                    Lots = position.Lots,
                    Name = position.Name
                });

            return currentPortfolioStocks;
        }
    }
}