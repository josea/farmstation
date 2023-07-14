using System.Text.Json.Serialization;

namespace FarmStationDb.Models.FarmDataJson;

public class FarmDataDrive
{
	[JsonPropertyName("AvailableSpace")]
	public decimal AvailableSpaceBytes { get; set; }

	public string DevicePath { get; set; }
	public string MountPath { get; set; }
	[JsonPropertyName("TotalSize")]

	public decimal TotalSizeBytes { get; set; }
	[JsonPropertyName("UsedSpace")]
	public decimal UsedSpaceBytes { get; set; }
}
