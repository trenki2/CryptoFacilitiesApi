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

using CryptoFacilities.Api.V2;
using System;
using System.Collections.Generic;

namespace Test
{
    internal class Test2
    {
        public static void Run()
        {
            string cfKey = "";
            string cfSecret = "";

            CryptoFacilitiesApi cf = new CryptoFacilitiesApi(cfKey, cfSecret, 500);

            DateTime serverTime = cf.GetServerTime();
            Console.WriteLine(serverTime);

            List<Instrument> instruments = cf.GetInstruments();

            List<Ticker> tickers = cf.GetTickers();
            foreach (var t in tickers)
                Console.WriteLine(t.Symbol + " Ask: " + t.Ask + " Bid: " + t.Bid);

            string symbol = instruments[0].Symbol;

            Console.WriteLine("GetOrderBook for " + symbol);
            OrderBook orderBook = cf.GetOrderBook(symbol);
            decimal ask = orderBook.Asks[0][0];
            decimal bid = orderBook.Bids[0][0];
            decimal spread = ask - bid;
            Console.WriteLine("Spread: " + spread);

            Console.WriteLine("Sending order");
            SendStatus sendStatus = cf.SendOrder("lmt", instruments[0].Symbol, "sell", 1, 10000.0m);

            Console.WriteLine("Open Orders:");
            List<Order> openOrders = cf.GetOpenOrders();
            foreach (var o in openOrders)
                Console.WriteLine(o.OrderId);

            Console.WriteLine("Cancelling Order");
            CancelStatus cancelStatus = cf.CancelOrder(sendStatus.OrderId);
            Console.WriteLine("Cancel Status: " + cancelStatus.Status);

            AccountInfo info = cf.GetAccountInfo();
            Console.WriteLine("Bitcoin balance: " + info.Balances["xbt"]);
            Console.WriteLine("Portfolio Value:" + info.Auxiliary.PortfolioValue);

            //sendStatus = cf.SendOrder("lmt", instruments[0].Symbol, "sell", 1, 10.0m);
            //openOrders = cf.GetOpenOrders();
            //List<History> history = cf.GetHistory(instruments[0].Symbol);

            //List<Fill> fills = cf.GetFills();
            //List<Position> positions = cf.GetOpenPositions();
        }
    }
}