using System;
using System.Net;
using Netco.Utils;

namespace ChannelAdvisorAccess.Misc
{
	public static class HandleError429
	{
		public static bool HasError429ForAccountId( string accountId, CacheManager cache )
		{
			var data = cache.Get< Data429Error >( accountId );

			if( data == null )
				return false;

			if( data.DateResolve <= DateTime.Now )
			{
				cache.Remove( accountId );
				return false;
			}

			return true;
		}

		public static void DoDelay()
		{
			var minutesLeftInTheHour = 62 - DateTime.UtcNow.Minute; // wait until current our ends + 2 extra minutes for buffer
			ChannelAdvisorLogger.LogTrace( string.Format( "Wait by reason of error 429 {0} minute(s)", minutesLeftInTheHour ) );

			SystemUtil.Sleep( TimeSpan.FromMinutes( minutesLeftInTheHour ) );
		}
	}
}
