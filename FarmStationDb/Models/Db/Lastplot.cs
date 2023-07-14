using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmStation.Models.Db;

public partial class Lastplot
{
    [MaxLength(200)]
    public string Id { get; set; } = null!;

    [Column("Lastplot")]
    [MaxLength(200)]
    public string? Lastplot1 { get; set; }
}
