using System;
using System.Text.Json.Serialization;

namespace UkgReady.Api.Models
{
	public class TimeEntry
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("date")]
		public DateTime Date { get; set; }

		[JsonPropertyName("type")]
		public string Type { get; set; }

		[JsonPropertyName("start_time")]
		public DateTime? StartTime { get; set; }

		[JsonPropertyName("end_time")]
		public DateTime? EndTime { get; set; }

		[JsonPropertyName("total")]
		public int TotalMilliseconds { get; set; }

		[JsonIgnore]
		public TimeSpan Total => TimeSpan.FromMilliseconds(TotalMilliseconds);



		[JsonPropertyName("time_off")]
		public TimeEntryNode TimeOff { get; set; }

		[JsonPropertyName("pay_category")]
		public TimeEntryNode PayCategory { get; set; }

		[JsonPropertyName("premium_shift")]
		public TimeEntryNode PremiumShift { get; set; }



		[JsonPropertyName("is_raw")]
		public bool IsRaw { get; }

		[JsonPropertyName("is_calc")]
		public bool IsCalc { get; }



		[JsonPropertyName("calc_start_time")]
		public DateTime? CalcStartTime { get; set; }
		
		[JsonPropertyName("calc_end_time")]
		public DateTime? CalcEndTime { get; set; }

		[JsonPropertyName("calc_total")]
		public int CalcTotalMilliseconds { get; set; }

		[JsonIgnore]
		public TimeSpan CalcTotal => TimeSpan.FromMilliseconds(CalcTotalMilliseconds);

		[JsonPropertyName("calc_pay_category")]
		public TimeEntryNode CalcPayCategory { get; set; }

		[JsonPropertyName("calc_premium_shift")]
		public TimeEntryNode CalcPremiumShift { get; set; }



		[JsonPropertyName("piecework")]
		public int Piecework { get; set; }

		[JsonPropertyName("amount")]
		public int Amount { get; set; }
	}
}
