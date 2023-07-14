using System.Diagnostics.Eventing.Reader;

namespace FarmStationDb.Models.FarmDataJson;

public class FarmDataMemory
{
	public long Free { get; set; }
	public long FreeVirtual { get; set; }
	public long Timestamp { get; set; }
	public long Total { get; set; }
    public long TotalVirtual { get; set; }
}
