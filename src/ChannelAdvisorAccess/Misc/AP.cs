using System;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Netco.ActionPolicyServices;
using Netco.Extensions;
using Netco.Utils;

namespace ChannelAdvisorAccess.Misc
{
	public static class AP
	{
		private const int RetryCount = 10;
		private const int Wait429After = 5;

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
			return ActionPolicy.Handle< Exception >().Retry( RetryCount, ( ex, i ) =>
			{
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );

				SystemUtil.Sleep( delay );
			} );
		}

		public static ActionPolicyAsync CreateQueryAsync( Func< string > additionalLogInfo = null )
		{
			return ActionPolicyAsync.Handle< Exception >().RetryAsync( RetryCount, async ( ex, i ) =>
			{
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );

				await Task.Delay( delay ).ConfigureAwait( false );
			} );
		}

		public static ActionPolicyAsync CreateQueryAsync( Func< string > additionalLogInfo, CancellationToken cancellationToken )
		{
			return ActionPolicyAsync.Handle< Exception >().RetryAsync( RetryCount, async ( ex, i ) =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );

				await Task.Delay( delay, cancellationToken ).ConfigureAwait( false );
			} );
		}

		public static ActionPolicy CreateSubmit( Func< string > additionalLogInfo = null )
		{
			return ActionPolicy.Handle< Exception >().Retry( RetryCount, ( ex, i ) =>
			{
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );

				SystemUtil.Sleep( delay );
			} );
		}

		public static ActionPolicyAsync CreateSubmitAsync( Func< string > additionalLogInfo = null )
		{
			return ActionPolicyAsync.Handle< Exception >().RetryAsync( RetryCount, async ( ex, i ) =>
			{
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );

				await Task.Delay( delay ).ConfigureAwait( false );
			} );
		}

		public static ActionPolicyAsync CreateSubmitAsync( Func< string > additionalLogInfo, CancellationToken cancellationToken )
		{
			return ActionPolicyAsync.Handle< Exception >().RetryAsync( RetryCount, async ( ex, i ) =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				var delay = GetDelay( ex, i );
				var message = CreateRetryMessage( additionalLogInfo, i, delay, ex );
				ChannelAdvisorLogger.LogTrace( ex, message );

				await Task.Delay( delay, cancellationToken ).ConfigureAwait( false );
			} );
		}
		#endregion

		#region Misc
		private static bool IsError429( Exception ex )
		{
			var webException = ex as WebException;
			var status = GetHttpStatusCode( webException );
			if( status.HasValue && ( int )status.Value == 429 )
				return true;

			var protocolException = ex as ProtocolException;
			if( protocolException != null )
			{
				var pwe = protocolException.InnerException as WebException;
				status = GetHttpStatusCode( pwe );
				if( status.HasValue && ( int )status.Value == 429 )
					return true;
			}

			if( ex != null && ex.InnerException != null )
			{
				var pwe = ex.InnerException as WebException;
				status = GetHttpStatusCode( pwe );
				if( status.HasValue && ( int )status.Value == 429 )
					return true;
			}

			return false;
		}

		private static TimeSpan GetDelay( Exception ex, int retryNumber )
		{
			if( ( retryNumber > Wait429After ) && IsError429( ex ) )
				return GetDelayFor429Exception();

			return TimeSpan.FromSeconds( 0.5 + retryNumber );
		}

		private static TimeSpan GetDelayFor429Exception()
		{
			var minutesLeftInTheHour = 62 - DateTime.UtcNow.Minute; // wait until current our ends + 2 extra minutes for buffer
			return TimeSpan.FromMinutes( minutesLeftInTheHour );
		}

		private static HttpStatusCode? GetHttpStatusCode( WebException webException )
		{
			if( webException != null )
			{
				var response = webException.Response as HttpWebResponse;
				if( response != null )
					return response.StatusCode;
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