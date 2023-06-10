using System.Text.Json.Serialization;

namespace UkgReady.Api.Contract.Common
{
	internal class UkgReadyError
	{
		[JsonPropertyName("code")]
		public int Code { get; set; }

		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
