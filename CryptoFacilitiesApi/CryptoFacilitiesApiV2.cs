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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace CryptoFacilities.Api.V2
{
    public class Instrument
    {
        /// <summary>
        /// The symbol of the Futures or index.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The type of the instrument, either futures, spot index or volatility index.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// True if the instrument can be traded on Crypto Facilities’ platform, false otherwise.
        /// </summary>
        public bool Tradeable { get; set; }

        /// <summary>
        /// For Futures: The underlying of the Futures, always cfhbpi.
        /// </summary>
        public string Underlying { get; set; }

        /// <summary>
        /// For Futures: The date and time at which the Futures stops trading.
        /// </summary>
        public DateTime LastTradingTime { get; set; }

        /// <summary>
        /// For Futures: The tick size increment of the Futures, currently 0.01 U.S.dollars.
        /// </summary>
        public decimal TickSize { get; set; }

        /// <summary>
        /// For Futures: The contract size of the Futures, currently 1 bitcoin
        /// </summary>
        public int ContractSize { get; set; }
    }

    public class Ticker
    {
        /// <summary>
        /// The symbol of the Futures or index
        /// </summary>
        public string Symbol { get; set; }

        /// True if the market is suspended, false otherwise

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Ticker"/> is suspended.
        /// </summary>
        public bool Suspended { get; set; }

        /// <summary>
        /// For Futures: The price of the last fill.
        /// For indices: The last calculated value.For spot indices, this is a
        /// U.S.dollar value. For the volatility index, this is a percentage value.
        /// </summary>
        public decimal Last { get; set; }

        /// <summary>
        /// The date and time at which last was observed
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// For Futures: The size of the last fill
        /// </summary>
        public int LastSize { get; set; }

        /// <summary>
        /// For Futures: The price of the fill observed 24 hours ago
        /// For cf-bpi and cf-hbpi: The value calculated 24 hours ago
        /// </summary>
        public decimal Open24h { get; set; }

        /// <summary>
        /// For Futures: The highest price of all fills observed in the last 24 hours
        /// For cf-bpi and cf-hbpi: The highest value calculated in the last 24 hours
        /// </summary>
        public decimal High24h { get; set; }

        /// <summary>
        /// For Futures: The lowest price of all fills observed in the last 24 hours.
        /// For cf-bpi and cf-hbpi: The lowest value calculated in the last 24 hours.
        /// </summary>
        public decimal Low24h { get; set; }

        /// <summary>
        /// For Futures: The sum of the sizes of all fills observed in the last 24 hours.
        /// </summary>
        public decimal Vol24h { get; set; }

        /// <summary>
        /// For Futures: The price of the current best bid.
        /// </summary>
        public decimal Bid { get; set; }

        /// <summary>
        /// For Futures: The size of the current best bid
        /// </summary>
        public int BidSize { get; set; }

        /// <summary>
        /// For Futures: The price of the current best ask.
        /// </summary>
        public decimal Ask { get; set; }

        /// <summary>
        /// For Futures: The size of the current best ask.
        /// </summary>
        public int AskSize { get; set; }

        /// <summary>
        /// For Futures: The price to which Crypto Facilities currently marks
        /// the Futures for margining purposes.
        /// </summary>
        public decimal MarkPrice { get; set; }
    }

    public class OrderBook
    {
        /// <summary>
        /// The first value of the inner list is the bid price, the second is the bid size.
        /// The outer list is sorted descending by bid price.
        /// </summary>
        public decimal[][] Bids { get; set; }

        /// <summary>
        /// The first value of the inner list is the ask price, the second is the ask size.
        /// The outer list is sorted ascending by ask price.
        /// </summary>
        public decimal[][] Asks { get; set; }
    }

    public class History
    {
        /// <summary>
        /// The date and time of a trade or an index computation.
        /// For Futures: The date and time of a trade.Data is not aggregated
        /// For indices: The date and time of an index computation.For cfbpi,
        /// data is aggregated to the last computation of each full hour.
        /// For cf-hbpi, data is not aggregated
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// For Futures: A continuous index starting at 1 for the first fill in a
        /// Futures contract maturity
        /// </summary>
        [JsonProperty(PropertyName = "trade_id")]
        public int TradeId { get; set; }

        /// <summary>
        /// For Futures: The price of a fill.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// For Futures: The size of a fill
        /// </summary>
        public int Size { get; set; }
    }

    public class Auxiliary
    {
        /// <summary>
        /// The available funds of the account, a bitcoin figure
        /// </summary>
        [JsonProperty(PropertyName = "af")]
        public decimal AvailableFunds { get; set; }

        /// <summary>
        /// The PnL of current open positions of the account, a U.S.dollar figure
        /// </summary>
        public decimal PnL { get; set; }

        /// <summary>
        /// The portfolio value of the account, a bitcoin figure
        /// </summary>
        [JsonProperty(PropertyName = "pv")]
        public decimal PortfolioValue { get; set; }

        /// <summary>
        /// The total price at which all current open positions of
        /// the account where bought or sold, a U.S.dollar figure.
        /// </summary>
        public decimal USD { get; set; }
    }

    public class MarginRequirements
    {
        /// <summary>
        /// The initial margin requirement of the account.
        /// </summary>
        [JsonProperty(PropertyName = "im")]
        public decimal InitialMargin { get; set; }

        /// <summary>
        /// The maintenance margin requirement of the account.
        /// </summary>
        [JsonProperty(PropertyName = "mm")]
        public decimal MaintenanceMargin { get; set; }

        /// <summary>
        /// The liquidation threshold of the account.
        /// </summary>
        [JsonProperty(PropertyName = "lt")]
        public decimal LiquidationThreshold { get; set; }

        /// <summary>
        /// The termination threshold of the account.
        /// </summary>
        [JsonProperty(PropertyName = "tt")]
        public decimal TerminationThreshold { get; set; }
    }

    public class TriggerEstimates
    {
        /// <summary>
        /// The approximate bitcoin spot price at which the
        /// account will reach its initial margin requirement.
        /// </summary>
        [JsonProperty(PropertyName = "im")]
        public decimal InitialMargin { get; set; }

        /// <summary>
        /// The approximate bitcoin spot price at which the
        /// account will reach its maintenance margin requirement.
        /// </summary>
        [JsonProperty(PropertyName = "mm")]
        public decimal MaintenanceMargin { get; set; }

        /// <summary>
        /// The approximate bitcoin spot price at which the
        /// account will reach its liquidation threshold.
        /// </summary>
        [JsonProperty(PropertyName = "lt")]
        public decimal LiquidationThreshold { get; set; }

        /// <summary>
        /// The approximate bitcoin spot price at which the
        /// account will reach its termination threshold.
        /// </summary>
        [JsonProperty(PropertyName = "tt")]
        public decimal TerminationThreshold { get; set; }
    }

    public class AccountInfo
    {
        /// <summary>
        /// The account balances.
        /// </summary>
        public Dictionary<string, int> Balances { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// A structure containing auxiliary account information.
        /// </summary>
        public Auxiliary Auxiliary { get; set; }

        /// <summary>
        /// A structure containing the account’s margin requirements.
        /// </summary>
        public MarginRequirements MarginRequirements { get; set; }

        /// <summary>
        /// A structure containing the account’s margin trigger estimates.
        /// </summary>
        public TriggerEstimates TriggerEstimates { get; set; }
    }

    public class SendStatus
    {
        /// <summary>
        /// The date and time the order was received.
        /// </summary>
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        /// The status of the order, either of:
        /// placed: the order was placed successfully
        ///  invalidSize: the order was not placed because size is invalid
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The unique identifier of the order.
        /// </summary>
        [JsonProperty(PropertyName = "order_id")]
        public string OrderId { get; set; }
    }

    public class CancelStatus
    {
        /// <summary>
        /// The date and time the order cancellation was received.
        /// </summary>
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        /// The status of the order cancellation, either of:
        /// cancelled: the order was found untouched and the entire size was cancelled successfully.
        /// partiallyFilled: the order was found partially filled and the unfilled size was cancelled successfully.
        /// filled: the order was found completely filled and could not be cancelled.
        /// notFound: the order was not found, either because it had already been cancelled or it never existed.
        /// </summary>
        public string Status { get; set; }
    }

    public class Order
    {
        /// <summary>
        /// The date and time the order was received.
        /// </summary>
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        /// The status of the order, either of:
        /// untouched: the entire size of the order is unfilled
        /// partiallyFilled: the size of the order is partially but not entirely filled
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The unique identifier of the order.
        /// </summary>
        [JsonProperty(PropertyName = "order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// The order type, either lmt for a limit order or stp for a stop order.
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// The symbol of the Futures the order refers to.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The direction of the order, either buy for a buy order or sell for a sell order.
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// The unfilled size associated with the order.
        /// </summary>
        public int UnfilledSize { get; set; }

        /// <summary>
        /// The filled size associated with the order.
        /// </summary>
        public int FilledSize { get; set; }

        /// <summary>
        /// The limit price associated with the order.
        /// </summary>
        public decimal LimitPrice { get; set; }

        /// <summary>
        /// If orderType is stp: The stop price associated with the order
        /// If orderType is lmt: Not returned because N/A.
        /// </summary>
        public decimal? StopPrice { get; set; }
    }

    public class Fill
    {
        /// <summary>
        /// The date and time the order was filled.
        /// </summary>
        public DateTime FillTime { get; set; }

        /// <summary>
        /// The unique identifier of the order
        /// </summary>
        [JsonProperty(PropertyName = "order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// The unique identifier of the fill. Note that several fill_id can pertain to one order_id(but not vice versa).
        /// </summary>
        [JsonProperty(PropertyName = "fill_id")]
        public string FillId { get; set; }

        /// <summary>
        /// The symbol of the Futures the fill occurred in.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The direction of the order, either buy for a buy order or sell for a sell orde.
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// The size of the fill.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The price of the fill.
        /// </summary>
        public decimal Price { get; set; }
    }

    public class Position
    {
        /// <summary>
        /// The date and time the position was entered into.
        /// </summary>
        public DateTime FillTime { get; set; }

        /// <summary>
        /// The symbol of the Futures the position is in.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The direction of the position, either long for a long position or
        /// short for a short position.
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// The size of the position, currently always 1.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The price at which the position was entered into.
        /// </summary>
        public decimal Price { get; set; }
    }

    public class Withdrawal
    {
        /// <summary>
        /// The date and time the withdrawal request was received.
        /// </summary>
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        /// The status of the withdrawal request, either of:
        /// * accepted: the withdrawal request was accepted and will be processed soon
        /// * insufficientAvailableFunds the withdrawal request was not accepted because available funds are insufficient
        /// * invalidAmount: the withdrawal request was not accepted because amount is invalid
        /// * invalidAddress: the withdrawal request was not accepted because targetAddress is not a valid bitcoin address
        /// * failed: the withdrawal request was not accepted because an error occurred
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// If status is accepted: The unique identifier of the withdrawal request
        /// Otherwise: Not returned because N/A
        /// </summary>
        [JsonProperty(PropertyName = "transfer_id")]
        public string TransferId { get; set; }
    }

    public class Transfer
    {
        /// <summary>
        /// * If transferType is deposit: The date and time the deposit was first detected on the bitcoin network
        /// * If transferType is withdrawal: The date and time the withdrawal request was received
        /// </summary>
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        /// * If status is processed: The date and time the transfer has received 3 or more confirmations on the bitcoin blockchain
        ///*  If status is pending: Not returned because N/A
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// The status of the transfer, either of:
        /// * processed: the transfer has received 3 or more confirmations on the bitcoin blockchain
        /// * pending: the transfer has received less than 3 confirmations on the bitcoin blockchain
        /// Note: Deposits become available only when processed,
        /// withdrawals are deducted once the withdrawal request has been received
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The unique identifier of the transfer.
        /// </summary>
        [JsonProperty(PropertyName = "transfer_id")]
        public string TransferId { get; set; }

        /// <summary>
        /// * If status is processed: The blockchain transaction id of
        ///   the transfer if the transfer involves an external bitcoin
        ///   address and Internal Transaction if the transaction
        ///   is sent to an address controlled by Crypto Facilities
        /// * If status is pending: Not returned because N/A
        /// </summary>
        [JsonProperty(PropertyName = "transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        /// The bitcoin address to which the transfer is sent.
        /// </summary>
        public string TargetAddress { get; set; }

        /// <summary>
        /// The type of the transfer, either deposit or withdrawal.
        /// </summary>
        public string TransferType { get; set; }

        /// <summary>
        /// The bitcoin amount that was transferred. Positive for deposits and negative for withdrawals.
        /// </summary>
        public decimal Amount { get; set; }
    }

    internal class ServerResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }
        public DateTime ServerTime { get; set; }
    }

    internal class GetInstrumentsResponse : ServerResponse
    {
        public List<Instrument> Instruments { get; set; }
    }

    internal class GetTickersResponse : ServerResponse
    {
        public List<Ticker> Tickers { get; set; }
    }

    internal class GetOrderBookResponse : ServerResponse
    {
        public OrderBook OrderBook { get; set; }
    }

    internal class GetHistoryResponse : ServerResponse
    {
        public List<History> History { get; set; }
    }

    internal class SendOrderResponse : ServerResponse
    {
        public SendStatus SendStatus { get; set; }
    }

    internal class CancelOrderResponse : ServerResponse
    {
        public CancelStatus CancelStatus { get; set; }
    }

    internal class GetOpenOrdersResponse : ServerResponse
    {
        public List<Order> OpenOrders { get; set; }
    }

    internal class GetFillsResponse : ServerResponse
    {
        public List<Fill> Fills { get; set; }
    }

    internal class GetOpenPositionsResponse : ServerResponse
    {
        public List<Position> OpenPositions { get; set; }
    }

    internal class WithdrawResponse : ServerResponse
    {
        public Withdrawal Withdrawal { get; set; }
    }

    internal class GetTransfersResponse : ServerResponse
    {
        public List<Transfer> Transfers { get; set; }
    }

    public class CryptoFacilitiesApi
    {
        //private const string endpoint = "https://www.cryptofacilities.com/derivatives";
        private const string endpoint = "https://176.31.224.165:9090/derivatives";

        private readonly string apiKey;
        private readonly string apiSecret;
        private readonly int rateLimit;

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
        public string Query(string path, OrderedDictionary param = null, bool auth = false)
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
        /// Gets the server time.
        /// </summary>
        public DateTime GetServerTime()
        {
            string res = Query("/api/v2/instruments");
            var response = JsonConvert.DeserializeObject<GetInstrumentsResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.ServerTime;
        }

        /// <summary>
        /// This endpoint returns key specifications for all currently listed Futures contracts and all indices.
        /// </summary>
        public List<Instrument> GetInstruments()
        {
            string res = Query("/api/v2/instruments");
            var response = JsonConvert.DeserializeObject<GetInstrumentsResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.Instruments;
        }

        /// <summary>
        /// This endpoint returns current market data for all currently listed Futures contracts and all indices.
        /// </summary>
        public List<Ticker> GetTickers()
        {
            string res = Query("/api/v2/tickers");
            var response = JsonConvert.DeserializeObject<GetTickersResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.Tickers;
        }

        /// <summary>
        /// This endpoint returns the entire order book of currently listed Futures contracts.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public OrderBook GetOrderBook(string symbol)
        {
            var param = new OrderedDictionary();
            param.Add("symbol", symbol);

            string res = Query("/api/v2/orderbook", param);
            var response = JsonConvert.DeserializeObject<GetOrderBookResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.OrderBook;
        }

        /// <summary>
        /// This endpoint returns the trading history of currently listed Futures contracts and of bitcoin spot indices.
        /// </summary>
        /// <param name="symbol">The symbol of the Futures or index.</param>
        /// <param name="lastTime">If not provided, returns the last 100 entries of the
        /// history.If provided, returns the 100 entries before lastTime
        /// </param>
        public List<History> GetHistory(string symbol, DateTime? lastTime = null)
        {
            var param = new OrderedDictionary();
            param.Add("symbol", symbol);
            if (lastTime != null)
                param.Add("lastTime", lastTime.Value.ToUniversalTime().ToString("o"));

            string res = Query("/api/v2/history", param);
            var response = JsonConvert.DeserializeObject<GetHistoryResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.History;
        }

        /// <summary>
        /// This endpoint returns key information relating to a Crypto Facilities account. This includes bitcoin and
        /// Futures balances, margin requirements, margin trigger estimates and auxiliary information such as
        /// available funds, PnL of open positions, portfolio value and the virtual U.S.dollar balance.
        /// </summary>
        public AccountInfo GetAccountInfo()
        {
            string res = Query("/api/v2/account", null, true);
            var response = (JObject)JsonConvert.DeserializeObject(res);
            if (response["result"].ToString() != "success")
                throw new Exception(response["error"].ToString());

            var info = new AccountInfo();
            JToken account = response["account"];
            JToken balances = account["balances"];
            foreach (JProperty b in balances)
                info.Balances[b.Name] = (int)b.Value;
            info.Auxiliary = account["auxiliary"].ToObject<Auxiliary>();
            info.MarginRequirements = account["marginRequirements"].ToObject<MarginRequirements>();
            info.TriggerEstimates = account["triggerEstimates"].ToObject<TriggerEstimates>();
            return info;
        }

        /// <summary>
        /// This endpoint allows sending a limit or stop order for a currently listed Futures contract.
        /// </summary>
        /// <param name="orderType">The order type, either lmt for a limit order or stp for a stop order</param>
        /// <param name="symbol">The symbol of the Futures the order refers to.</param>
        /// <param name="side">The direction of the order, either buy for a buy order or sell for a sell order.</param>
        /// <param name="size">The size associated with the order.</param>
        /// <param name="limitPrice">The limit price associated with the order. Must not have more than 2 decimal places.</param>
        /// <param name="stopPrice">The stop price associated with a stop order.
        /// Required if orderType is stp. Must not have more than 2 decimal places.Note that for stp
        /// orders, limitPrice is also required and denotes the worst price at which the stp order can get filled.</param>
        /// <exception cref="System.Exception"></exception>
        public SendStatus SendOrder(string orderType, string symbol, string side, int size, decimal limitPrice, decimal? stopPrice = null)
        {
            var param = new OrderedDictionary();
            param.Add("orderType", orderType);
            param.Add("symbol", symbol);
            param.Add("side", side);
            param.Add("size", size.ToString());
            param.Add("limitPrice", Convert.ToString(limitPrice, CultureInfo.InvariantCulture));
            if (stopPrice != null)
                param.Add("stopPrice", Convert.ToString(stopPrice, CultureInfo.InvariantCulture));

            string res = Query("/api/v2/sendorder", param, true);
            var response = JsonConvert.DeserializeObject<SendOrderResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.SendStatus;
        }

        /// <summary>
        /// This endpoint allows cancelling an open order for a Futures contract.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to be cancelled.</param>
        public CancelStatus CancelOrder(string orderId)
        {
            var param = new OrderedDictionary();
            param.Add("order_id", orderId);

            string res = Query("/api/v2/cancelorder", param, true);
            var response = JsonConvert.DeserializeObject<CancelOrderResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.CancelStatus;
        }

        // This endpoint returns information on all open orders for all Futures contracts.
        public List<Order> GetOpenOrders()
        {
            string res = Query("/api/v2/openorders", null, true);
            var response = JsonConvert.DeserializeObject<GetOpenOrdersResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.OpenOrders;
        }

        /// <summary>
        /// This endpoint returns information on filled orders for all Futures contracts.
        /// </summary>
        /// <param name="lastFillTime">If not provided, returns the last 100 fills in any Futures contract.
        /// If provided, returns the 100 entries before lastFillTime</param>
        /// <returns></returns>
        public List<Fill> GetFills(DateTime? lastFillTime = null)
        {
            var param = new OrderedDictionary();
            if (lastFillTime != null)
                param.Add("lastFillTime", lastFillTime.Value.ToUniversalTime().ToString("o"));

            string res = Query("/api/v2/fills", param, true);
            var response = JsonConvert.DeserializeObject<GetFillsResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.Fills;
        }

        /// <summary>
        /// This endpoint returns all open positions in all Futures contracts. This includes Futures contracts that
        /// have matured but have not yet been settled.
        /// Notes:
        /// * Our platform closes out open positions on a first-in-first-out (FIFO) basis
        /// * A filled order to buy 5 Futures contracts will result in 5 positions of size 1 each
        /// </summary>
        public List<Position> GetOpenPositions()
        {
            string res = Query("/api/v2/openpositions", null, true);
            var response = JsonConvert.DeserializeObject<GetOpenPositionsResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.OpenPositions;
        }

        /// <summary>
        /// This endpoint allows submitting a request to withdraw bitcoins from a Crypto Facilities account.
        /// </summary>
        /// <param name="targetAddress">The bitcoin address to which the withdrawal shall be made.</param>
        /// <param name="amount">The amount of bitcoins that shall be withdrawn.
        /// Must not have more than 8 decimal places.</param>
        public Withdrawal Withdraw(string targetAddress, decimal amount)
        {
            string res = Query("/api/v2/withdrawal", null, true);
            var response = JsonConvert.DeserializeObject<WithdrawResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.Withdrawal;
        }

        /// <summary>
        /// This endpoint returns information on bitcoin deposits and withdrawals to and from a Crypto Facilities account.
        /// </summary>
        /// <param name="lastTransferTime">If not provided, returns the last 100 bitcoin
        /// deposits or withdrawals w.r.t. receivedTime.If provided, returns the 100
        /// entries before lastTransferTime w.r.t. receivedTime.</param>
        public List<Transfer> GetTransfers(DateTime? lastTransferTime = null)
        {
            var param = new OrderedDictionary();
            if (lastTransferTime != null)
                param.Add("lastTransferTime", lastTransferTime.Value.ToUniversalTime().ToString("o"));

            string res = Query("/api/v2/withdrawal", param, true);
            var response = JsonConvert.DeserializeObject<GetTransfersResponse>(res);
            if (response.Result != "success")
                throw new Exception(response.Error);
            return response.Transfers;
        }

        // TODO: implement batchorder

        #region Utility methods

        private void AddHeaders(HttpWebRequest webRequest, string endpointPath, string postData)
        {
            string nonce = GetNonce().ToString();
            string authent = GetAuthent(endpointPath, postData, nonce);

            webRequest.Headers.Add("APIKey", apiKey);
            webRequest.Headers.Add("Nonce", nonce);
            webRequest.Headers.Add("Authent", authent);
        }

        private string GetAuthent(string endpointPath, string postData, string nonce)
        {
            byte[] h = sha256_hash(postData + nonce + endpointPath);
            byte[] base64DecodedSecret = Convert.FromBase64String(apiSecret);
            byte[] r = hmacsha512(base64DecodedSecret, h);
            string authent = Convert.ToBase64String(r);
            return authent;
        }

        private string BuildPostData(OrderedDictionary param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (DictionaryEntry item in param)
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
                return hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        private byte[] hmacsha512(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA512(keyByte))
            {
                return hash.ComputeHash(messageBytes);
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