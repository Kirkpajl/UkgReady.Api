using System.Text.Json.Serialization;

namespace UkgReady.Api.Models
{
	public class EmployeeLinks
	{
		[JsonPropertyName("self")]
		public string Self { get; set; }

		[JsonPropertyName("demographics")]
		public string Demographics { get; set; }

		[JsonPropertyName("pay-info")]
		public string PayInfo { get; set; }

		[JsonPropertyName("badges")]
		public string Badges { get; set; }

		[JsonPropertyName("profiles")]
		public string Profiles { get; set; }

		[JsonPropertyName("hcm-profiles")]
		public string HcmProfiles { get; set; }

		[JsonPropertyName("hcm-fields")]
		public string HcmFields { get; set; }
	}
}
