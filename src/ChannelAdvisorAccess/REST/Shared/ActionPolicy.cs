using System;
using System.Net.Http;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.REST.Exceptions;
using Polly;

namespace ChannelAdvisorAccess.REST.Shared
{
	public class ActionPolicy
	{
		private readonly int _retryAttempts;

		public ActionPolicy( int attempts )
		{
			if( attempts <= 0 )
			{
				throw new ArgumentOutOfRangeException( nameof(attempts), attempts, "attempts must be greater than 0" );
			}

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
					retryAttempt => TimeSpan.FromSeconds( Math.Pow( 2, retryAttempt ) ),
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
	}
}
