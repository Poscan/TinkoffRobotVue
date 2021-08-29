using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinkOff.Domain
{
    public class Stock
    {
        public Decimal ExpectedYield { get; set; }
        public Decimal AveragePositionPrice { get; set; }
        public int Lots { get; set; }
        public string Name { get; set; }

        public Decimal CostPrice => AveragePositionPrice * Lots;
        public Decimal CurrentPrice => CostPrice + ExpectedYield;
        public Decimal Percentages => Decimal.Round(((CurrentPrice * 100 / CostPrice) - 100), 2);
    }
}
