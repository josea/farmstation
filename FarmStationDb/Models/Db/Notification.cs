using System.ComponentModel.DataAnnotations;

namespace FarmStation.Models.Db;

public partial class Notification
{
    public int NotificationId { get; set; }

    [MaxLength(200)]
    public string? User { get; set; }

	[MaxLength(200)]
	public string? Type { get; set; }

	[MaxLength(200)]
	public string? Name { get; set; }
}
