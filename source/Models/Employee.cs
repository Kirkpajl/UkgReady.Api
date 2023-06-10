using System.Text.Json.Serialization;

namespace UkgReady.Api.Models
{
	public class Employee
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("username")]
		public string Username { get; set; }

		[JsonPropertyName("first_name")]
		public string FirstName { get; set; }

		[JsonPropertyName("last_name")]
		public string LastName { get; set; }

		[JsonPropertyName("employee_id")]
		public string EmployeeId { get; set; }

		[JsonPropertyName("external_id")]
		public string ExternalId { get; set; }

		[JsonPropertyName("primary_account_id")]
		public int PrimaryAccountId { get; set; }

		[JsonPropertyName("ein_name")]
		public string EinName { get; set; }

		[JsonPropertyName("dates")]
		public EmployeeDates Dates { get; set; }

		[JsonPropertyName("status")]
		public string Status { get; set; }

		[JsonPropertyName("_links")]
		public EmployeeLinks Links { get; set; }
	}
}
