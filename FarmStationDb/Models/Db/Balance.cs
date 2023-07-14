using System.ComponentModel.DataAnnotations.Schema;

namespace FarmStation.Models;

public partial class Balance
{
    public string Id { get; set; } = null!;


    [Column("Balance")]
    public string? Balance1 { get; set; }
}
