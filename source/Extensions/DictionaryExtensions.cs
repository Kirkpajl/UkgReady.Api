using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkgReady.Api.Extensions
{
    internal static class DictionaryExtensions
    {
        public static bool AddParameterIfNotNull<T>(this Dictionary<string, string> parameters, string name, T value)
        {
            // Ensure that the required arguments were provided
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) return false;

            // Parse the value to the appropriate string
            string valueString = value.ToString();            

            if (value is Enum e) valueString = e.GetDescription();  //if (value.GetType().IsEnum) valueString = (value as Enum).GetDescription();
            if (value is DateTime dt) valueString = dt.ToFishbowlDateString();

            // Add the parameter to the dictionary
            parameters.Add(name, valueString);

            return true;


            //if (name != null) queryParameters.Add("name", name);
            //if (active != null) queryParameters.Add("active", active?.ToString());
        }
    }
}
