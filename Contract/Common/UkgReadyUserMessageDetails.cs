using System.Text.Json.Serialization;

namespace UkgReady.Api.Contract.Common
{
	internal class UkgReadyUserMessageDetails
	{
		[JsonPropertyName("code")]
		public int Code { get; set; }
	}
}
