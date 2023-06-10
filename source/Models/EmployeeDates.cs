using System;
using System.Text.Json.Serialization;

namespace UkgReady.Api.Models
{
	public class EmployeeDates
	{
		[JsonPropertyName("hired")]
		public DateTime Hired { get; set; }

		[JsonPropertyName("re_hired")]
		public DateTime ReHired { get; set; }

		[JsonPropertyName("started")]
		public DateTime Started { get; set; }

		[JsonPropertyName("terminated")]
		public DateTime Terminated { get; set; }
	}
}
