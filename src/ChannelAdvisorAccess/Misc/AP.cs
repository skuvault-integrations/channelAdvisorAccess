using System;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Extensions;
using Netco.Utils;

namespace ChannelAdvisorAccess.Misc
{
	public static class AP
	{
		private static readonly ActionPolicy _query = CreateQuery();
		private static readonly ActionPolicyAsync _queryAsync = CreateQueryAsync();
		private static readonly ActionPolicy _sumbit = CreateSubmit();
		private static readonly ActionPolicyAsync _sumbitAsync = CreateSubmitAsync();

		public static ActionPolicy Query
		{
			get { return _query; }
		}

		public static ActionPolicyAsync QueryAsync
		{
			get { return _queryAsync; }
		}

		public static ActionPolicy Submit
		{
			get { return _sumbit; }
		}

		public static ActionPolicyAsync SubmitAsync
		{
			get { return _sumbitAsync; }
		}

		#region ActionPolicyCreator
		public static ActionPolicy CreateQuery( Func< string > additionalLogInfo = null )
		{
			return ActionPolicy.Handle< Exception >().Retry( 10, ( ex, i ) =>
			{
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );
				SystemUtil.Sleep( delay );
			} );
		}

		public static ActionPolicyAsync CreateQueryAsync( Func< string > additionalLogInfo = null )
		{
			return ActionPolicyAsync.Handle< Exception >().RetryAsync( 10, async ( ex, i ) =>
			{
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );
				await Task.Delay( delay );
			} );
		}

		public static ActionPolicy CreateSubmit( Func< string > additionalLogInfo = null )
		{
			return ActionPolicy.Handle< Exception >().Retry( 10, ( ex, i ) =>
			{
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );
				SystemUtil.Sleep( delay );
			} );
		}

		public static ActionPolicyAsync CreateSubmitAsync( Func< string > additionalLogInfo = null )
		{
			return ActionPolicyAsync.Handle< Exception >().RetryAsync( 10, async ( ex, i ) =>
			{
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );
				await Task.Delay( delay );
			} );
		}
		#endregion

		#region Misc
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

			if( ex != null && ex.InnerException != null )
			{
				var pwe = ex.InnerException as WebException;
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

		private static string CollestMessages( Exception exc )
		{
			try
			{
				if( exc == null )
					return string.Empty;

				return "{0},{1}".FormatWith( exc.Message, CollestMessages( exc.InnerException ) );
			}
			catch( Exception )
			{
				return string.Empty;
			}
		}

		private static string CreateRetryMessage( Func< string > additionalLogInfo, int i, TimeSpan delay, Exception ex )
		{
			return "Retrying CA API get call for the {0} time, delay:{1}, Additional info: {2}, ExceptionSummary {3}".FormatWith( i, delay, ( additionalLogInfo ?? ( () => string.Empty ) )(), CollestMessages( ex ) );
		}
		#endregion
	}
}