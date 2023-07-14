using System.ComponentModel.DataAnnotations;

namespace FarmStation.Models.Db;

public partial class Drive
{
    [MaxLength(200)]
    public string Id { get; set; } = null!;

    public float? Drives { get; set; }
}
