using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UkgReady.Api
{
	class Program
	{
		private static ApplicationConfig Configuration { get; } = LoadConfiguration();



		static async Task Main()
		{
			// Output UkgReady configuration
			Console.WriteLine("UkgReady.Api Test Client");
			Console.WriteLine("-------------------------------------");
			Console.WriteLine($"API Key:  {Configuration.ApiKey}");
			Console.WriteLine($"Username:  {Configuration.Username}");
			Console.WriteLine($"Password:  {new string('*', Configuration.Password.Length)}");
			Console.WriteLine($"Company Name:  {Configuration.CompanyName}");
			Console.WriteLine("-------------------------------------");
			Console.WriteLine("");

			// Initialize the UkgReady REST Api client
			using var ukgReady = new UkgReadyApiClient(
				apiKey: Configuration.ApiKey,
				userName: Configuration.Username, 
				userPassword: Configuration.Password, 
				companyName: Configuration.CompanyName,
				allowInvalidServerCertificates: true);

			// Terminate the previous Fishbowl session (if token is present), then start a new session
			bool isLoggedIn = await InitializeSession(ukgReady);

			if (isLoggedIn)
			{
				// Employees
				await TestEmployees(ukgReady);

				// Time Entries
				await TestTimeEntries(ukgReady);
				await TestEmployeeTimeEntries(ukgReady);

				Console.WriteLine("All tests completed.  Press any key to exit...");
			}
			else
			{
				Console.WriteLine("Unable to authenticate with KG Ready.  Testing has been cancelled.  Press any key to exit...");
			}

			// Pause execution
			Console.ReadKey();
		}



		private static async Task<bool> InitializeSession(UkgReadyApiClient ukgReady)
		{
			// Attempt to login
			try
			{
				Console.WriteLine("Initializing user session...");

				// Login to the UKG Ready server
				await ukgReady.LoginAsync();

				return true;
			}
			catch (Exception ex)
			{
				OutputException(ex, $"An {ex.GetType().Name} occurred while initializing the user session!");
				return false;
			}
		}

		private static async Task TestEmployees(UkgReadyApiClient ukgReady)
		{
			try
			{
				Console.WriteLine($"Testing Employees api...");

				var employees = await ukgReady.GetEmployeesAsync();
				employees = employees
					.OrderBy(e => e.LastName)
					.ThenBy(e => e.FirstName)
					.ToArray();

				foreach (var employee in employees)
				{
					Console.WriteLine($"  [{employee.Id}]:  {employee.LastName}, {employee.FirstName} - Emp Id:  {employee.EmployeeId}");
				}
				Console.WriteLine("");
			}
			catch (Exception ex)
			{
				OutputException(ex, $"An {ex.GetType().Name} occurred while testing the Employees Api!");
			}
		}

		private static async Task TestTimeEntries(UkgReadyApiClient ukgReady)
		{
			try
			{
				Console.WriteLine($"Testing Time Entries api...");

				var timeEntrySets = await ukgReady.GetTimeEntriesAsync(DateTime.Today, DateTime.Today, true);

				foreach (var timeEntrySet in timeEntrySets)
				{
					Console.WriteLine($"  [EMP {timeEntrySet.Employee.AccountId}]:  {timeEntrySet.StartDate:d} - {timeEntrySet.TimeEntries.Sum(t => t.Total.TotalHours):N2} hr");
				}
				Console.WriteLine("");
			}
			catch (Exception ex)
			{
				OutputException(ex, $"An {ex.GetType().Name} occurred while testing the Time Entries Api!");
			}
		}

		private static async Task TestEmployeeTimeEntries(UkgReadyApiClient ukgReady)
		{
			try
			{
				Console.WriteLine($"Testing Employee Time Entries api...");

				var employees = await ukgReady.GetEmployeesAsync();
				var employee = employees.FirstOrDefault();
				if (employee == null) return;

				var timeEntrySet = await ukgReady.GetEmployeeTimeEntriesAsync(employee.Id, DateTime.Today, DateTime.Today);
				Console.WriteLine($"  [EMP {timeEntrySet.Employee.AccountId}]:  {timeEntrySet.StartDate:d} - {timeEntrySet.TimeEntries.Sum(t => t.Total.TotalHours):N2} hr");
				Console.WriteLine("");
			}
			catch (Exception ex)
			{
				OutputException(ex, $"An {ex.GetType().Name} occurred while testing the Employee Time Entries Api!");
			}
		}



		#region Configuration Helper Methods

		private static ApplicationConfig LoadConfiguration() =>
			new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", true, true)
			.Build()
			.Get<ApplicationConfig>();

		#endregion Configuration Helper Methods



		/// <summary>
		/// Write the exception to the console with a custom format
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="message"></param>
		private static void OutputException(Exception ex, string message = null)
		{
			if (string.IsNullOrWhiteSpace(message))
				message = $"An {ex.GetType().Name} occurred while testing the UKG Ready REST Api!";

			var currentColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;

			Console.WriteLine(message);
			Console.WriteLine(ex.Message);
			Console.WriteLine(ex.StackTrace);
			Console.WriteLine();

			Console.ForegroundColor = currentColor;
		}
	}
}
