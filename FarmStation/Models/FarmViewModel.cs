using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FarmStation.Models;

public class FarmViewModel
{
	public decimal BlocksPer10Mins { get; set; }
	public decimal ColdWalletBalanceCrypto { get; set; }
	public decimal ColdWalletBalanceFiat { get; set; }
	public string Crypto { get; set; }
	public decimal DaysSinceLastBlock { get; set; }
	public int DrivesCount { get; set; }
	public decimal EffortPct => (DaysSinceLastBlock / ETWdays) * 100; 
	public decimal ETWdays => BlocksPer10Mins == 0 ? -1 :  (NetSpaceInBytes / PlotsSizeInBytes) / (BlocksPer10Mins * 6.0m * 24.0m);
	public decimal FarmedBalanceCrypto { get; set; }
	public decimal FarmedBalanceFiat { get; set; }
	public int FarmingSinceDaysAgo { get; set; }
	public string FiatCurrency { get; set; }
	public bool HasColdWallet { get; set; }
	public string Id { get; set; }
	public DateTime LastUpdated { get; set; }
	public int LastUpdatedMinAgo => (int) (DateTime.UtcNow - LastUpdated).TotalMinutes;
	public int MissedChallenges { get; set; }
	public string Name { get; set; }
	public decimal NetSpaceInBytes { get; set; }
	public int PlotsCount { get; set; }
	public long PlotsSizeInBytes { get; set; }
	public double ResponseTimeSecondsMax { get; set; }
	public double ResponseTimeSecondsMedian { get; set; }
	public string Status { get; set; }
	public decimal TotalDriveSpaceBytes { get; set; }
	public decimal TotalDriveSpaceTiB => TotalDriveSpaceBytes / (decimal)Math.Pow(1024, 4);
	public decimal WalletBalanceCrypto { get; set; }
	public decimal WalletBalanceFiat { get; set; }
}
