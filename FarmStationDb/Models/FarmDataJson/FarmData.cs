using System.Text.Json.Serialization;

namespace FarmStationDb.Models.FarmDataJson;

/// <summary>
/// From Json model stored in farms.data.
/// </summary>
public class FarmData
{
	public double AvgTime { get; set; }
	public double Balance { get; set; }
	public string BlockChainVersion { get; set; } = null!;
	public decimal BlockRewards { get; set; }
	public decimal BlocksPer10Mins { get; set; }
	public bool CheckPlotSize { get; set; }
	public int CompleteSubSlots { get; set; }
	public string Crypto { get; set; } = null!;
	public string Currency { get; set; } = null!;
	public decimal DaysSinceLastBlock
	{
		get {
			decimal val;			
			if ( decimal.TryParse(Wallets[0].DaysSinceLastBlock, out val)) return val;
			return -1m; 
		}
	}
	public FarmDataDrive[] Drives { get; set; }
	public int EligiblePLots { get; set; }
	public FarmDataFilterCategories FilterCategories { get; set; }
	public long FreeDiskSpace { get; set; }
	public int FullNodesConnected { get; set; }
	public FarmDataHardware Hardware { get; set; }
	public int HarvesterErrors { get; set; }
	public string Id { get; set; } = null!;
	public long LastUpdated { get; set; }
	public int LooseSignagePoints { get; set; }
	public double MaxTime { get; set; }
	public double MedianTime { get; set; }
	public double MinTime { get; set; }
	public int MissedChallenges { get; set; }
	public string Name { get; set; } = null!;
	[JsonPropertyName("NetSpace")]
	public decimal NetSpaceInBytes { get; set; }
	public int NumberFilters { get; set; }
	public bool OnlineConfig { get; set; }
	public int PeakBlockHeight { get; set; }
	public FarmDataPlot[] Plots { get; set; }
	public long PlotsSizeInBytes => Plots.Sum(p => p.SizeBytes);
	public int PoolErrors { get; set; }
	public int ProofsFound { get; set; }
	public string Status { get; set; } = null!;
	public double StdDeviation { get; set; }
	public int SyncedBlockHeight { get; set; }
	public long TotalDiskSpace { get; set; }
	public decimal TotalPlots { get; set; }
	public int Type { get; set; }
	public string Version { get; set; } = null!;
	public FarmDataWallet[] Wallets { get; set; } = null!;
}
