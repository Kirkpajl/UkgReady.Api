using System.Text.Json.Serialization;

namespace UkgReady.Api.Contract.Login
{
	class LoginCredentials
	{
		/// <summary>
		/// The username of the user logging in.
		/// </summary>
		[JsonPropertyName("username")]
		public string Username { get; set; }

		/// <summary>
		/// The user's password.
		/// </summary>
		[JsonPropertyName("password")]
		public string Password { get; set; }

		/// <summary>
		/// The company's short name.
		/// </summary>
		[JsonPropertyName("company")]
		public string Company { get; set; }
	}
}
