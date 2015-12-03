using System;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;

namespace ChannelAdvisorAccess.Services.Items
{
	public partial class ItemsService: IItemsService
	{
		#region Get Config Info
		public ClassificationConfigurationInformation[] GetClassificationConfigInfo( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				var classificationConfigurationInformations = AP.CreateQuery( this.AdditionalLogInfo ).Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var result = this._client.GetClassificationConfigurationInformation( this._credentials, this.AccountId );
					CheckCaSuccess( result );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : result.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					return result.ResultData;
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : classificationConfigurationInformations.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				return classificationConfigurationInformations;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< ClassificationConfigurationInformation[] > GetClassificationConfigInfoAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				var classificationConfigurationInformations = await AP.CreateQueryAsync( this.AdditionalLogInfo ).Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var result = await this._client.GetClassificationConfigurationInformationAsync( this._credentials, this.AccountId ).ConfigureAwait( false );
					CheckCaSuccess( result.GetClassificationConfigurationInformationResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : result.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					return result.GetClassificationConfigurationInformationResult.ResultData;
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : classificationConfigurationInformations.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				return classificationConfigurationInformations;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public DistributionCenterResponse[] GetDistributionCenterList( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				var distributionCenterResponses = AP.CreateQuery( this.AdditionalLogInfo ).Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var result = this._client.GetDistributionCenterList( this._credentials, this.AccountId );
					this.CheckCaSuccess( result );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : result.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					return result.ResultData;
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : distributionCenterResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				return distributionCenterResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< DistributionCenterResponse[] > GetDistributionCenterListAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				var distributionCenterResponses = await AP.CreateQueryAsync( this.AdditionalLogInfo ).Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var result = await this._client.GetDistributionCenterListAsync( this._credentials, this.AccountId ).ConfigureAwait( false );
					this.CheckCaSuccess( result.GetDistributionCenterListResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : result.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					return result.GetDistributionCenterListResult.ResultData;
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : distributionCenterResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				return distributionCenterResponses;
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