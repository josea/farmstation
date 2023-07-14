using System.Text.Json.Serialization;

namespace FarmStationDb.Models.FarmDataJson;

public class FarmDataFilterCategories
{
    [JsonPropertyName("0.0-0.25")]
    public int Cat0_0__0_25 { get; set; }

    [JsonPropertyName("0.25-0.5")]
    public int Cat0_25__0_5 { get; set; }

    [JsonPropertyName("0.5-1.0")]
    public int Cat0_5__1_0 { get; set; }

    [JsonPropertyName("1.0-5.0")]
    public int Cat1_0__5_0 { get; set; }

	[JsonPropertyName("10.0-15.0")]
	public int Cat10_0__15_0 { get; set; }

	[JsonPropertyName("15.0-20.0")]

	public int Cat15_0__20_0 { get; set; }

	[JsonPropertyName("20.0-25.0")]
	public int Cat20_0__25_0 { get; set; }

	[JsonPropertyName("25.0-30.0")]
	public int Cat25_0__30_0 { get; set; }

	[JsonPropertyName("5.0-10.0")]
	public int Cat5_0__10_0 { get; set; }
}
