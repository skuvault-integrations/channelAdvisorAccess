using System;
using System.Threading;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using Netco.Logging;

namespace ChannelAdvisorAccess.Services.Items
{
	public partial class ItemsService: IItemsService
	{
		#region Delete item
		public void DeleteItem( string sku, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : sku ) );

				AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : sku ) );					
					var resultOfBoolean = this._client.DeleteInventoryItem( this._credentials, this.AccountId, sku );
					this.RefreshLastNetworkActivityTime();
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : sku ) );
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : sku ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task DeleteItemAsync( string sku, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : sku ) );

				await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : sku ) );					
					var resultOfBoolean = await this._client.DeleteInventoryItemAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					CheckCaSuccess( resultOfBoolean.DeleteInventoryItemResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : sku ) );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : sku ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}
		#endregion}
	}
}