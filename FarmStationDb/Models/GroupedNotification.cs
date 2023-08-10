using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmStationDb.Models;

public class GroupedNotification
{
	public int Count { get; set; }
	public DateTime MinTimeStamp { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Type { get; set; } = string.Empty;
	public string User { get; set; } = string.Empty;
}
