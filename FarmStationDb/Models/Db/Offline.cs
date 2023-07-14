using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmStation.Models.Db;

[Table("Offline")]
public partial class Offline
{
	[MaxLength(200)]
	public string Id { get; set; } = null!;

	[MaxLength(200)]
	public string? Name { get; set; }

	public int? Notify { get; set; }
}
