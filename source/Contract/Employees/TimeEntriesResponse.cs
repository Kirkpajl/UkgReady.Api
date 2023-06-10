using System.Text.Json.Serialization;
using UkgReady.Api.Contract.Common;
using UkgReady.Api.Models;

namespace UkgReady.Api.Contract.Employees
{
	class TimeEntriesResponse : IUkgReadyResponse
	{
		[JsonPropertyName("time_entry_sets")]
		public TimeEntrySet[] TimeEntrySets { get; set; }
	}
}
