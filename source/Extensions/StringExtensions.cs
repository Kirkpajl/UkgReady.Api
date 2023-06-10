using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkgReady.Api.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Remove suffix from string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string RemoveFromEnd(this string s, string suffix) => s.EndsWith(suffix) ? s[..^suffix.Length] : s;
    }
}
