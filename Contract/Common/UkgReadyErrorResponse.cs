using System.Text.Json.Serialization;

namespace UkgReady.Api.Contract.Common
{
	internal class UkgReadyErrorResponse
	{
		[JsonPropertyName("errors")]
		public UkgReadyError[] Errors { get; set; }

		[JsonPropertyName("user_messages")]
		public UkgReadyUserMessage[] UserMessages { get; set; }
	}
}
