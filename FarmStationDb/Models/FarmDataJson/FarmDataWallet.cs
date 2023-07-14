using System.Numerics;

namespace FarmStationDb.Models.FarmDataJson;

public class FarmDataWallet
{
	public long ConfirmedBalance { get; set; }
	public string Currency { get; set; } = null!;
	public string DaysSinceLastBlock { get; set; }
	public decimal MajorToMinorMultiplier { get; set; }
	public string Name { get; set; } = null!;
	public long NetBalance { get; set; }
	public int Status { get; set; }
	public int Type { get; set; }
	public long UnconfirmedBalance { get; set; }
    public int WalletHeight { get; set; }
}
