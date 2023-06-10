using System.Text.Json.Serialization;
using UkgReady.Api.Contract.Common;
using UkgReady.Api.Models;

namespace UkgReady.Api.Contract.Employees
{
	class EmployeesResponse : IUkgReadyResponse
	{
		[JsonPropertyName("employees")]
		public Employee[] Employees { get; set; }
	}
}
