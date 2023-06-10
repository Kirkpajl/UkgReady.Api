using System.Text.Json.Serialization;

namespace UkgReady.Api.Contract.Common
{
	internal class UkgReadyUserMessage
	{
		[JsonPropertyName("severity")]
		public string Severity { get; set; }

		[JsonPropertyName("text")]
		public string Text { get; set; }

		[JsonPropertyName("details")]
		public UkgReadyUserMessageDetails Details { get; set; }
	}
}
