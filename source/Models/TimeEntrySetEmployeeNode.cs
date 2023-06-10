using System.Text.Json.Serialization;

namespace UkgReady.Api.Models
{
	public class TimeEntrySetEmployeeNode
	{
		[JsonPropertyName("account_id")]
		public long AccountId { get; set; }
	}
}
