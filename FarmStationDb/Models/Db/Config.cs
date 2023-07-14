using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmStation.Models.Db;

public partial class Config
{
    [MaxLength(200)]
    public string Id { get; set; } = null!;

    [MaxLength(60000)]
    [Column("Config")]
    public string? Config1 { get; set; }
}
