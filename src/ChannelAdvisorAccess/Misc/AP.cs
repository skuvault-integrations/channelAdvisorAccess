using System;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Logging;
using Netco.Utils;

namespace ChannelAdvisorAccess.Misc
{
	public static class AP
	{
		public static ActionPolicy Query
		{
			get { return _query; }
		}

		private static readonly ActionPolicy _query = ActionPolicy.Handle< Exception >().Retry( 10, ( ex, i ) =>
		{
			typeof( AP ).Log().Trace( ex, "Retrying CA API get call for the {0} time", i );
			SystemUtil.Sleep( GetDelay( ex, i ) );
		} );

		public static ActionPolicyAsync QueryAsync
		{
			get { return _queryAsync; }
		}

		private static readonly ActionPolicyAsync _queryAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( 10, async ( ex, i ) =>
		{
			typeof( AP ).Log().Trace( ex, "Retrying CA API get call for the {0} time", i );
			await Task.Delay( GetDelay( ex, i ) );
		} );

		public static ActionPolicy Submit
		{
			get { return _sumbit; }
		}

		private static readonly ActionPolicy _sumbit = ActionPolicy.Handle< Exception >().Retry( 10, ( ex, i ) =>
		{
			typeof( AP ).Log().Trace( ex, "Retrying CA API submit call for the {0} time", i );
			SystemUtil.Sleep( GetDelay( ex, i ) );
		} );

		public static ActionPolicyAsync SubmitAsync
		{
			get { return _sumbitAsync; }
		}

		private static readonly ActionPolicyAsync _sumbitAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( 10, async ( ex, i ) =>
		{
			typeof( AP ).Log().Trace( ex, "Retrying CA API submit call for the {0} time", i );
			await Task.Delay( GetDelay( ex, i ) );
		} );

		private static TimeSpan GetDelay( Exception ex, int retryNumber )
		{
			var webException = ex as WebException;
			var delay = GetDelayForWebException( webException );
			if( delay != null )
				return delay.Value;

			var protoclException = ex as ProtocolException;
			if( protoclException != null )
			{
				var pwe = protoclException.InnerException as WebException;
				var delayPwe = GetDelayForWebException( pwe );
				if( delayPwe != null )
					return delayPwe.Value;
			}

			return TimeSpan.FromSeconds( 0.5 + retryNumber );
		}

		private static TimeSpan? GetDelayForWebException( WebException webException )
		{
			if( webException != null )
			{
				var response = webException.Response as HttpWebResponse;
				if( response != null )
				{
					if( ( int )response.StatusCode == 429 )
					{
						var minutesLeftInTheHour = 62 - DateTime.UtcNow.Minute; // wait until current our ends + 2 extra minutes for buffer
						return TimeSpan.FromMinutes( minutesLeftInTheHour );
					}
				}
			}
			return null;
		}
	}
}