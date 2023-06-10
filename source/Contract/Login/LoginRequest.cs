using System.Text.Json.Serialization;
using UkgReady.Api.Contract.Common;

namespace UkgReady.Api.Contract.Login
{
	/// <summary>
	/// This logs the user in to Fishbowl and returns the list of Access Rights.
	/// </summary>
	/// <remarks>
	/// The first time your Integrated App logs in, you'll need to approve the App in Fishbowl.
	/// </remarks>
	class LoginRequest : IUkgReadyRequest
	{
		public LoginRequest(string username, string password, string company)
		{
			Credentials = new LoginCredentials
			{
				Username = username,
				Password = password,
				Company = company
			};
		}



		[JsonPropertyName("credentials")]
		public LoginCredentials Credentials { get; set; }
	}
}
