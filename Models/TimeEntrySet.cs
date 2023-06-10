using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace UkgReady.Api.Models
{
	public class TimeEntrySet
	{
		[JsonPropertyName("employee")]
		public TimeEntrySetEmployeeNode Employee { get; set; }

		[JsonPropertyName("start_date")]
		public DateTime StartDate { get; set; }

		[JsonPropertyName("end_date")]
		public DateTime EndDate { get; set; }

		[JsonPropertyName("time_entries")]
		public TimeEntry[] TimeEntries { get; set; }

		[JsonIgnore]
		public TimeSpan Total => TimeEntries == null ? TimeSpan.Zero : TimeSpan.FromHours(TimeEntries.Sum(t => t.Total.TotalHours));
	}
}
