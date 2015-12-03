using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;
using CuttingEdge.Conditions;
using Netco.Extensions;

namespace ChannelAdvisorAccess.Services.Items
{
	public partial class ItemsService: IItemsService
	{
		#region Update items
		public void SynchItem( InventoryItemSubmit item, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { item, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				AP.CreateSubmit( this.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : parameters.ToJson() ) );
					if( !isCreateNew && !this.DoesSkuExist( item.Sku, mark ) )
						return;

					var resultOfBoolean = this._client.SynchInventoryItem( this._credentials, this.AccountId, item );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : parameters.ToJson() ) );
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task SynchItemAsync( InventoryItemSubmit item, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { item, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				await AP.CreateSubmitAsync( this.AdditionalLogInfo ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : parameters.ToJson() ) );
					if( !isCreateNew && !( await this.DoesSkuExistAsync( item.Sku, mark ) ) )
						return;

					var resultOfBoolean = await this._client.SynchInventoryItemAsync( this._credentials, this.AccountId, item ).ConfigureAwait( false );
					CheckCaSuccess( resultOfBoolean.SynchInventoryItemResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : parameters.ToJson() ) );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void SynchItems( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { items, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				if( !isCreateNew )
				{
					var existSkus = this.DoSkusExist( items.Select( x => x.Sku ), mark ).Select( x => x.Sku );
					items = items.Where( x => existSkus.Contains( x.Sku ) );
				}

				items.DoWithPages( 100, i => AP.CreateSubmit( this.AdditionalLogInfo ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
					var resultOfBoolean = this._client.SynchInventoryItemList( this._credentials, this.AccountId, i.ToArray() );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
				} ) );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { items, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				if( !isCreateNew )
				{
					var existSkus = ( this.DoSkusExist( items.Select( x => x.Sku ), mark ) ).Select( x => x.Sku );
					items = items.Where( x => existSkus.Contains( x.Sku ) );
				}

				await items.DoWithPagesAsync( 100, async i => await AP.CreateSubmitAsync( this.AdditionalLogInfo ).Do( async () =>
				{
					await AP.CreateSubmitAsync( this.AdditionalLogInfo ).Do( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
						var resultOfBoolean = await this._client.SynchInventoryItemListAsync( this._credentials, this.AccountId, i.ToArray() ).ConfigureAwait( false );
						CheckCaSuccess( resultOfBoolean.SynchInventoryItemListResult );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
					} ).ConfigureAwait( false );
				} ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );

				AP.CreateSubmit( this.AdditionalLogInfo ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemQuantityAndPrice.ToJson() ) );
					var resultOfBoolean = this._client.UpdateInventoryItemQuantityAndPrice( this._credentials, this.AccountId, itemQuantityAndPrice );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemQuantityAndPrice.ToJson() ) );
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );

				await AP.CreateSubmitAsync( this.AdditionalLogInfo ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemQuantityAndPrice.ToJson() ) );
					var resultOfBoolean = await this._client.UpdateInventoryItemQuantityAndPriceAsync( this._credentials, this.AccountId, itemQuantityAndPrice ).ConfigureAwait( false );
					CheckCaSuccess( resultOfBoolean.UpdateInventoryItemQuantityAndPriceResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemQuantityAndPrice.ToJson() ) );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
				itemQuantityAndPrices.DoWithPages( 5000, itemsPage => AP.CreateSubmit( this.AdditionalLogInfo ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemsPage.ToJson() ) );
					var resultOfBoolean = this._client.UpdateInventoryItemQuantityAndPriceList( this._credentials, this.AccountId, itemsPage.ToArray() );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemsPage.ToJson() ) );
				} ) );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );

				await itemQuantityAndPrices.DoWithPagesAsync( 500, async i => await AP.CreateSubmitAsync( this.AdditionalLogInfo ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
					var result = await this._client.UpdateInventoryItemQuantityAndPriceListAsync( this._credentials, this.AccountId, i.ToArray() ).ConfigureAwait( false );
					CheckCaSuccess( result.UpdateInventoryItemQuantityAndPriceListResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : result.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
				} ).ConfigureAwait( false ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void RemoveLabelListFromItemList( string[] labels, IEnumerable< string > skus, string reason, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { labels, skus, reason };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				skus.DoWithPages( 500, s => AP.CreateSubmit( this.AdditionalLogInfo ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
					var resultOfBoolean = this._client.RemoveLabelListFromInventoryItemList( this._credentials, this.AccountId, labels, s.ToArray(), reason );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
				} ) );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task RemoveLabelListFromItemListAsync( string[] labels, IEnumerable< string > skus, string reason, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { labels, skus, reason };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				await skus.DoWithPagesAsync( 500, async s => await AP.CreateSubmitAsync( this.AdditionalLogInfo ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
					var resultOfBoolean = await this._client.RemoveLabelListFromInventoryItemListAsync( this._credentials, this.AccountId, labels, s.ToArray(), reason ).ConfigureAwait( false );
					CheckCaSuccess( resultOfBoolean.RemoveLabelListFromInventoryItemListResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
				} ).ConfigureAwait( false ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { labels, createLabelIfNotExist, skus, reason };
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				skus.DoWithPages( 500, s => AP.CreateSubmit( this.AdditionalLogInfo ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
					var resultOfBoolean = this._client.AssignLabelListToInventoryItemList( this._credentials, this.AccountId, labels, createLabelIfNotExist, s.ToArray(), reason );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
				} ) );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { labels, createLabelIfNotExist, skus, reason };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				await skus.DoWithPagesAsync( 500, async s => await AP.CreateSubmitAsync( this.AdditionalLogInfo ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
					var resultOfBoolean = await this._client.AssignLabelListToInventoryItemListAsync( this._credentials, this.AccountId, labels, createLabelIfNotExist, s.ToArray(), reason ).ConfigureAwait( false );
					CheckCaSuccess( resultOfBoolean.AssignLabelListToInventoryItemListResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
				} ).ConfigureAwait( false ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		private Func< string > CreateMethodCallInfo( Func< string > additionalLogInfo, [ CallerMemberName ] string callerMemberName = "" )
		{
			var message = "MethodInfo: {0}, ".FormatWith( callerMemberName );
			return () => message + additionalLogInfo();
		}
		#endregion}
	}
}