using System;
using System.Net.Http;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.REST.Exceptions;
using CuttingEdge.Conditions;
using Polly;

namespace ChannelAdvisorAccess.REST.Shared
{
	public class ActionPolicy
	{
		private readonly int _retryAttempts;
		private const int MaxRetryWaitTimeInSec = 600;

		public ActionPolicy( int attempts )
		{
			Condition.Requires( attempts ).IsGreaterThan( 0 );

			this._retryAttempts = attempts;
		}

		/// <summary>
		///	Retries function until it succeed or failed
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="funcToThrottle"></param>
		/// <param name="onRetryAttempt">Retry attempts</param>
		/// <returns></returns>
		public Task< TResult > ExecuteAsync< TResult >( Func< Task< TResult > > funcToThrottle, Action< Exception, TimeSpan, int > onRetryAttempt )
		{
			return Policy.Handle< ChannelAdvisorNetworkException >()
				.WaitAndRetryAsync( this._retryAttempts,
					retryAttempt => this.GetDelayBeforeNextAttempt( retryAttempt ),
					( exception, timeSpan, retryCount, context ) =>
					{
						if ( onRetryAttempt != null )
							onRetryAttempt.Invoke( exception, timeSpan, retryCount );
					})
				.ExecuteAsync( async () =>
				{
					try
					{
						return await funcToThrottle().ConfigureAwait( false );
					}
					catch ( Exception exception )
					{
						Exception caException = exception;

						if ( caException is ChannelAdvisorNetworkException )
							throw caException;

						if ( exception is HttpRequestException
								|| exception is ChannelAdvisorUnauthorizedException )
							caException = new ChannelAdvisorNetworkException( exception.Message, exception );
						else
						{
							if ( !( exception is ChannelAdvisorException ) )
								caException = new ChannelAdvisorException( exception.Message, exception );
						}

						throw caException;
					}
				});
		}

		public TimeSpan GetDelayBeforeNextAttempt( int retryAttempt )
		{
			return TimeSpan.FromSeconds( (int)Math.Min( MaxRetryWaitTimeInSec, Math.Pow( 2, retryAttempt ) ) );
		}
	}
}
