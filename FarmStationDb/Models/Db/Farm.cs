using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FarmStation.Models;

public partial class Farm
{
	public string? Data { get; set; } = null!;

	[MaxLength(10)]
	public string FarmingStatus { get; set; } = string.Empty;

	[MaxLength(10)]
	public string LastNotificationFarmingStatus { get; set; } = string.Empty;

	public DateTime? FarmingStatusTimestamp { get; set; }
	public string Id { get; set; } = null!;
	public DateTime? LastStatusNotificationTimestamp { get; set; }

	[ConcurrencyCheck]	
	public DateTime LastUpdated { get; set; }

	public DateTime? LastFarming { get; set; }

	public bool NotifyWhenOffline { get; set; }
	public bool? PublicApi { get; set; }

	[MaxLength(200)]
	public string User { get; set; } = null!;
}
