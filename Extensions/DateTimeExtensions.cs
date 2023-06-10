using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkgReady.Api.Extensions
{
    internal static class DateTimeExtensions
    {
        public static string ToFishbowlDateString(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");
    }
}
