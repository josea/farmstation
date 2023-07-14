using System.ComponentModel.DataAnnotations;

namespace FarmStation.Models.Db;

public partial class Status
{
	[MaxLength(200)]
	public string Id { get; set; } = null!;

    public int? Isfarming { get; set; }
}
