using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
		public void SynchItem( InventoryItemSubmit item, CancellationToken token, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { item, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );

				AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : parameters.ToJson() ) );
					if( !isCreateNew && !this.DoesSkuExist( item.Sku, token, mark ) )
						return;

					this.RefreshLastNetworkActivityTime();
					var resultOfBoolean = this._client.SynchInventoryItem( this._credentials, this.AccountId, item );
					this.RefreshLastNetworkActivityTime();

					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : parameters.ToJson() ) );
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task SynchItemAsync( InventoryItemSubmit item, CancellationToken token, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { item, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );

				await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : parameters.ToJson() ) );
					if( !isCreateNew && !( await this.DoesSkuExistAsync( item.Sku, token, mark ).ConfigureAwait( false ) ) )
						return;

					this.RefreshLastNetworkActivityTime();
					var resultOfBoolean = await this._client.SynchInventoryItemAsync( this._credentials, this.AccountId, item ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					CheckCaSuccess( resultOfBoolean.SynchInventoryItemResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : parameters.ToJson() ) );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void SynchItems( IEnumerable< InventoryItemSubmit > items, CancellationToken token, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { items, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );

				if( !isCreateNew )
				{
					var existSkus = this.DoSkusExist( items.Select( x => x.Sku ), token, mark ).Select( x => x.Sku );
					items = items.Where( x => existSkus.Contains( x.Sku ) );
				}

				var itemsByPages = ToChunks( items, 100 );
				foreach( var i in itemsByPages )
				{
					AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
						this.RefreshLastNetworkActivityTime();
						var resultOfBoolean = this._client.SynchInventoryItemList( this._credentials, this.AccountId, i.ToArray() );
						this.RefreshLastNetworkActivityTime();
						CheckCaSuccess( resultOfBoolean );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
					} );
				}
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items, CancellationToken token, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { items, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );

				if( !isCreateNew )
				{
					var existSkus = ( this.DoSkusExist( items.Select( x => x.Sku ), token, mark ) ).Select( x => x.Sku );
					items = items.Where( x => existSkus.Contains( x.Sku ) );
				}

				await items.DoWithPagesAsync( 100, async i => await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
						this.RefreshLastNetworkActivityTime();
						var resultOfBoolean = await this._client.SynchInventoryItemListAsync( this._credentials, this.AccountId, i.ToArray() ).ConfigureAwait( false );
						this.RefreshLastNetworkActivityTime();
						CheckCaSuccess( resultOfBoolean.SynchInventoryItemListResult );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : i.ToJson() ) );
					} ).ConfigureAwait( false );
				} ).ConfigureAwait( false ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : itemQuantityAndPrice.ToJson() ) );

				AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemQuantityAndPrice.ToJson() ) );
					this.RefreshLastNetworkActivityTime();
					var resultOfBoolean = this._client.UpdateInventoryItemQuantityAndPrice( this._credentials, this.AccountId, itemQuantityAndPrice );
					this.RefreshLastNetworkActivityTime();
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemQuantityAndPrice.ToJson() ) );
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfo(), methodParameters : itemQuantityAndPrice.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : itemQuantityAndPrice.ToJson() ) );

				await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemQuantityAndPrice.ToJson() ) );
					this.RefreshLastNetworkActivityTime();
					var resultOfBoolean = await this._client.UpdateInventoryItemQuantityAndPriceAsync( this._credentials, this.AccountId, itemQuantityAndPrice ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					CheckCaSuccess( resultOfBoolean.UpdateInventoryItemQuantityAndPriceResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemQuantityAndPrice.ToJson() ) );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfo(), methodParameters : itemQuantityAndPrice.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : itemQuantityAndPrices.ToJson() ) );

				var itemQuantityAndPricesByPages = ToChunks( itemQuantityAndPrices, 500 );
				foreach( var itemsPage in itemQuantityAndPricesByPages )
				{
					AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemsPage.ToJson() ) );
						this.RefreshLastNetworkActivityTime();
						var resultOfBoolean = this._client.UpdateInventoryItemQuantityAndPriceList( this._credentials, this.AccountId, itemsPage.ToArray() );
						this.RefreshLastNetworkActivityTime();
						CheckCaSuccess( resultOfBoolean );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemsPage.ToJson() ) );
					} );
				}
				
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfo(), methodParameters : itemQuantityAndPrices.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : itemQuantityAndPrices.ToJson() ) );

				var itemQuantityAndPricesByPages = ToChunks( itemQuantityAndPrices, 800 );
				await itemQuantityAndPricesByPages.DoInBatchAsync( 3, async itemsPage =>
				{
					await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemsPage.ToJson() ) );
						this.RefreshLastNetworkActivityTime();
						var result = await this._client.UpdateInventoryItemQuantityAndPriceListAsync( this._credentials, this.AccountId, itemsPage.ToArray() ).ConfigureAwait( false );
						this.RefreshLastNetworkActivityTime();
						CheckCaSuccess( result.UpdateInventoryItemQuantityAndPriceListResult );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : result.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : itemsPage.ToJson() ) );
					} ).ConfigureAwait( false );
				} ).ConfigureAwait( false );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void RemoveLabelListFromItemList( string[] labels, IEnumerable< string > skus, string reason, CancellationToken token, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { labels, skus, reason };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );

				var skusByPages = ToChunks( skus, 500 );
				foreach( var s in skusByPages )
				{
					AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
						this.RefreshLastNetworkActivityTime();
						var resultOfBoolean = this._client.RemoveLabelListFromInventoryItemList( this._credentials, this.AccountId, labels, s.ToArray(), reason );
						this.RefreshLastNetworkActivityTime();
						CheckCaSuccess( resultOfBoolean );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
					} );
				}

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task RemoveLabelListFromItemListAsync( string[] labels, IEnumerable< string > skus, string reason, CancellationToken token, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { labels, skus, reason };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );

				await skus.DoWithPagesAsync( 500, async s => await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
					this.RefreshLastNetworkActivityTime();
					var resultOfBoolean = await this._client.RemoveLabelListFromInventoryItemListAsync( this._credentials, this.AccountId, labels, s.ToArray(), reason ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					CheckCaSuccess( resultOfBoolean.RemoveLabelListFromInventoryItemListResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
				} ).ConfigureAwait( false ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, CancellationToken token, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { labels, createLabelIfNotExist, skus, reason };
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );

				var skusByPages = ToChunks( skus, 500 );
				foreach( var s in skusByPages )
				{
					AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
						this.RefreshLastNetworkActivityTime();
						var resultOfBoolean = this._client.AssignLabelListToInventoryItemList( this._credentials, this.AccountId, labels, createLabelIfNotExist, s.ToArray(), reason );
						this.RefreshLastNetworkActivityTime();
						CheckCaSuccess( resultOfBoolean );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
					} );
				}
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, CancellationToken token, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { labels, createLabelIfNotExist, skus, reason };

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );

				await skus.DoWithPagesAsync( 500, async s => await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
					this.RefreshLastNetworkActivityTime();
					var resultOfBoolean = await this._client.AssignLabelListToInventoryItemListAsync( this._credentials, this.AccountId, labels, createLabelIfNotExist, s.ToArray(), reason ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					CheckCaSuccess( resultOfBoolean.AssignLabelListToInventoryItemListResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : !this.LogDetailsEnum.HasFlag( LogDetailsEnum.LogParametersAndResultForRetry ) ? null : s.ToJson() ) );
				} ).ConfigureAwait( false ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : parameters.ToJson() ) );
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