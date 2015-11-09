using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Extensions;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;
using CuttingEdge.Conditions;
using Netco.Extensions;
using Netco.Logging;
using Newtonsoft.Json;

namespace ChannelAdvisorAccess.Services.Items
{
	public class ItemsService: IItemsService
	{
		private readonly APICredentials _credentials;
		private readonly InventoryServiceSoapClient _client;

		private readonly ObjectCache _cache;
		private readonly string _allItemsCacheKey;
		private readonly object _inventoryCacheLock = new Object();

		[ JsonIgnore ]
		public Func< string > AdditionalLogInfo{ get; set; }

		private string AdditionalLogInfoString
		{
			get
			{
				if( this.AdditionalLogInfo == null )
					return null;

				string res;
				try
				{
					res = this.AdditionalLogInfo();
				}
				catch
				{
					return null;
				}

				return res;
			}
		}
		public string Name{ get; private set; }
		public string AccountId{ get; private set; }
		public TimeSpan SlidingCacheExpiration{ get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ItemsService"/> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="name">The account user-friendly name.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="cache">The cache.</param>
		/// <remarks>If <paramref name="cache"/> is <c>null</c> no caching takes place.</remarks>
		public ItemsService( APICredentials credentials, string name, string accountId, ObjectCache cache = null )
		{
			this._credentials = credentials;
			this.AccountId = accountId;
			this._client = new InventoryServiceSoapClient();

			this.Name = name;
			this._cache = cache;
			this.SlidingCacheExpiration = ObjectCache.NoSlidingExpiration;
			this._allItemsCacheKey = string.Format( "caAllItems_ID_{0}", this.AccountId );
		}

		#region Ping
		public void Ping( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				AP.Query.Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var result = this._client.Ping( this._credentials );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : result.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					this.CheckCaSuccess( result );
				} );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				await AP.QueryAsync.Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var result = await this._client.PingAsync( this._credentials ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : result.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					this.CheckCaSuccess( result.PingResult );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}
		#endregion

		#region Get items
		public bool DoesSkuExist( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku.ToJson() ) );
				var skuExist = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku.ToJson() ) );
					var result = this._client.DoesSkuExist( this._credentials, this.AccountId, sku );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : result.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku.ToJson() ) );
					return result;
				} );

				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : skuExist.ToJson(), methodParameters : sku.ToJson() ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( skuExist, skuExist.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : resultWithSuccessCheck.ToJson(), methodParameters : sku.ToJson() ) );
				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku.ToJson() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< bool > DoesSkuExistAsync( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku.ToJson() ) );
				var skuExist = await AP.QueryAsync.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku.ToJson() ) );
					var result = this._client.DoesSkuExistAsync( this._credentials, this.AccountId, sku );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : result.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku.ToJson() ) );
					return result;
				}
					);
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : skuExist.ToJson(), methodParameters : sku.ToJson() ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( skuExist, skuExist.DoesSkuExistResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : resultWithSuccessCheck.ToJson(), methodParameters : sku.ToJson() ) );
				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable< string > skus, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
				var doesSkuExistResponses = skus.ProcessWithPages( 500, skusPage =>
					AP.Query.Get( () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
						var skusResult = this._client.DoesSkuExistList( this._credentials, this.AccountId, skusPage.ToArray() );
						var resultWithSuccessCheck = this.GetResultWithSuccessCheck( skusResult, skusResult.ResultData );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
						return resultWithSuccessCheck;
					} ) );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : doesSkuExistResponses.ToJson(), methodParameters : skus.ToJson() ) );
				return doesSkuExistResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< IEnumerable< DoesSkuExistResponse > > DoSkusExistAsync( IEnumerable< string > skus, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
				var doesSkuExistResponses = await skus.ProcessWithPagesAsync< string, DoesSkuExistResponse >( 500, async skusPage =>
					await AP.QueryAsync.Get( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
						var skusResult = await this._client.DoesSkuExistListAsync( this._credentials, this.AccountId, skusPage.ToArray() ).ConfigureAwait( false );
						var resultWithSuccessCheck = this.GetResultWithSuccessCheck( skusResult, skusResult.DoesSkuExistListResult.ResultData );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
						return resultWithSuccessCheck;
					} ) ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : doesSkuExistResponses.ToJson(), methodParameters : skus.ToJson() ) );
				return doesSkuExistResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public IEnumerable< InventoryItemResponse > GetAllItems( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				IEnumerable< InventoryItemResponse > inventoryItemResponses;
				if( this.UseCache() )
					inventoryItemResponses = this.GetCachedInventory();
				else
					inventoryItemResponses = this.DownloadAllItems();
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : inventoryItemResponses.ToJson() ) );

				return inventoryItemResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		private IEnumerable< InventoryItemResponse > GetCachedInventory( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
			IEnumerable< InventoryItemResponse > inventoryItemResponses;
			lock( this._inventoryCacheLock )
			{
				var cachedInventory = this._cache.Get( this._allItemsCacheKey ) as IEnumerable< InventoryItemResponse >;
				if( cachedInventory != null )
					inventoryItemResponses = cachedInventory;
				else
				{
					var items = this.DownloadAllItems( mark ).ToList();
					this._cache.Set( this._allItemsCacheKey, items, new CacheItemPolicy { SlidingExpiration = this.SlidingCacheExpiration } );
					inventoryItemResponses = items;
				}
			}
			ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : inventoryItemResponses.ToJson() ) );
			return inventoryItemResponses;
		}

		private IEnumerable< InventoryItemResponse > DownloadAllItems( Mark mark = null )
		{
			ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

			var filter = new ItemsFilter
			{
				DetailLevel = { IncludeClassificationInfo = true, IncludePriceInfo = true, IncludeQuantityInfo = true }
			};

			var inventoryItemResponses = this.GetFilteredItems( filter );
			ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : inventoryItemResponses.ToJson() ) );

			return inventoryItemResponses;
		}

		private bool UseCache()
		{
			return this._cache != null;
		}

		/// <summary>
		/// Gets the items by skus.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <returns>Enumerator to process items. <c>null</c> is returned
		/// for non-existing skus.</returns>
		/// <remarks>Items are pulled 1 at a time to handle non-existing skus.
		/// This results in slower performance.</remarks>
		public IEnumerable< InventoryItemResponse > GetItems( IEnumerable< string > skus )
		{
			var checkedSkus = this.DoSkusExist( skus );

			var existingSkus = checkedSkus.Where( s => s.Result ).Select( s => s.Sku );

			return existingSkus.ProcessWithPages( 100, skusPage =>
			{
				var itemsResult = AP.Query.Get( () => this._client.GetInventoryItemList( this._credentials, this.AccountId, skusPage.ToArray() ) );
				return this.GetResultWithSuccessCheck( itemsResult, itemsResult.ResultData );
			} );
		}

		public async Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable< string > skus )
		{
			var checkedSkus = await this.DoSkusExistAsync( skus ).ConfigureAwait( false );

			var existingSkus = checkedSkus.Where( s => s.Result ).Select( s => s.Sku );

			return await existingSkus.ProcessWithPagesAsync< string, InventoryItemResponse >( 100, async skusPage =>
			{
				var itemsResult = await AP.QueryAsync.Get( async () => await this._client.GetInventoryItemListAsync( this._credentials, this.AccountId, skusPage.ToArray() ) ).ConfigureAwait( false );
				return this.GetResultWithSuccessCheck( itemsResult, itemsResult.GetInventoryItemListResult.ResultData );
			} ).ConfigureAwait( false );
		}

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		public IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter, Mark mark = null )
		{
			ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );

			filter.Criteria.PageSize = 100;
			filter.Criteria.PageNumber = 0;

			var filteredItems = new List< InventoryItemResponse >();
			while( true )
			{
				filter.Criteria.PageNumber += 1;
				var itemResponse = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
					var apiResultOfArrayOfInventoryItemResponse = this._client.GetFilteredInventoryItemList
						(
							this._credentials,
							this.AccountId, filter.Criteria, filter.DetailLevel,
							filter.SortField, filter.SortDirection );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfArrayOfInventoryItemResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
					return apiResultOfArrayOfInventoryItemResponse;
				} );

				if( !this.IsRequestSuccessful( itemResponse ) )
				{
					filteredItems.Add( null );
					continue;
				}

				var items = itemResponse.ResultData;

				if( items == null )
				{
					ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : filteredItems.ToJson(), methodParameters : filter.ToJson() ) );
					return filteredItems;
				}

				filteredItems.AddRange( items );

				if( items.Length == 0 || items.Length < filter.Criteria.PageSize )
				{
					ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : filteredItems.ToJson(), methodParameters : filter.ToJson() ) );
					return filteredItems;
				}
			}
		}

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		public async Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter )
		{
			filter.Criteria.PageSize = 100;
			filter.Criteria.PageNumber = 0;

			var items = new List< InventoryItemResponse >();
			while( true )
			{
				filter.Criteria.PageNumber += 1;
				var itemResponse = await AP.QueryAsync.Get( async () => await this._client.GetFilteredInventoryItemListAsync
					( this._credentials,
						this.AccountId, filter.Criteria, filter.DetailLevel,
						filter.SortField, filter.SortDirection ).ConfigureAwait( false ) ).ConfigureAwait( false );

				if( !this.IsRequestSuccessful( itemResponse.GetFilteredInventoryItemListResult ) )
					continue;

				var pageItems = itemResponse.GetFilteredInventoryItemListResult.ResultData;
				if( pageItems == null )
					return items;

				items.AddRange( pageItems );

				if( pageItems.Length == 0 || pageItems.Length < filter.Criteria.PageSize )
					return items;
			}
		}

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="startPage">The first page number to query.</param>
		/// <param name="pageLimit">The max number of pages to query.</param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		public async Task< PagedApiResponse< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, int startPage, int pageLimit )
		{
			filter.Criteria.PageSize = 100;
			filter.Criteria.PageNumber = ( startPage > 0 ) ? startPage - 1 : 1;

			var items = new List< InventoryItemResponse >();
			for( var iteration = 0; iteration < pageLimit; iteration++ )
			{
				filter.Criteria.PageNumber += 1;

				var itemResponse = await AP.QueryAsync.Get( async () => 
					await this._client.GetFilteredInventoryItemListAsync( this._credentials, this.AccountId, filter.Criteria, filter.DetailLevel, filter.SortField, filter.SortDirection )
					.ConfigureAwait( false ) ).ConfigureAwait( false );

				if( !this.IsRequestSuccessful( itemResponse.GetFilteredInventoryItemListResult ) )
					continue;

				var pageItems = itemResponse.GetFilteredInventoryItemListResult.ResultData;
				if( pageItems == null )
					return new PagedApiResponse< InventoryItemResponse >( items, filter.Criteria.PageNumber, true );

				items.AddRange( pageItems );
				if( pageItems.Length == 0 || pageItems.Length < filter.Criteria.PageSize )
					return new PagedApiResponse< InventoryItemResponse >( items, filter.Criteria.PageNumber, true );
			}

			return new PagedApiResponse< InventoryItemResponse >( items, filter.Criteria.PageNumber, false );
		}

		public AttributeInfo[] GetAttributes( string sku )
		{
			var attributeList = AP.Query.Get(
				() =>
					this._client.GetInventoryItemAttributeList( this._credentials, this.AccountId, sku ) );
			return this.GetResultWithSuccessCheck( attributeList, attributeList.ResultData );
		}

		public async Task< AttributeInfo[] > GetAttributesAsync( string sku )
		{
			var attributeList = await AP.QueryAsync.Get( async () =>
				await this._client.GetInventoryItemAttributeListAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false ) ).ConfigureAwait( false );

			if (!this.IsRequestSuccessfulAttribute(attributeList))
				return default(AttributeInfo[]);

			return attributeList.GetInventoryItemAttributeListResult.ResultData;
		}

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <returns>Item quantities.</returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		public QuantityInfoResponse GetItemQuantities( string sku )
		{
			var requestResult = AP.Query.Get( () =>
				this._client.GetInventoryItemQuantityInfo( this._credentials, this.AccountId, sku ) );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
		}

		public async Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku )
		{
			var requestResult = await AP.QueryAsync.Get( async () =>
				await this._client.GetInventoryItemQuantityInfoAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false ) ).ConfigureAwait( false );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.GetInventoryItemQuantityInfoResult.ResultData );
		}

		public ClassificationConfigurationInformation[] GetClassificationConfigurationInformation()
		{
			var requestResult = AP.Query.Get( () => this._client.GetClassificationConfigurationInformation( this._credentials, this.AccountId ) );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
		}

		public async Task< ClassificationConfigurationInformation[] > GetClassificationConfigurationInformationAsync()
		{
			var requestResult = await AP.QueryAsync.Get( async () => await this._client.GetClassificationConfigurationInformationAsync( this._credentials, this.AccountId ).ConfigureAwait( false ) ).ConfigureAwait( false );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.GetClassificationConfigurationInformationResult.ResultData );
		}

		public StoreInfo GetStoreInfo( string sku )
		{
			var requestResult = AP.Query.Get( () => this._client.GetInventoryItemStoreInfo( this._credentials, this.AccountId, sku ) );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
		}

		public async Task< StoreInfo > GetStoreInfoAsync( string sku )
		{
			var requestResult = await AP.QueryAsync.Get( async () => await this._client.GetInventoryItemStoreInfoAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false ) ).ConfigureAwait( false );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.GetInventoryItemStoreInfoResult.ResultData );
		}

		public ImageInfoResponse[] GetImageList( string sku )
		{
			var requestResult = AP.Query.Get( () => this._client.GetInventoryItemImageList( this._credentials, this.AccountId, sku ) );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
		}

		public async Task< ImageInfoResponse[] > GetImageListAsync( string sku )
		{
			var requestResult = await AP.QueryAsync.Get( async () => await this._client.GetInventoryItemImageListAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false ) ).ConfigureAwait( false );

			if (!this.IsRequestSuccessfulImage(requestResult))
				return default(ImageInfoResponse[]);

			return requestResult.GetInventoryItemImageListResult.ResultData;
		}

		public DistributionCenterInfoResponse[] GetShippingInfo( string sku )
		{
			var requestResult = AP.Query.Get( () => this._client.GetInventoryItemShippingInfo( this._credentials, this.AccountId, sku ) );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
		}

		public async Task< DistributionCenterInfoResponse[] > GetShippingInfoAsync( string sku )
		{
			var requestResult = await AP.QueryAsync.Get( async () => await this._client.GetInventoryItemShippingInfoAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false ) ).ConfigureAwait( false );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.GetInventoryItemShippingInfoResult.ResultData );
		}

		public VariationInfo GetVariationInfo( string sku )
		{
			var requestResult = AP.Query.Get( () => this._client.GetInventoryItemVariationInfo( this._credentials, this.AccountId, sku ) );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
		}

		public async Task< VariationInfo > GetVariationInfoAsync( string sku )
		{
			var requestResult = await AP.QueryAsync.Get( async () => await this._client.GetInventoryItemVariationInfoAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false ) ).ConfigureAwait( false );
			return this.GetResultWithSuccessCheck( requestResult, requestResult.GetInventoryItemVariationInfoResult.ResultData );
		}

		/// <summary>
		/// Gets the available quantity.
		/// </summary>
		/// <param name="sku">The sku of the item.</param>
		/// <returns>
		/// The Available quantity for the specified sku.
		/// </returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryQuantity"/>
		public int GetAvailableQuantity( string sku )
		{
			var quantityResult = AP.Query.Get( () =>
			{
				var result = this._client.GetInventoryQuantity( this._credentials, this.AccountId, sku );
				CheckCaSuccess( result );
				return result;
			} );
			return quantityResult.ResultData;
		}

		public async Task< int > GetAvailableQuantityAsync( string sku )
		{
			var quantityResult = await AP.QueryAsync.Get( async () =>
			{
				var result = await this._client.GetInventoryQuantityAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
				CheckCaSuccess( result.GetInventoryQuantityResult );
				return result.GetInventoryQuantityResult;
			} ).ConfigureAwait( false );
			return quantityResult.ResultData;
		}

		public IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus )
		{
			return skus.ProcessWithPages( 100, s =>
			{
				var requestResult = AP.Query.Get( () => this._client.GetInventoryQuantityList( this._credentials, this.AccountId, s.ToArray() ) );

				return this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
			} );
		}

		public async Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus )
		{
			return await skus.ProcessWithPagesAsync< string, InventoryQuantityResponse >( 100, async s =>
			{
				var requestResult = await AP.QueryAsync.Get( async () => await this._client.GetInventoryQuantityListAsync( this._credentials, this.AccountId, s.ToArray() ) ).ConfigureAwait( false );
				return this.GetResultWithSuccessCheck( requestResult.GetInventoryQuantityListResult, requestResult.GetInventoryQuantityListResult.ResultData );
			} ).ConfigureAwait( false );
		}

		#region  Skus
		public IEnumerable< string > GetAllSkus()
		{
			return this.GetFilteredSkus( new ItemsFilter() );
		}

		public async Task< IEnumerable< string > > GetAllSkusAsync()
		{
			return await this.GetFilteredSkusAsync( new ItemsFilter() );
		}

		public IEnumerable< string > GetFilteredSkus( ItemsFilter filter )
		{
			filter.Criteria.PageSize = 100;
			filter.Criteria.PageNumber = 0;

			filter.DetailLevel.IncludeClassificationInfo = true;
			filter.DetailLevel.IncludePriceInfo = true;
			filter.DetailLevel.IncludeQuantityInfo = true;

			while( true )
			{
				filter.Criteria.PageNumber += 1;
				var itemResponse = AP.Query.Get(
					() => this._client.GetFilteredSkuList
						(
							this._credentials, this.AccountId, filter.Criteria,
							filter.SortField, filter.SortDirection ) );

				if( !this.IsRequestSuccessful( itemResponse ) )
				{
					yield return null;
					continue;
				}

				var items = itemResponse.ResultData;

				if( items == null )
					yield break;

				foreach( var item in items )
				{
					yield return item;
				}

				if( items.Length == 0 || items.Length < filter.Criteria.PageSize )
					yield break;
			}
		}

		public async Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter )
		{
			filter.Criteria.PageSize = 100;
			filter.Criteria.PageNumber = 0;

			var skus = new List< string >();
			while( true )
			{
				filter.Criteria.PageNumber += 1;
				var itemResponse = await AP.QueryAsync.Get( async () => await this._client.GetFilteredSkuListAsync
					( this._credentials, this.AccountId, filter.Criteria,
						filter.SortField, filter.SortDirection ).ConfigureAwait( false ) ).ConfigureAwait( false );

				if( !this.IsRequestSuccessful( itemResponse.GetFilteredSkuListResult ) )
					continue;

				var pageSkus = itemResponse.GetFilteredSkuListResult.ResultData;
				if( pageSkus == null )
					return skus;

				skus.AddRange( pageSkus );

				if( pageSkus.Length == 0 || pageSkus.Length < filter.Criteria.PageSize )
					return skus;
			}
		}

		public async Task< PagedApiResponse< string > > GetFilteredSkusAsync( ItemsFilter filter, int startPage, int pageLimit )
		{
			filter.Criteria.PageSize = 100;
			filter.Criteria.PageNumber = ( startPage > 0 ) ? startPage -1 : 1;

			var skus = new List< string >();
			for( var iteration = 0; iteration < pageLimit; iteration++ )
			{
				filter.Criteria.PageNumber += 1;

				var itemResponse = await AP.QueryAsync.Get( async () => 
					await this._client.GetFilteredSkuListAsync( this._credentials, this.AccountId, filter.Criteria, filter.SortField, filter.SortDirection )
					.ConfigureAwait( false ) ).ConfigureAwait( false );

				if( !this.IsRequestSuccessful( itemResponse.GetFilteredSkuListResult ) )
					continue;

				var pageSkus = itemResponse.GetFilteredSkuListResult.ResultData;
				if( pageSkus == null )
					return new PagedApiResponse< string >( skus, filter.Criteria.PageNumber, true );

				skus.AddRange( pageSkus );

				if( pageSkus.Length == 0 || pageSkus.Length < filter.Criteria.PageSize )
					return new PagedApiResponse< string >( skus, filter.Criteria.PageNumber, true );
			}

			return new PagedApiResponse< string >( skus, filter.Criteria.PageNumber, false );
		}
		#endregion

		#endregion

		#region Update items
		public void SynchItem( InventoryItemSubmit item, bool isCreateNew = false )
		{
			AP.Submit.Do( () =>
			{
				if( !isCreateNew && !this.DoesSkuExist( item.Sku ) )
					return;

				var resultOfBoolean = this._client.SynchInventoryItem( this._credentials, this.AccountId, item );
				CheckCaSuccess( resultOfBoolean );
			} );
		}

		public async Task SynchItemAsync( InventoryItemSubmit item, bool isCreateNew = false )
		{
			await AP.SubmitAsync.Do( async () =>
			{
				if( !isCreateNew && !( await this.DoesSkuExistAsync( item.Sku ) ) )
					return;

				var resultOfBoolean = await this._client.SynchInventoryItemAsync( this._credentials, this.AccountId, item ).ConfigureAwait( false );
				CheckCaSuccess( resultOfBoolean.SynchInventoryItemResult );
			} ).ConfigureAwait( false );
		}

		public void SynchItems( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false )
		{
			if( !isCreateNew )
			{
				var existSkus = this.DoSkusExist( items.Select( x => x.Sku ) ).Select( x => x.Sku );
				items = items.Where( x => existSkus.Contains( x.Sku ) );
			}

			items.DoWithPages( 100, i => AP.Submit.Do( () =>
			{
				var resultOfBoolean = this._client.SynchInventoryItemList( this._credentials, this.AccountId, i.ToArray() );
				CheckCaSuccess( resultOfBoolean );
			} ) );
		}

		public async Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false )
		{
			if( !isCreateNew )
			{
				var existSkus = ( this.DoSkusExist( items.Select( x => x.Sku ) ) ).Select( x => x.Sku );
				items = items.Where( x => existSkus.Contains( x.Sku ) );
			}

			await items.DoWithPagesAsync( 100, async i => await AP.SubmitAsync.Do( async () =>
			{
				await AP.SubmitAsync.Do( async () =>
				{
					var resultOfBoolean = await this._client.SynchInventoryItemListAsync( this._credentials, this.AccountId, i.ToArray() ).ConfigureAwait( false );
					CheckCaSuccess( resultOfBoolean.SynchInventoryItemListResult );
				} ).ConfigureAwait( false );
			} ) ).ConfigureAwait( false );
		}

		public void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice )
		{
			AP.Submit.Do( () =>
			{
				var resultOfBoolean = this._client.UpdateInventoryItemQuantityAndPrice( this._credentials, this.AccountId, itemQuantityAndPrice );
				CheckCaSuccess( resultOfBoolean );
			} );
		}

		public async Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice )
		{
			await AP.SubmitAsync.Do( async () =>
			{
				var resultOfBoolean = await this._client.UpdateInventoryItemQuantityAndPriceAsync( this._credentials, this.AccountId, itemQuantityAndPrice ).ConfigureAwait( false );
				CheckCaSuccess( resultOfBoolean.UpdateInventoryItemQuantityAndPriceResult );
			} ).ConfigureAwait( false );
		}

		public void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices )
		{
			itemQuantityAndPrices.DoWithPages( 5000, itemsPage => AP.Submit.Do( () =>
			{
				var resultOfBoolean = this._client.UpdateInventoryItemQuantityAndPriceList( this._credentials, this.AccountId, itemsPage.ToArray() );
				CheckCaSuccess( resultOfBoolean );
			} ) );
		}

		public async Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices )
		{
			await itemQuantityAndPrices.DoWithPagesAsync( 500, async i => await AP.SubmitAsync.Do( async () =>
			{
				var result = await this._client.UpdateInventoryItemQuantityAndPriceListAsync( this._credentials, this.AccountId, i.ToArray() ).ConfigureAwait( false );
				CheckCaSuccess( result.UpdateInventoryItemQuantityAndPriceListResult );
			} ).ConfigureAwait( false ) ).ConfigureAwait( false );
		}

		public void RemoveLabelListFromItemList( string[] labels, IEnumerable< string > skus, string reason )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			skus.DoWithPages( 500, s => AP.Submit.Do( () =>
			{
				var resultOfBoolean = this._client.RemoveLabelListFromInventoryItemList( this._credentials, this.AccountId, labels, s.ToArray(), reason );
				CheckCaSuccess( resultOfBoolean );
			} ) );
		}

		public async Task RemoveLabelListFromItemListAsync( string[] labels, IEnumerable< string > skus, string reason )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			await skus.DoWithPagesAsync( 500, async s => await AP.SubmitAsync.Do( async () =>
			{
				var resultOfBoolean = await this._client.RemoveLabelListFromInventoryItemListAsync( this._credentials, this.AccountId, labels, s.ToArray(), reason ).ConfigureAwait( false );
				CheckCaSuccess( resultOfBoolean.RemoveLabelListFromInventoryItemListResult );
			} ).ConfigureAwait( false ) ).ConfigureAwait( false );
		}

		public void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			skus.DoWithPages( 500, s => AP.Submit.Do( () =>
			{
				var resultOfBoolean = this._client.AssignLabelListToInventoryItemList( this._credentials, this.AccountId, labels, createLabelIfNotExist, s.ToArray(), reason );
				CheckCaSuccess( resultOfBoolean );
			} ) );
		}

		public async Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			await skus.DoWithPagesAsync( 500, async s => await AP.SubmitAsync.Do( async () =>
			{
				var resultOfBoolean = await this._client.AssignLabelListToInventoryItemListAsync( this._credentials, this.AccountId, labels, createLabelIfNotExist, s.ToArray(), reason ).ConfigureAwait( false );
				CheckCaSuccess( resultOfBoolean.AssignLabelListToInventoryItemListResult );
			} ).ConfigureAwait( false ) ).ConfigureAwait( false );
		}
		#endregion

		#region Delete item
		public void DeleteItem( string sku )
		{
			AP.Submit.Do( () =>
			{
				var resultOfBoolean = this._client.DeleteInventoryItem( this._credentials, this.AccountId, sku );
				CheckCaSuccess( resultOfBoolean );
			} );
		}

		public async Task DeleteItemAsync( string sku )
		{
			await AP.SubmitAsync.Do( async () =>
			{
				var resultOfBoolean = await this._client.DeleteInventoryItemAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
				CheckCaSuccess( resultOfBoolean.DeleteInventoryItemResult );
			} ).ConfigureAwait( false );
		}
		#endregion

		#region Get Config Info
		public ClassificationConfigurationInformation[] GetClassificationConfigInfo()
		{
			return AP.Query.Get( () =>
			{
				var result = this._client.GetClassificationConfigurationInformation( this._credentials, this.AccountId );
				CheckCaSuccess( result );
				return result.ResultData;
			} );
		}

		public async Task< ClassificationConfigurationInformation[] > GetClassificationConfigInfoAsync()
		{
			return await AP.QueryAsync.Get( async () =>
			{
				var result = await this._client.GetClassificationConfigurationInformationAsync( this._credentials, this.AccountId ).ConfigureAwait( false );
				CheckCaSuccess( result.GetClassificationConfigurationInformationResult );
				return result.GetClassificationConfigurationInformationResult.ResultData;
			} ).ConfigureAwait( false );
		}

		public DistributionCenterResponse[] GetDistributionCenterList()
		{
			return AP.Query.Get( () =>
			{
				var result = this._client.GetDistributionCenterList( this._credentials, this.AccountId );
				CheckCaSuccess( result );
				return result.ResultData;
			} );
		}

		public async Task< DistributionCenterResponse[] > GetDistributionCenterListAsync()
		{
			return await AP.QueryAsync.Get( async () =>
			{
				var result = await this._client.GetDistributionCenterListAsync( this._credentials, this.AccountId ).ConfigureAwait( false );
				CheckCaSuccess( result.GetDistributionCenterListResult );
				return result.GetDistributionCenterListResult.ResultData;
			} ).ConfigureAwait( false );
		}
		#endregion

		#region Check for Success
		/// <summary>
		/// Gets the result with success check.
		/// </summary>
		/// <typeparam name="T">Type of the result.</typeparam>
		/// <param name="apiResult">The API result.</param>
		/// <param name="resultData">The result data.</param>
		/// <returns>Returns result default value (typically <c>null</c>) if there was a problem
		/// with API call, otherwise returns result.</returns>
		private T GetResultWithSuccessCheck< T >( object apiResult, T resultData )
		{
			if (!this.IsRequestSuccessful(apiResult))
				return default( T );

			return resultData;
		}
		
		private bool IsRequestSuccessful(object obj)
		{
			var type = obj.GetType();

			var statusProp = type.GetProperty("Status");
			var status = (ResultStatus)statusProp.GetValue(obj, null);

			var messageCodeProp = type.GetProperty("MessageCode");
			var messageCode = (int)messageCodeProp.GetValue(obj, null);

			var message = (string)type.GetProperty("Message").GetValue(obj, null);

			return IsRequestSuccessfulCommon(status, message, messageCode);
		}

		private bool IsRequestSuccessfulImage(object obj)
		{
			GetInventoryItemImageListResponse apiResult = (GetInventoryItemImageListResponse)obj;

			var status = apiResult.GetInventoryItemImageListResult.Status;
			var message = apiResult.GetInventoryItemImageListResult.Message;
			var messageCode = apiResult.GetInventoryItemImageListResult.MessageCode;

			return IsRequestSuccessfulCommon(status, message, messageCode);
		}

		private bool IsRequestSuccessfulAttribute(object obj)
		{
			GetInventoryItemAttributeListResponse apiResult = (GetInventoryItemAttributeListResponse)obj;

			var status = apiResult.GetInventoryItemAttributeListResult.Status;
			var message = apiResult.GetInventoryItemAttributeListResult.Message;
			var messageCode = apiResult.GetInventoryItemAttributeListResult.MessageCode;

			return IsRequestSuccessfulCommon(status, message, messageCode);
		}

		/// <summary>
		/// Determines whether request was successful or not.
		/// </summary>
		/// <param name="apiResult">The API result.</param>
		/// <returns>
		/// 	<c>true</c> if request was successful; otherwise, <c>false</c>.
		/// </returns>
		private bool IsRequestSuccessfulCommon(ResultStatus status, string message, int messageCode)
		{
			var isRequestSuccessful = status == ResultStatus.Success && messageCode == 0;

			if (!isRequestSuccessful)
			{
				if (message.Contains("The specified SKU was not found") || message.Contains("All DoesSkuExist requests failed for the SKU list specified!"))
					this.Log().Trace("CA Api Request for '{0}' failed with message: {1}", AccountId, message);
				else
					this.Log().Error("CA Api Request for '{0}' failed with message: {1}", AccountId, message);
			}

			return isRequestSuccessful;
		}

		private static void CheckCaSuccess( APIResultOfBoolean apiResult )
		{
			if( apiResult.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( apiResult.MessageCode, apiResult.Message );
		}

		private static void CheckCaSuccess( APIResultOfArrayOfSynchInventoryItemResponse apiResult )
		{
			if( apiResult.Status != ResultStatus.Success )
			{
				// no sku exists, ignore
				if( apiResult.Message == "All Update requests failed for the SKU list specified!" )
					return;

				var msgs = new StringBuilder();

				foreach( var result in apiResult.ResultData )
				{
					if( !result.Result && !IsSkuMissing( result ) )
						msgs.AppendLine( result.ErrorMessage );
				}

				if( msgs.Length > 0 )
					throw new ChannelAdvisorException( apiResult.MessageCode, string.Concat( apiResult.Message, Environment.NewLine, msgs.ToString() ) );
			}
		}

		private static bool IsSkuMissing( SynchInventoryItemResponse r )
		{
			const string skuMissingMsg = "The specified Sku does not exist";
			return r.ErrorMessage.StartsWith( skuMissingMsg, StringComparison.InvariantCultureIgnoreCase );
		}

		private static void CheckCaSuccess( APIResultOfArrayOfUpdateInventoryItemResponse apiResult )
		{
			if( apiResult.Status != ResultStatus.Success )
			{
				// no sku exists, ignore
				if( apiResult.Message == "All Update requests failed for the SKU list specified!" )
					return;

				if( apiResult.ResultData == null || apiResult.ResultData.Length == 0 )
					throw new ChannelAdvisorException( apiResult.MessageCode, apiResult.Message );

				var msgs = new StringBuilder();

				foreach( var result in apiResult.ResultData )
				{
					if( !result.Result && !IsSkuMissing( result ) )
						msgs.AppendLine( result.ErrorMessage );
				}

				if( msgs.Length > 0 )
					throw new ChannelAdvisorException( apiResult.MessageCode, string.Concat( apiResult.Message, Environment.NewLine, msgs.ToString() ) );
			}
		}

		private static bool IsSkuMissing( UpdateInventoryItemResponse result )
		{
			const string skuMissingMsg = "The specified Sku does not exist";
			return result.ErrorMessage.StartsWith( skuMissingMsg, StringComparison.InvariantCultureIgnoreCase );
		}

		private static void CheckCaSuccess( APIResultOfInt32 result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		private static void CheckCaSuccess( APIResultOfArrayOfClassificationConfigurationInformation result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		private void CheckCaSuccess( APIResultOfString result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		private void CheckCaSuccess( APIResultOfArrayOfDistributionCenterResponse result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}
		#endregion

		private string CreateMethodCallInfo( string methodParameters = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", string notes = "", [ CallerMemberName ] string memberName = "" )
		{
			mark = mark ?? Mark.Blank();
			var connectionInfo = this.ToJson();
			var str = string.Format(
				"{{Mark:\"{3}\", MethodName:{0}, ConnectionInfo:{1}, MethodParameters:{2} {4}{5}{6}{7}}}",
				memberName,
				connectionInfo,
				string.IsNullOrWhiteSpace( methodParameters ) ? PredefinedValues.EmptyJsonObject : methodParameters,
				mark,
				string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors,
				string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult,
				string.IsNullOrWhiteSpace( notes ) ? string.Empty : ",Notes: " + notes,
				string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", " + additionalInfo
				);
			return str;
		}
	}
}