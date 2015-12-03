using System;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;

namespace ChannelAdvisorAccess.Services.Items
{
	public partial class ItemsService: IItemsService
	{
		#region Ping
		public void Ping( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				AP.CreateQuery( this.AdditionalLogInfo ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var result = this._client.Ping( this._credentials );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : result.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					this.CheckCaSuccess( result );
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task PingAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				await AP.CreateQueryAsync( this.AdditionalLogInfo ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var result = await this._client.PingAsync( this._credentials ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : result.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					this.CheckCaSuccess( result.PingResult );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}
		#endregion
	}
}