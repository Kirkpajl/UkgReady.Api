using System.Text.Json.Serialization;
using UkgReady.Api.Contract.Common;

namespace UkgReady.Api.Contract.Login
{
	class LoginResponse : IUkgReadyResponse
	{
		[JsonPropertyName("token")]
		public string Token { get; set; }

		[JsonPropertyName("ttl")]
		public int TimeToLive { get; set; }

		[JsonPropertyName("units")]
		public string TimeToLiveUnits { get; set; }
	}
}
