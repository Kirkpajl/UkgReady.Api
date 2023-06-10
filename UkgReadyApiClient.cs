using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using UkgReady.Api.Contract.Common;
using UkgReady.Api.Contract.Employees;
using UkgReady.Api.Contract.Login;
using UkgReady.Api.Converters;
using UkgReady.Api.Exceptions;
using UkgReady.Api.Extensions;
using UkgReady.Api.Helpers;
using UkgReady.Api.Models;

namespace UkgReady.Api
{
	public sealed class UkgReadyApiClient : IDisposable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the UkgReady.Api.UkgReadyApiClient class.
		/// </summary>
		/// <param name="baseAddress">The base URL of the API endpoints (i.e., "https://secure3.saashr.com/ta/rest/").</param>
		public UkgReadyApiClient(string baseAddress = "https://secure3.saashr.com/ta/rest/", string apiKey = default, string userName = default, string userPassword = default, string companyName = default, bool allowInvalidServerCertificates = false)
		{
			// Correct any BaseAddress formatting issues
			if (!baseAddress.EndsWith("/"))
				baseAddress += "/";

			// Create a handler to ignore SSL certificate validation errors (if directed)
			HttpClientHandler httpClientHandler = allowInvalidServerCertificates
				? new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator }
				: new HttpClientHandler();

			// Create an instance of HttpClient and assign the base address of the Web API.
			_httpClient = new HttpClient(httpClientHandler)
			{
				BaseAddress = new Uri(baseAddress)
			};
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("UkgReadyApiClient")));
			if (!string.IsNullOrWhiteSpace(apiKey))
				_httpClient.DefaultRequestHeaders.Add("Api-Key", apiKey);

			// Initialize the global JSON serialization options
			_jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
			_jsonOptions.Converters.Add(new JsonStringEnumConverter());
			_jsonOptions.Converters.Add(new DateOnlyConverter());
			_jsonOptions.Converters.Add(new EnumDescriptionConverter());
			_jsonOptions.Converters.Add(new EnumDisplayConverter());

			// Retain any UKG Ready configuration values
			_apiKey = apiKey;
			_userName = userName;
			_userPassword = userPassword;
			_companyName = companyName;
		}

		~UkgReadyApiClient()
		{
			Dispose(false);
		}

		#endregion Constructors

		#region Private Fields

		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _jsonOptions;
		private bool _disposed;

		private string _token, _apiKey, _userName, _userPassword, _companyName;
		private readonly object _syncLock = new object();

		#endregion Private Fields

		#region Public Properties

		/// <summary>
		/// Authorization Token for the current session
		/// </summary>
		public string Token => _token;



		/// <summary>
		/// UKG Ready User Account Name
		/// </summary>
		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}

		/// <summary>
		/// UKG Ready User Account Password
		/// </summary>
		public string UserPassword
		{
			get { return _userPassword; }
			set { _userPassword = value; }
		}

		/// <summary>
		/// UKG Ready Company Short Name
		/// </summary>
		public string CompanyName
		{
			get { return _companyName; }
			set { _companyName = value; }
		}

		#endregion Public Properties



		#region Login

		public Task LoginAsync(string apiKey, string username, string password, string companyName, CancellationToken cancellationToken = default)
		{
			// Update the connection properties
			_apiKey = apiKey;
			_userName = username;
			_userPassword = password;
			_companyName = companyName;

			_httpClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);

			// Execute the login request
			return LoginAsync(cancellationToken);
		}

		public async Task LoginAsync(CancellationToken cancellationToken = default)
		{
			// Execute the login request
			var request = new LoginRequest(_userName, _userPassword, _companyName);
			var response = await PostAsync<LoginRequest, LoginResponse>("v1/login", request, cancellationToken);

			// Cache the token
			SetToken(response.Token);
		}




		/// <summary>
		/// Update the private <see cref="_token"/> field in a thread-safe manner, as well as modifying the 'Authorization' header.
		/// </summary>
		/// <param name="token"></param>
		private void SetToken(string token)
		{
			// Set the token field value
			lock (_syncLock)
			{
				_token = token;
			}

			// Add/Remove the 'Authorization' header
			if (token == null)
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);
			}
			else
			{
				_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
				_httpClient.DefaultRequestHeaders.Remove("Api-Key");
			}
		}

		#endregion Login

		#region Employees

		/// <summary>
		/// Returns basic information about the employees a user has permissions to view, together with links to APIs that provide detailed information about each employee.
		/// </summary>
		/// <param name="einId">The ein id to get the employees if a company has multiple eins. All employees for all eins are returned if not specified. A list of company eins can be fetched using the ein lookup api.</param>
		/// <param name="terminated">Used to get either only terminated employees or only active employees. All employees are returned if not specified.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Returns basic information about the employees a user has permissions to view</returns>
		public async Task<Employee[]> GetEmployeesAsync(int? einId = null, bool? terminated = null, CancellationToken cancellationToken = default)
		{
			// Assemble the Request Uri
			var queryParameters = new Dictionary<string, string>();

			queryParameters.AddParameterIfNotNull("ein_id", einId);
			queryParameters.AddParameterIfNotNull("terminated", terminated);

			string requestUri = QueryHelpers.AddQueryString($"v2/companies/|{_companyName}/employees", queryParameters);

			// Process the request
			var response = await GetAsync<EmployeesResponse>(requestUri, cancellationToken);

			// Return the Employees collection
			return response.Employees;
		}

		/// <summary>
		/// Allows existing time entries to be fetched for specified company in given date range.
		/// </summary>
		/// <remarks>
		/// Maximum date range of request is 31 days.
		/// Response will contain all data that exists in time entry even if they do not displayed on UI due to settings.
		/// </remarks>
		/// <param name="startDate">Minimum date of time entry.</param>
		/// <param name="endDate">Maximum date of time entry.</param>
		/// <param name="isLight">Use light request containing only raw time entry data, no calculated time.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Returns time entries for the specified company  in the given date range.</returns>
		public async Task<TimeEntrySet[]> GetTimeEntriesAsync(DateTime startDate, DateTime endDate, bool? isLight = null, CancellationToken cancellationToken = default)
		{
			// Assemble the Request Uri
			var queryParameters = new Dictionary<string, string>();

			queryParameters.AddParameterIfNotNull("start_date", startDate);
			queryParameters.AddParameterIfNotNull("end_date", endDate);
			queryParameters.AddParameterIfNotNull("is_light", isLight);

			string requestUri = QueryHelpers.AddQueryString($"v2/companies/|{_companyName}/time-entries", queryParameters);

			// Process the request
			var response = await GetAsync<TimeEntriesResponse>(requestUri, cancellationToken);

			// Return the Employees collection
			return response.TimeEntrySets;
		}

		/// <summary>
		/// Allows existing time entries to be fetched for specified employee in given date range.
		/// </summary>
		/// <remarks>
		/// Maximum date range of request is 31 days.
		/// Response will contain all data that exists in time entry even if they do not displayed on UI due to settings.
		/// </remarks>
		/// <param name="accountId">Account should be referenced via account id.</param>
		/// <param name="startDate">Minimum date of time entry.</param>
		/// <param name="endDate">Maximum date of time entry.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Returns time entries for the specified employee in the given date range.</returns>
		public async Task<TimeEntrySet> GetEmployeeTimeEntriesAsync(long accountId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
		{
			// Assemble the Request Uri
			var queryParameters = new Dictionary<string, string>();

			queryParameters.AddParameterIfNotNull("start_date", startDate);
			queryParameters.AddParameterIfNotNull("end_date", endDate);

			string requestUri = QueryHelpers.AddQueryString($"v2/companies/|{_companyName}/employees/{accountId}/time-entries", queryParameters);

			// Process the request
			var response = await GetAsync<TimeEntrySet>(requestUri, cancellationToken);

			// Return the TimeEntrySet
			return response;
		}

		#endregion Employees



		#region API Helper Methods

		/// <summary>
		/// Send a GET request to the specified Uri as an asynchronous operation.
		/// </summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used by other objects or threads to receive
		/// notice of cancellation.
		/// </param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
		/// must be set.
		/// </exception>
		/// <exception cref="System.Net.Http.HttpRequestException">
		/// The request failed due to an underlying issue such as network connectivity, DNS
		/// failure, server certificate validation or timeout.
		/// </exception>
		/// <exception cref="System.Threading.Tasks.TaskCanceledException">
		/// .NET Core and .NET 5.0 and later only: The request failed due to timeout.
		/// </exception>
		private async Task<TResponse> GetAsync<TResponse>(string requestUri, CancellationToken cancellationToken = default)
		{
			// HTTP GET
			var response = await _httpClient.GetAsync(requestUri, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
			}
			else
			{
				throw await HandleRequestFailureAsync(requestUri, response, "Failed to complete HTTP GET request.", cancellationToken);
			}
		}



		/// <summary>
		/// Send a POST request with a cancellation token as an asynchronous operation.
		/// </summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used by other objects or threads to receive
		/// notice of cancellation.
		/// </param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
		/// must be set.
		/// </exception>
		/// <exception cref="System.Net.Http.HttpRequestException">
		/// The request failed due to an underlying issue such as network connectivity, DNS
		/// failure, server certificate validation or timeout.
		/// </exception>
		/// <exception cref="System.Threading.Tasks.TaskCanceledException">
		/// .NET Core and .NET 5.0 and later only: The request failed due to timeout.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private async Task PostAsync(string requestUri, CancellationToken cancellationToken = default)
		{
			// HTTP POST
			var response = await _httpClient.PostAsync(requestUri, null, cancellationToken: cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				return;
			}
			else
			{
				throw await HandleRequestFailureAsync(requestUri, response, "Failed to complete HTTP POST request.", cancellationToken);
			}
		}

		/// <summary>
		/// Send a POST request to the specified Uri containing the value serialized as JSON
		/// in the request body.
		/// </summary>
		/// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="requestData">The value to serialize.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used by other objects or threads to receive
		/// notice of cancellation.
		/// </param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
		/// must be set.
		/// </exception>
		/// <exception cref="System.Net.Http.HttpRequestException">
		/// The request failed due to an underlying issue such as network connectivity, DNS
		/// failure, server certificate validation or timeout.
		/// </exception>
		/// <exception cref="System.Threading.Tasks.TaskCanceledException">
		/// .NET Core and .NET 5.0 and later only: The request failed due to timeout.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private async Task PostAsync<TRequest>(string requestUri, TRequest requestData, CancellationToken cancellationToken = default)
		{
			// HTTP POST
			var response = await _httpClient.PostAsJsonAsync(requestUri, requestData, cancellationToken: cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				return;
			}
			else
			{
				throw await HandleRequestFailureAsync(requestUri, response, "Failed to complete HTTP POST request.", cancellationToken);
			}
		}

		/// <summary>
		/// Send a POST request with a cancellation token as an asynchronous operation.
		/// </summary>
		/// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="requestData">The value to serialize.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used by other objects or threads to receive
		/// notice of cancellation.
		/// </param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
		/// must be set.
		/// </exception>
		/// <exception cref="System.Net.Http.HttpRequestException">
		/// The request failed due to an underlying issue such as network connectivity, DNS
		/// failure, server certificate validation or timeout.
		/// </exception>
		/// <exception cref="System.Threading.Tasks.TaskCanceledException">
		/// .NET Core and .NET 5.0 and later only: The request failed due to timeout.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private async Task<TResponse> PostAsync<TResponse>(string requestUri, CancellationToken cancellationToken = default)
		{
			// HTTP POST
			var response = await _httpClient.PostAsync(requestUri, null, cancellationToken: cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
			}
			else
			{
				throw await HandleRequestFailureAsync(requestUri, response, "Failed to complete HTTP POST request.", cancellationToken);
			}
		}

		/// <summary>
		/// Send a POST request to the specified Uri containing the value serialized as JSON
		/// in the request body.
		/// </summary>
		/// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="requestData">The value to serialize.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used by other objects or threads to receive
		/// notice of cancellation.
		/// </param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
		/// must be set.
		/// </exception>
		/// <exception cref="System.Net.Http.HttpRequestException">
		/// The request failed due to an underlying issue such as network connectivity, DNS
		/// failure, server certificate validation or timeout.
		/// </exception>
		/// <exception cref="System.Threading.Tasks.TaskCanceledException">
		/// .NET Core and .NET 5.0 and later only: The request failed due to timeout.
		/// </exception>
		private async Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest requestData, CancellationToken cancellationToken = default)
		{
			// HTTP POST
			var response = await _httpClient.PostAsJsonAsync(requestUri, requestData, cancellationToken: cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
			}
			else
			{
				throw await HandleRequestFailureAsync(requestUri, response, "Failed to complete HTTP POST request.", cancellationToken);
			}
		}



		/// <summary>
		/// Send a PUT request with a cancellation token as an asynchronous operation.
		/// </summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used by other objects or threads to receive
		/// notice of cancellation.
		/// </param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
		/// must be set.
		/// </exception>
		/// <exception cref="System.Net.Http.HttpRequestException">
		/// The request failed due to an underlying issue such as network connectivity, DNS
		/// failure, server certificate validation or timeout.
		/// </exception>
		/// <exception cref="System.Threading.Tasks.TaskCanceledException">
		/// .NET Core and .NET 5.0 and later only: The request failed due to timeout.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private async Task PutAsync(string requestUri, CancellationToken cancellationToken = default)
		{
			// HTTP PUT
			var response = await _httpClient.PutAsync(requestUri, null, cancellationToken: cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				//_ = response.Content.ReadFromJsonAsync<TValue>(_jsonOptions, cancellationToken);
				return;
			}
			else
			{
				throw await HandleRequestFailureAsync(requestUri, response, "Failed to complete HTTP PUT request.", cancellationToken);
			}
		}

		/// <summary>
		/// Send a PUT request to the specified Uri containing the value serialized as JSON
		/// in the request body.
		/// </summary>
		/// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="requestData">The value to serialize.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used by other objects or threads to receive
		/// notice of cancellation.
		/// </param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
		/// must be set.
		/// </exception>
		/// <exception cref="System.Net.Http.HttpRequestException">
		/// The request failed due to an underlying issue such as network connectivity, DNS
		/// failure, server certificate validation or timeout.
		/// </exception>
		/// <exception cref="System.Threading.Tasks.TaskCanceledException">
		/// .NET Core and .NET 5.0 and later only: The request failed due to timeout.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private async Task PutAsync<TRequest>(string requestUri, TRequest requestData, CancellationToken cancellationToken = default)
		{
			// HTTP PUT
			var response = await _httpClient.PutAsJsonAsync(requestUri, requestData, cancellationToken: cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				//_ = response.Content.ReadFromJsonAsync<TValue>(_jsonOptions, cancellationToken);
				return;
			}
			else
			{
				throw await HandleRequestFailureAsync(requestUri, response, "Failed to complete HTTP PUT request.", cancellationToken);
			}
		}



		/// <summary>
		/// Send a DELETE request with a cancellation token as an asynchronous operation.
		/// </summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used by other objects or threads to receive
		/// notice of cancellation.
		/// </param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
		/// must be set.
		/// </exception>
		/// <exception cref="System.Net.Http.HttpRequestException">
		/// The request failed due to an underlying issue such as network connectivity, DNS
		/// failure, server certificate validation or timeout.
		/// </exception>
		/// <exception cref="System.Threading.Tasks.TaskCanceledException">
		/// .NET Core and .NET 5.0 and later only: The request failed due to timeout.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private async Task DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
		{
			// HTTP DELETE
			var response = await _httpClient.DeleteAsync(requestUri, cancellationToken: cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				//_ = response.Content.ReadFromJsonAsync<TValue>(_jsonOptions, cancellationToken);
				return;
			}
			else
			{
				throw await HandleRequestFailureAsync(requestUri, response, "Failed to complete HTTP DELETE request.", cancellationToken);
			}
		}



		/// <summary>
		/// Encapsulate the HttpResponseMessage in a UkgReadyOperationException object.
		/// </summary>
		/// <param name="requestUri"></param>
		/// <param name="httpResponse"></param>
		/// <param name="message"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="UkgReadyOperationException"></exception>
		private async Task<Exception> HandleRequestFailureAsync(string requestUri, HttpResponseMessage httpResponse, string message, CancellationToken cancellationToken = default)
		{
			// Extract the response's content as a string
			var content = await httpResponse.Content.ReadAsStringAsync();

			var ukgReadyError = await httpResponse.Content.ReadFromJsonAsync<UkgReadyErrorResponse>(_jsonOptions, cancellationToken);
			string errorMessage = ukgReadyError?.Errors?.FirstOrDefault()?.Message;

			// Dispose of the content object (if necessary)
			httpResponse.Content?.Dispose();

			// Throw the appropriate exception
			return httpResponse.StatusCode switch
			{
				HttpStatusCode.BadRequest => new UkgReadyOperationException(errorMessage ?? "Could not accept the request. Possibly a missing required parameter.", requestUri, httpResponse.StatusCode, content),
				HttpStatusCode.Unauthorized => new UkgReadyAuthenticationException(errorMessage ?? "Invalid session key or login credentials.", requestUri, httpResponse.StatusCode, content),
				HttpStatusCode.PaymentRequired => new UkgReadyOperationException(errorMessage ?? "The parameters were valid but the request failed.", requestUri, httpResponse.StatusCode, content),
				HttpStatusCode.Forbidden => new UkgReadyAuthorizationException(errorMessage ?? "The current user did not have the required access rights.", requestUri, httpResponse.StatusCode, content),
				HttpStatusCode.NotFound => new UkgReadyOperationException(errorMessage ?? "The path does not exist.", requestUri, httpResponse.StatusCode, content),
				HttpStatusCode.InternalServerError => new UkgReadyOperationException(errorMessage ?? "An error occurred on Fishbowl's end.", requestUri, httpResponse.StatusCode, content),
				_ => new UkgReadyOperationException(message + " - " + errorMessage, requestUri, httpResponse.StatusCode, content),
			};
		}

		#endregion API Helper Methods

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				// Terminate the session (if logged in)
				if (!string.IsNullOrWhiteSpace(Token))
					SetToken(null);  //Task.Run(() => LogoutAsync()).GetAwaiter().GetResult();

				// Dispose managed objects
				_httpClient.Dispose();
			}

			// Dispose unmanaged objects
			_disposed = true;
		}

		#endregion IDisposable Members
	}
}