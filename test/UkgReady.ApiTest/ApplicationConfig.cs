using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UkgReady.Api
{
    public class ApplicationConfig
    {
        [JsonPropertyName("baseUrl")]
        public string BaseUrl { get; set; } //= "https://secure3.saashr.com/ta/rest/";



        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }

		[JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

		[JsonPropertyName("companyName")]
		public string CompanyName { get; set; }
    }
}
