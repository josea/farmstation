using System.Text.Json.Serialization;

namespace FarmStationDb.Models.FarmDataJson;

public class FarmDataPlot
{
	[JsonPropertyName("Begin")]
	public long BeginUnixDate { get; set; }

	public int Drive { get; set; }
	[JsonPropertyName("End")]
	public long EndUnixDate { get; set; }

	public string Id { get; set; }
	public bool IsNft { get; set; }
	public bool Loaded { get; set; }
	public string PlotSize { get; set; }
	[JsonPropertyName("Size")]
	public long SizeBytes { get; set; }
}
