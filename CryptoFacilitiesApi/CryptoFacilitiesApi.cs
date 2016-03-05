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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace CryptoFacilities.Api.V1
{
    public class Contract
    {
        /// <summary>
        /// The currency of denomination of the contract (always USD)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The name of the contract (e.g. F-XBT:USD-Mar15)
        /// </summary>
        public string Tradeable { get; set; }

        /// <summary>
        /// The day and the UTC time the contract is last traded (e.g. 2015-03-20 16:00:00)
        /// </summary>
        public string LastTradingDayAndTime { get; set; }

        /// <summary>
        /// The minimum trade size of the contract
        /// </summary>
        public int ContractSize { get; set; }

        /// <summary>
        /// The tick size of the contract
        /// </summary>
        public decimal TickSize { get; set; }

        /// <summary>
        /// A boolean indicating whether the contract is currently suspended from trading, either false or true
        /// </summary>
        public bool Suspended { get; set; }
    }

    public class Order
    {
        #region Required

        // The following parameters must be set to be able to place an order.
        // Type
        // Tradeable
        // Unit
        // Dir
        // Qty
        // Price

        /// <summary>
        /// The order type (always LMT )
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The name of the contract (e.g. F-XBT:USD-Mar15 )
        /// </summary>
        public string Tradeable { get; set; }

        /// <summary>
        /// The currency of denomination of the contract (always USD )
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The direction of the order, either Buy or Sell
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// The quantity of the order. This must be an integer number
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// The limit buy or sell price. This must be a multiple of the tick size, which is 0.01
        /// </summary>
        public decimal Price { get; set; }

        #endregion Required

        // The following parameters will be set on order execution

        /// <summary>
        /// The order identifier if the order has been placed successfully
        /// </summary>
        public string Uid { get; set; }
    }

    public class OrderInfo
    {
        /// <summary>
        /// The identifier of the order
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// The submission time of the order
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// The currency of denomination of the contract (always USD)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The name of the contract (e.g. F-XBT:USD-Mar15)
        /// </summary>
        public string Tradeable { get; set; }

        /// <summary>
        /// The direction of the order, either Buy or Sell
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// The order quantity that has not been matched yet
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// The order quantity that has been matched
        /// </summary>
        public int Filled { get; set; }

        /// <summary>
        /// The order type (always LMT)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The limit buy or sell price
        /// </summary>
        public decimal Lmt { get; set; }
    }

    public class TradeInfo
    {
        /// <summary>
        /// The identifier of the order
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// The submission time of the order
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// The currency of denomination of the contract (always USD)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The name of the contract (e.g. F-XBT:USD-Mar15)
        /// </summary>
        public string Tradeable { get; set; }

        /// <summary>
        /// The direction of the order, either Buy or Sell
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// The order quantity that has not been matched yet
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// The price at which the order was matched
        /// </summary>
        public decimal Price { get; set; }
    }

    internal class GetContractsResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        public List<Contract> Contracts { get; set; }
    }

    internal class GetTickerResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
    }

    internal class GetCumulativeBidAskResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        //public decimal[][] CumulatedBids { get; set; }
        //public decimal[][] CumulatedAsks { get; set; }

        public string CumulatedBids { get; set; }
        public string CumulatedAsks { get; set; }
    }

    internal class GetOpenOrdersResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        public List<OrderInfo> Orders { get; set; }
    }

    internal class GetTradesResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        public List<TradeInfo> Trades { get; set; }
    }

    public class CryptoFacilitiesApi
    {
        private const string endpoint = "https://www.cryptofacilities.com/derivatives";

        private readonly string apiKey;
        private readonly string apiSecret;
        private readonly int rateLimit;

        /// <summary>
        /// Contruct the API interface object.
        ///
        /// This class is thread safe.
        /// </summary>
        /// <param name="key">API Key</param>
        /// <param name="secret">API Secret Key</param>
        /// <param name="rateLimit">Rate limit in milliseconds</param>
        public CryptoFacilitiesApi(string key, string secret, int rateLimit = 500)
        {
            this.apiKey = key;
            this.apiSecret = secret;
            this.rateLimit = rateLimit;
        }

        /// <summary>
        /// Execute a query to the CryptoFacilities API.
        /// </summary>
        /// <param name="path">API Path (e.g. /api/ticker)</param>
        /// <param name="param">Parameters and their values</param>
        /// <param name="auth">Set to true when authentication is to be used</param>
        /// <returns></returns>
        public string Query(string path, Dictionary<string, string> param = null, bool auth = false)
        {
            RateLimit();

            string postData = BuildPostData(param);

            string url = endpoint + path;
            if (postData != "")
                url += "?" + postData;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";

            if (auth)
                AddHeaders(webRequest, path, postData);

            if (postData != "")
            {
                using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write(postData);
                }
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            using (Stream str = webResponse.GetResponseStream())
            using (StreamReader sr = new StreamReader(str))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// This method returns all contracts currently listed, together with their specifications.
        /// </summary>
        /// <returns></returns>
        public List<Contract> GetContracts()
        {
            string res = Query("/api/contracts");
            var response = JsonConvert.DeserializeObject<GetContractsResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.Contracts;
        }

        /// <summary>
        /// This method returns the current best bid and ask prices for a contract (i.e. Level 1 market depth).
        /// </summary>
        /// <param name="tradeable">The name of the contract (e.g. F-XBT:USD-Mar15)</param>
        /// <param name="unit">The currency of denomination of the contract (always USD)</param>
        /// <param name="ask">Current best ask price</param>
        /// <param name="bid">Current best bid price</param>
        public void GetTicker(string tradeable, string unit, out decimal ask, out decimal bid)
        {
            var param = new Dictionary<string, string>();
            param["tradeable"] = tradeable;
            param["unit"] = unit;

            string res = Query("/api/ticker", param);
            var response = JsonConvert.DeserializeObject<GetTickerResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);

            ask = response.Ask;
            bid = response.Bid;
        }

        /// <summary>
        /// This method returns the current best bid and best ask prices for a contract (i.e. Level 2 market depth), together with their cumulative volumes.
        /// </summary>
        /// <param name="tradeable">The name of the contract (e.g. F-XBT:USD-Mar15)</param>
        /// <param name="unit">The currency of denomination of the contract (always USD)</param>
        /// <param name="cumulatedBids">Array or the form [[bid, cumQty],[bid, cumQty],...,[bid, cumQty]]</param>
        /// <param name="cumulatedAsks">Array or the form [[ask, cumQty],[ask, cumQty],...,[ask, cumQty]]</param>
        public void GetCumulativeBidAsk(
            string tradeable,
            string unit,
            out decimal[][] cumulatedBids,
            out decimal[][] cumulatedAsks)
        {
            var param = new Dictionary<string, string>();
            param["tradeable"] = tradeable;
            param["unit"] = unit;

            string res = Query("/api/cumulativebidask", param);
            var response = JsonConvert.DeserializeObject<GetCumulativeBidAskResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);

            var cBids = JsonConvert.DeserializeObject<decimal[][]>(response.CumulatedBids);
            var cAsks = JsonConvert.DeserializeObject<decimal[][]>(response.CumulatedAsks);

            cumulatedBids = cBids.OrderByDescending(x => x[0]).ToArray();
            cumulatedAsks = cAsks;
        }

        /// <summary>
        /// This method returns the current price of the CF-BPI.
        /// </summary>
        /// <returns></returns>
        public decimal GetCFBPI()
        {
            string res = Query("/api/cfbpi");
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            return Convert.ToDecimal(response["cf-bpi"], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method returns an estimate of the current annualized volatility of the CF-BPI.
        /// It is calculated as the standard deviation of log returns of the last 60 observed
        /// minutely prices of the CF-BPI, scaled by sqrt( 60 * 24 * 365 ). It is updated every
        /// 60 seconds and is provided purely for informational purposes but you might find it
        /// useful for monitoring the risk of your portfolio.
        /// </summary>
        /// <returns></returns>
        public decimal GetVolatility()
        {
            string res = Query("/api/volatility");
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            return Convert.ToDecimal(response["volatility"], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This method returns your bitcoin account balance, together with the balances of
        /// all contract positions you might have.
        /// </summary>
        /// <returns>Dictionary with (Tradeable, Balance) entries</returns>
        public Dictionary<string, decimal> GetBalance()
        {
            string res = Query("/api/balance", auth: true);
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            var result = new Dictionary<string, decimal>();

            foreach (var row in response)
            {
                if (row.Key == "result")
                    continue;
                result.Add(row.Key, (decimal)double.Parse(row.Value.ToString(), CultureInfo.InvariantCulture));
            }

            return result;
        }

        /// <summary>
        /// This method allows you to place an order to buy or sell contracts.
        /// </summary>
        /// <param name="type">The order type (always LMT )</param>
        /// <param name="tradeable">The name of the contract (e.g. F-XBT:USD-Mar15 )</param>
        /// <param name="unit">The currency of denomination of the contract (always USD )</param>
        /// <param name="dir">The direction of the order, either Buy or Sell </param>
        /// <param name="qty">The quantity of the order. This must be an integer number</param>
        /// <param name="price">The limit buy or sell price. This must be a multiple of the tick size, which is 0.01</param>
        /// <returns></returns>
        public string PlaceOrder(
            string type,
            string tradeable,
            string unit,
            string dir,
            int qty,
            decimal price)
        {
            var param = new Dictionary<string, string>();
            param["type"] = "LMT";
            param["tradeable"] = tradeable;
            param["unit"] = unit;
            param["dir"] = dir;
            param["qty"] = qty.ToString();
            param["price"] = Convert.ToString(price, CultureInfo.InvariantCulture);

            string res = Query("/api/placeOrder", param, true);
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            return response["orderId"].ToString();
        }

        /// <summary>
        /// This method allows you to place an order to buy or sell contracts.
        /// The method sets the Uid field of the order.
        /// </summary>
        /// <param name="order">The order to palce</param>
        /// <returns>The order id</returns>
        public string PlaceOrder(ref Order order)
        {
            return order.Uid = PlaceOrder(order.Type, order.Tradeable, order.Unit, order.Dir, order.Qty, order.Price);
        }

        /// <summary>
        /// This method allows you to cancel an order that you submitted previously and that
        /// has not been matched yet.
        /// </summary>
        /// <param name="uid">The identifier of the order that you want to cancel. This identifier is returned by the methods 'Place Order' or 'Open Orders'</param>
        /// <param name="tradeable">The name of the contract (e.g. F-XBT:USD-Mar15)</param>
        /// <param name="unit">The currency of denomination of the contract (always USD)</param>
        /// <returns>Returns true on success</returns>
        public bool CancelOrder(string uid, string tradeable, string unit)
        {
            var param = new Dictionary<string, string>();
            param["uid"] = uid;
            param["tradeable"] = tradeable;
            param["unit"] = unit;

            string res = Query("/api/cancelOrder", param, true);
            var response = (JObject)JsonConvert.DeserializeObject(res);

            return response["result"].ToString() == "success";
        }

        /// <summary>
        /// This method allows you to cancel an order that you submitted previously and that
        /// has not been matched yet.
        /// </summary>
        /// <param name="order">The order to cancel</param>
        /// <returns>Returns true on success</returns>
        public bool CancelOrder(Order order)
        {
            return CancelOrder(order.Uid, order.Tradeable, order.Unit);
        }

        /// <summary>
        /// This method allows you to retrieve information on all of your open orders.
        /// </summary>
        /// <returns>A list of OrderInfo objects</returns>
        public List<OrderInfo> GetOpenOrders()
        {
            string res = Query("/api/openOrders", auth: true);
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            try { return JsonConvert.DeserializeObject<List<OrderInfo>>(response["orders"].ToString()); }
            catch (Exception) { return new List<OrderInfo>(); }
        }

        /// <summary>
        /// This method allows you to retrieve information on your last matched orders.
        /// </summary>
        /// <param name="number"> the number of matched orders to return. This must be an integer number. The method's return is capped to a maximum of 100 matched orders</param>
        /// <returns>A list of TradeInfo objects</returns>
        public List<TradeInfo> GetTrades(int number = 100)
        {
            var param = new Dictionary<string, string>();
            param["number"] = number.ToString();

            string res = Query("/api/trades", param, auth: true);
            var response = JsonConvert.DeserializeObject<GetTradesResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);

            return response.Trades ?? new List<TradeInfo>();
        }

        #region Utility methods

        private void AddHeaders(HttpWebRequest webRequest, string path, string postData)
        {
            string nonce = GetNonce().ToString();

            byte[] h = sha256_hash(postData + nonce + path);
            byte[] base64DecodedSecret = Convert.FromBase64String(apiSecret);
            byte[] r = hmacsha512(base64DecodedSecret, h);
            string authent = Convert.ToBase64String(r);

            webRequest.Headers.Add("APIKey", apiKey);
            webRequest.Headers.Add("Nonce", nonce);
            webRequest.Headers.Add("Authent", authent);
        }

        private string BuildPostData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, item.Value));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        private long GetNonce()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private byte[] sha256_hash(string value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));
                return result;
            }
        }

        private byte[] hmacsha512(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA512(keyByte))
            {
                byte[] result = hash.ComputeHash(messageBytes);
                return result;
            }
        }

        #endregion Utility methods

        #region RateLimiter

        private long lastTicks = 0;
        private object thisLock = new object();

        private void RateLimit()
        {
            lock (thisLock)
            {
                long elapsedTicks = DateTime.Now.Ticks - lastTicks;
                var timespan = new TimeSpan(elapsedTicks);
                if (timespan.TotalMilliseconds < rateLimit)
                    Thread.Sleep(rateLimit - (int)timespan.TotalMilliseconds);
                lastTicks = DateTime.Now.Ticks;
            }
        }

        #endregion RateLimiter
    }
}