/*
Copyright(c) 2016 Markus Trenkwalder

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish, 
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using CryptoFacilities;
using System;
using System.Linq;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string apiKey = "";
            string apiSecret = "";

            CryptoFacilitiesApi api = new CryptoFacilitiesApi(apiKey, apiSecret);

            var contracts = api.GetContracts().OrderBy(x => x.Tradeable).ToList();

            foreach (var c in contracts)
            {
                // Get ticker
                decimal ask, bid;
                api.GetTicker(c.Tradeable, c.Unit, out ask, out bid);
                Console.WriteLine($"{c.Tradeable}\tBid:{bid}\tAsk:{ask}");

                // Get Orderbook
                decimal[][] bids, asks;
                api.GetCumulativeBidAsk(c.Tradeable, c.Unit, out bids, out asks);

                // Access cumulative quantity
                decimal cumulativeQtyBid = bids[0][1];
                decimal cumulativeQtyAsk = asks[0][1];

                // Compute spread
                decimal spread = asks[0][0] - bids[0][0];
                Console.WriteLine($"{c.Tradeable}\tSpread: {spread}");
            }

            var cfbpi = api.GetCFBPI();
            Console.WriteLine($"CF-BPI: {cfbpi}");

            var volatility = api.GetVolatility();
            Console.WriteLine($"Volatility: {volatility}");

            Console.WriteLine("Balance:");
            var balance = api.GetBalance();
            foreach (var e in balance)
                Console.WriteLine($"{e.Key}: {e.Value}");

            Order order = new Order();
            order.Type = "LMT";
            order.Tradeable = contracts[0].Tradeable;
            order.Unit = contracts[0].Unit;
            order.Dir = "Sell";
            order.Qty = 1;
            order.Price = 1000.00m;

            //string orderId = api.PlaceOrder(ref order);
            //Console.WriteLine($"OrderId: {orderId}");

            Console.WriteLine("Open Orders:");
            var orders = api.GetOpenOrders();
            foreach (var o in orders)
                Console.WriteLine(o.Uid);

            //bool res = api.CancelOrder(order);
            //Console.WriteLine($"CancelOrder Result: {res}");

            Console.WriteLine("Trades:");
            var trades = api.GetTrades();
            foreach (var t in trades)
                Console.WriteLine(t.Uid);
        }
    }
}