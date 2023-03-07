using System;
using System.Globalization;

namespace ChannelAdvisorAccess.REST.Extensions
{
	public static class DateExtensions
	{
		/// <summary>
		///	Convert date in format suitable for REST end point
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ToDateTimeOffset( this DateTime date )
		{
			return date.ToString( "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture );
		}
	}
}