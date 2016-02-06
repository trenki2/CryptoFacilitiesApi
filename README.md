# CryptoFacilitiesApi
C# Library to access the CryptoFacilities REST API (https://www.cryptofacilities.com/)

Example code:

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

		// Access best bid and ask
		decimal bestBid = bids[0][0];
		decimal bestAsk = asks[0][0];

		// Compute spread
		decimal spread = bestAsk - bestBid;
		Console.WriteLine($"{c.Tradeable}\tSpread: {spread}");
	}

	