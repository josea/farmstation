using FarmStation.Models;
using LazyCache;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace FarmStation.Services;

public class FiatService
{
	private readonly HttpClient _httpClient;
	private readonly IAppCache _cache;

	public FiatService(HttpClient httpClient, IAppCache cache)
	{
		_httpClient = httpClient;
		_cache = cache;
	}
	// https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
	// https://stackoverflow.com/questions/20084695/lock-and-async-method-in-c-sharp
	//static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

//	ConcurrentDictionary<string, (decimal rate, DateTime cacheExpiration)> _currencyToChiaCache =
	//		new ConcurrentDictionary<string, (decimal rate, DateTime cacheExpiration)>();



	private async Task<decimal> GetChiaRateToFiatAsync(string currency)
	{
		var rates = await _httpClient.GetFromJsonAsync<CoinGeckoChiaRate>(
			"https://api.coingecko.com/api/v3/simple/price?ids=chia&vs_currencies=cad,usd&include_last_updated_at=true");
		
		if (!rates!.Rate.ContainsKey("cad")) return -1;

		var result = decimal.Parse(rates.Rate["cad"].ToString()!);
		
		return result;
	}

	private Task<ChiaForkRates?> GetChiaForkRatesAsync()
	{
		return _httpClient.GetFromJsonAsync<ChiaForkRates>("https://chiafork.space/API/allforks.json");
	}

	private async Task<decimal> GetRateUSDtoCurrencyAsync(string currency) {
		var rateUsdCad = await _httpClient.GetFromJsonAsync<CoinbaseRates>($"https://api.coinbase.com/v2/prices/USD-{currency}/spot");

		return rateUsdCad!.data.Amount;
	}

	public async Task<decimal> GetRateAsync(string crypto, string currency)
	{
		if (crypto.ToLower() == "xch")
		{
			var result = await _cache.GetOrAddAsync<decimal>("xch-cad", 
				async () => await GetChiaRateToFiatAsync("cad"), DateTimeOffset.Now.AddMinutes(5) );
			if (result < 0) _cache.Remove("xch-cad");

			return result; 
		}
		else
		{
			var rates = await _cache.GetOrAddAsync<ChiaForkRates?>("allforks", () => GetChiaForkRatesAsync(), DateTimeOffset.Now.AddMinutes(30));
			var rateUsdCad = await _cache.GetOrAddAsync<decimal>("usd-cad", async () => await GetRateUSDtoCurrencyAsync("cad"), 
				DateTimeOffset.Now.AddMinutes(1));
			
			var rateCrypto = rates?.Rates.FirstOrDefault(r => r.Symbol.ToLower() == crypto.ToLower());
			if (rateCrypto != null || rateCrypto?.PriceUSD == "nan")
			{
				return decimal.Parse(rateCrypto.PriceUSD) * rateUsdCad; 
			}
			return -1;
		}		
	}
}
