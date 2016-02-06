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