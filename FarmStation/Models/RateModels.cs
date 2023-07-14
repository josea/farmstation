using System.Text.Json.Serialization;

namespace FarmStation.Models;

public class ChiaForkRate
{
	[JsonPropertyName("price")]
	public string PriceUSD { get; set; }

	public string Symbol { get; set; }
}

public class ChiaForkRates
{
	[JsonPropertyName("All Forks")]
	public ChiaForkRate[] Rates { get; set; }
}

public class CoinbaseRates
{
	public CoinbaseRate data { get; set; }
}

public class CoinGeckoChiaRate
{
	[JsonPropertyName("chia")]
	public Dictionary<string, object> Rate { get; set; }

}
public class CoinbaseRate
{
	public decimal Amount { get; set; }
	public string Base { get; set; }
	public string Currency { get; set; }
}