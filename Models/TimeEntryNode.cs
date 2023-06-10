using System.Text.Json.Serialization;

namespace UkgReady.Api.Models
{
	public class TimeEntryNode
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }
	}
}
