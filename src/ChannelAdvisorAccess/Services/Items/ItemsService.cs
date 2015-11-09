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

			var inventoryItemResponses = this.GetFilteredItems( filter, mark );
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
		/// <param name="mark">use it to simplify navigation inside log, allows to view call ierarchy</param>
		/// <returns>Enumerator to process items. <c>null</c> is returned
		/// for non-existing skus.</returns>
		/// <remarks>Items are pulled 1 at a time to handle non-existing skus.
		/// This results in slower performance.</remarks>
		public IEnumerable< InventoryItemResponse > GetItems( IEnumerable< string > skus, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				var checkedSkus = this.DoSkusExist( skus, mark );
				var existingSkus = checkedSkus.Where( s => s.Result ).Select( s => s.Sku );

				var message = "{\"ExistingSkus\":\"" + existingSkus.ToJson() + "\"}";
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, notes : message ) );

				var inventoryItemResponses = existingSkus.ProcessWithPages( 100, skusPage =>
				{
					var itemsResult = AP.Query.Get( () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skusPage.ToJson() ) );
						var apiResultOfArrayOfInventoryItemResponse = this._client.GetInventoryItemList( this._credentials, this.AccountId, skusPage.ToArray() );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodResult : apiResultOfArrayOfInventoryItemResponse.ToJson(), methodParameters : skusPage.ToJson() ) );
						return apiResultOfArrayOfInventoryItemResponse;
					} );
					return this.GetResultWithSuccessCheck( itemsResult, itemsResult.ResultData );
				} );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson(), methodResult : inventoryItemResponses.ToJson() ) );
				return inventoryItemResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable< string > skus, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				var checkedSkus = await this.DoSkusExistAsync( skus, mark ).ConfigureAwait( false );
				var existingSkus = checkedSkus.Where( s => s.Result ).Select( s => s.Sku );

				var message = "{\"ExistingSkus\":\"" + existingSkus.ToJson() + "\"}";
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, notes : message ) );

				var inventoryItemResponses = await existingSkus.ProcessWithPagesAsync< string, InventoryItemResponse >( 100, async skusPage =>
				{
					var itemsResult = await AP.QueryAsync.Get( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skusPage.ToJson() ) );
						var getInventoryItemListResponse = await this._client.GetInventoryItemListAsync( this._credentials, this.AccountId, skusPage.ToArray() );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getInventoryItemListResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : skusPage.ToJson() ) );
						return getInventoryItemListResponse;
					} ).ConfigureAwait( false );

					ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson(), methodResult : itemsResult.ToJson() ) );

					return this.GetResultWithSuccessCheck( itemsResult, itemsResult.GetInventoryItemListResult.ResultData );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson(), methodResult : inventoryItemResponses.ToJson() ) );
				return inventoryItemResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark">use it to simplify navigation inside log, allows to view call ierarchy</param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		public IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
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
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark">use it to simplify navigation inside log, allows to view call ierarchy</param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		public async Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
				filter.Criteria.PageSize = 100;
				filter.Criteria.PageNumber = 0;

				var items = new List< InventoryItemResponse >();
				while( true )
				{
					filter.Criteria.PageNumber += 1;
					var itemResponse = await AP.QueryAsync.Get( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						var getFilteredInventoryItemListResponse = await this._client.GetFilteredInventoryItemListAsync
							( this._credentials,
								this.AccountId, filter.Criteria, filter.DetailLevel,
								filter.SortField, filter.SortDirection ).ConfigureAwait( false );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getFilteredInventoryItemListResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return getFilteredInventoryItemListResponse;
					}
						).ConfigureAwait( false );

					if( !this.IsRequestSuccessful( itemResponse.GetFilteredInventoryItemListResult ) )
						continue;

					var pageItems = itemResponse.GetFilteredInventoryItemListResult.ResultData;
					if( pageItems == null )
					{
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : items.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return items;
					}

					items.AddRange( pageItems );

					if( pageItems.Length == 0 || pageItems.Length < filter.Criteria.PageSize )
					{
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : items.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return items;
					}
				}
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="startPage">The first page number to query.</param>
		/// <param name="pageLimit">The max number of pages to query.</param>
		/// <param name="mark">use it to simplify navigation inside log, allows to view call ierarchy</param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		public async Task< PagedApiResponse< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
				filter.Criteria.PageSize = 100;
				filter.Criteria.PageNumber = ( startPage > 0 ) ? startPage - 1 : 1;

				var items = new List< InventoryItemResponse >();
				for( var iteration = 0; iteration < pageLimit; iteration++ )
				{
					filter.Criteria.PageNumber += 1;

					var itemResponse = await AP.QueryAsync.Get( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						var getFilteredInventoryItemListResponse = await this._client.GetFilteredInventoryItemListAsync( this._credentials, this.AccountId, filter.Criteria, filter.DetailLevel, filter.SortField, filter.SortDirection )
							.ConfigureAwait( false );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getFilteredInventoryItemListResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return getFilteredInventoryItemListResponse;
					}
						).ConfigureAwait( false );

					if( !this.IsRequestSuccessful( itemResponse.GetFilteredInventoryItemListResult ) )
						continue;

					var pageItems = itemResponse.GetFilteredInventoryItemListResult.ResultData;
					if( pageItems == null )
					{
						var pagedApiResponse = new PagedApiResponse< InventoryItemResponse >( items, filter.Criteria.PageNumber, true );
						ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, notes : "PageResponse", methodResult : pagedApiResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return pagedApiResponse;
					}

					items.AddRange( pageItems );
					if( pageItems.Length == 0 || pageItems.Length < filter.Criteria.PageSize )
					{
						var pagedApiResponse = new PagedApiResponse< InventoryItemResponse >( items, filter.Criteria.PageNumber, true );
						ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, notes : "PageResponse", methodResult : pagedApiResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return pagedApiResponse;
					}
				}

				var apiResponse = new PagedApiResponse< InventoryItemResponse >( items, filter.Criteria.PageNumber, false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
				return apiResponse;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public AttributeInfo[] GetAttributes( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var attributeList = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var apiResultOfArrayOfAttributeInfo = this._client.GetInventoryItemAttributeList( this._credentials, this.AccountId, sku );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfArrayOfAttributeInfo.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return apiResultOfArrayOfAttributeInfo;
				}
					);
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : attributeList.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( attributeList, attributeList.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< AttributeInfo[] > GetAttributesAsync( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var attributeList = await AP.QueryAsync.Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var getInventoryItemAttributeListResponse = await this._client.GetInventoryItemAttributeListAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getInventoryItemAttributeListResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return getInventoryItemAttributeListResponse;
				}
					).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : attributeList.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var attributeInfos = !this.IsRequestSuccessfulAttribute( attributeList ) ? default( AttributeInfo[] ) : attributeList.GetInventoryItemAttributeListResult.ResultData;
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : attributeInfos.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return attributeInfos;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <param name="mark"></param>
		/// <returns>Item quantities.</returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		public QuantityInfoResponse GetItemQuantities( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var apiResultOfQuantityInfoResponse = this._client.GetInventoryItemQuantityInfo( this._credentials, this.AccountId, sku );
					ChannelAdvisorLogger.LogTraceRetryEnd(this.CreateMethodCallInfo(mark: mark, methodResult: apiResultOfQuantityInfoResponse.ToJson(), additionalInfo: this.AdditionalLogInfoString, methodParameters: sku));

					return apiResultOfQuantityInfoResponse;
				}
					);
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );

				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = await AP.QueryAsync.Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var getInventoryItemQuantityInfoResponse = await this._client.GetInventoryItemQuantityInfoAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getInventoryItemQuantityInfoResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );

					return getInventoryItemQuantityInfoResponse;
				}
					).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.GetInventoryItemQuantityInfoResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );

				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public ClassificationConfigurationInformation[] GetClassificationConfigurationInformation( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				var requestResult = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var apiResultOfArrayOfClassificationConfigurationInformation = this._client.GetClassificationConfigurationInformation( this._credentials, this.AccountId );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfArrayOfClassificationConfigurationInformation.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );

					return apiResultOfArrayOfClassificationConfigurationInformation;
				} );
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );

				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< ClassificationConfigurationInformation[] > GetClassificationConfigurationInformationAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				var requestResult = await AP.QueryAsync.Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
					var getClassificationConfigurationInformationResponse = await this._client.GetClassificationConfigurationInformationAsync( this._credentials, this.AccountId ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getClassificationConfigurationInformationResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					return getClassificationConfigurationInformationResponse;
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.GetClassificationConfigurationInformationResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );

				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public StoreInfo GetStoreInfo( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );

				var requestResult = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var apiResultOfStoreInfo = this._client.GetInventoryItemStoreInfo( this._credentials, this.AccountId, sku );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfStoreInfo.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return apiResultOfStoreInfo;
				} );
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< StoreInfo > GetStoreInfoAsync( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = await AP.QueryAsync.Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var getInventoryItemStoreInfoResponse = await this._client.GetInventoryItemStoreInfoAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getInventoryItemStoreInfoResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return getInventoryItemStoreInfoResponse;
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.GetInventoryItemStoreInfoResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public ImageInfoResponse[] GetImageList( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var apiResultOfArrayOfImageInfoResponse = this._client.GetInventoryItemImageList( this._credentials, this.AccountId, sku );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfArrayOfImageInfoResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return apiResultOfArrayOfImageInfoResponse;
				} );
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< ImageInfoResponse[] > GetImageListAsync( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = await AP.QueryAsync.Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var getInventoryItemImageListResponse = await this._client.GetInventoryItemImageListAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getInventoryItemImageListResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return getInventoryItemImageListResponse;
				} ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var imageInfoResponses = !this.IsRequestSuccessfulImage( requestResult ) ? default( ImageInfoResponse[] ) : requestResult.GetInventoryItemImageListResult.ResultData;
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : imageInfoResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return imageInfoResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public DistributionCenterInfoResponse[] GetShippingInfo( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var apiResultOfArrayOfDistributionCenterInfoResponse = this._client.GetInventoryItemShippingInfo( this._credentials, this.AccountId, sku );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfArrayOfDistributionCenterInfoResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return apiResultOfArrayOfDistributionCenterInfoResponse;
				} );
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var distributionCenterInfoResponses = this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : distributionCenterInfoResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return distributionCenterInfoResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< DistributionCenterInfoResponse[] > GetShippingInfoAsync( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = await AP.QueryAsync.Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var getInventoryItemShippingInfoResponse = await this._client.GetInventoryItemShippingInfoAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getInventoryItemShippingInfoResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return getInventoryItemShippingInfoResponse;
				} ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var distributionCenterInfoResponses = this.GetResultWithSuccessCheck( requestResult, requestResult.GetInventoryItemShippingInfoResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : distributionCenterInfoResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return distributionCenterInfoResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public VariationInfo GetVariationInfo( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var apiResultOfVariationInfo = this._client.GetInventoryItemVariationInfo( this._credentials, this.AccountId, sku );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfVariationInfo.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );

					return apiResultOfVariationInfo;
				} );
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< VariationInfo > GetVariationInfoAsync( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var requestResult = await AP.QueryAsync.Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var getInventoryItemVariationInfoResponse = await this._client.GetInventoryItemVariationInfoAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getInventoryItemVariationInfoResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return getInventoryItemVariationInfoResponse;
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.GetInventoryItemVariationInfoResult.ResultData );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return resultWithSuccessCheck;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		/// <summary>
		/// Gets the available quantity.
		/// </summary>
		/// <param name="sku">The sku of the item.</param>
		/// <param name="mark"></param>
		/// <returns>
		/// The Available quantity for the specified sku.
		/// </returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryQuantity"/>
		public int GetAvailableQuantity( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var quantityResult = AP.Query.Get( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var result = this._client.GetInventoryQuantity( this._credentials, this.AccountId, sku );
					CheckCaSuccess( result );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : result.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return result;
				} );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : quantityResult.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return quantityResult.ResultData;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< int > GetAvailableQuantityAsync( string sku, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				var quantityResult = await AP.QueryAsync.Get( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					var result = await this._client.GetInventoryQuantityAsync( this._credentials, this.AccountId, sku ).ConfigureAwait( false );
					CheckCaSuccess( result.GetInventoryQuantityResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : result.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
					return result.GetInventoryQuantityResult;
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : quantityResult.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : sku ) );
				return quantityResult.ResultData;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
				var inventoryQuantityResponses = skus.ProcessWithPages( 100, s =>
				{
					var requestResult = AP.Query.Get( () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
						var apiResultOfArrayOfInventoryQuantityResponse = this._client.GetInventoryQuantityList( this._credentials, this.AccountId, s.ToArray() );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfArrayOfInventoryQuantityResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
						return apiResultOfArrayOfInventoryQuantityResponse;
					} );

					var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult, requestResult.ResultData );
					ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodParameters : s.ToJson(), methodResult : resultWithSuccessCheck.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					return resultWithSuccessCheck;
				} );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : inventoryQuantityResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
				return inventoryQuantityResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
				var inventoryQuantityResponses = await skus.ProcessWithPagesAsync< string, InventoryQuantityResponse >( 100, async s =>
				{
					var requestResult = await AP.QueryAsync.Get( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
						var getInventoryQuantityListResponse = await this._client.GetInventoryQuantityListAsync( this._credentials, this.AccountId, s.ToArray() );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getInventoryQuantityListResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
						return getInventoryQuantityListResponse;
					} ).ConfigureAwait( false );
					var resultWithSuccessCheck = this.GetResultWithSuccessCheck( requestResult.GetInventoryQuantityListResult, requestResult.GetInventoryQuantityListResult.ResultData );
					ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, notes : "ProcessByPage", methodParameters : s.ToJson(), methodResult : requestResult.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
					return resultWithSuccessCheck;
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : inventoryQuantityResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : skus.ToJson() ) );
				return inventoryQuantityResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}
		#endregion

		#region  Skus
		public IEnumerable< string > GetAllSkus( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				var filteredSkus = this.GetFilteredSkus( new ItemsFilter(), mark );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : filteredSkus.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				return filteredSkus;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< IEnumerable< string > > GetAllSkusAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
				var filteredSkus = await this.GetFilteredSkusAsync( new ItemsFilter(), mark );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : filteredSkus.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );
				return filteredSkus;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public IEnumerable< string > GetFilteredSkus( ItemsFilter filter, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );

				filter.Criteria.PageSize = 100;
				filter.Criteria.PageNumber = 0;

				filter.DetailLevel.IncludeClassificationInfo = true;
				filter.DetailLevel.IncludePriceInfo = true;
				filter.DetailLevel.IncludeQuantityInfo = true;

				var filteredSkus = new List< string >();
				while( true )
				{
					filter.Criteria.PageNumber += 1;
					var itemResponse = AP.Query.Get(
						() =>
						{
							ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
							var apiResultOfArrayOfString = this._client.GetFilteredSkuList
								(
									this._credentials, this.AccountId, filter.Criteria,
									filter.SortField, filter.SortDirection );
							ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResultOfArrayOfString.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );

							return apiResultOfArrayOfString;
						} );
					ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : itemResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );

					if( !this.IsRequestSuccessful( itemResponse ) )
					{
						filteredSkus.Add( null );
						continue;
					}

					var items = itemResponse.ResultData;

					if( items == null )
					{
						ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : filteredSkus.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return filteredSkus;
					}

					filteredSkus.AddRange( items );

					if( items.Length == 0 || items.Length < filter.Criteria.PageSize )
					{
						ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : filteredSkus.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return filteredSkus;
					}
				}
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
				filter.Criteria.PageSize = 100;
				filter.Criteria.PageNumber = 0;

				var skus = new List< string >();
				while( true )
				{
					filter.Criteria.PageNumber += 1;
					var itemResponse = await AP.QueryAsync.Get( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						var getFilteredSkuListResponse = await this._client.GetFilteredSkuListAsync
							( this._credentials, this.AccountId, filter.Criteria,
								filter.SortField, filter.SortDirection ).ConfigureAwait( false );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getFilteredSkuListResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return getFilteredSkuListResponse;
					} ).ConfigureAwait( false );

					ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : itemResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );

					if( !this.IsRequestSuccessful( itemResponse.GetFilteredSkuListResult ) )
						continue;

					var pageSkus = itemResponse.GetFilteredSkuListResult.ResultData;
					if( pageSkus == null )
					{
						ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : skus.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return skus;
					}

					skus.AddRange( pageSkus );

					if( pageSkus.Length == 0 || pageSkus.Length < filter.Criteria.PageSize )
					{
						ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : skus.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : filter.ToJson() ) );
						return skus;
					}
				}
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< PagedApiResponse< string > > GetFilteredSkusAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { filter, startPage, pageLimit };

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				filter.Criteria.PageSize = 100;
				filter.Criteria.PageNumber = ( startPage > 0 ) ? startPage - 1 : 1;

				var skus = new List< string >();
				for( var iteration = 0; iteration < pageLimit; iteration++ )
				{
					filter.Criteria.PageNumber += 1;

					var itemResponse = await AP.QueryAsync.Get( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
						var getFilteredSkuListResponse = await this._client.GetFilteredSkuListAsync( this._credentials, this.AccountId, filter.Criteria, filter.SortField, filter.SortDirection )
							.ConfigureAwait( false );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : getFilteredSkuListResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
						return getFilteredSkuListResponse;
					} ).ConfigureAwait( false );

					ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : itemResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

					if( !this.IsRequestSuccessful( itemResponse.GetFilteredSkuListResult ) )
						continue;

					var pageSkus = itemResponse.GetFilteredSkuListResult.ResultData;
					if( pageSkus == null )
					{
						var pagedApiResponse = new PagedApiResponse< string >( skus, filter.Criteria.PageNumber, true );
						ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : pagedApiResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
						return pagedApiResponse;
					}

					skus.AddRange( pageSkus );

					if( pageSkus.Length == 0 || pageSkus.Length < filter.Criteria.PageSize )
					{
						var pagedApiResponse = new PagedApiResponse< string >( skus, filter.Criteria.PageNumber, true );
						ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : pagedApiResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
						return pagedApiResponse;
					}
				}

				var apiResponse = new PagedApiResponse< string >( skus, filter.Criteria.PageNumber, false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : apiResponse.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
				return apiResponse;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		#endregion

		#region Update items
		public void SynchItem( InventoryItemSubmit item, bool isCreateNew = false, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var parameters = new { item, isCreateNew };

			try
			{
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				AP.Submit.Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
					if( !isCreateNew && !this.DoesSkuExist( item.Sku, mark ) )
						return;

					var resultOfBoolean = this._client.SynchInventoryItem( this._credentials, this.AccountId, item );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
				} );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				await AP.SubmitAsync.Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
					if( !isCreateNew && !( await this.DoesSkuExistAsync( item.Sku, mark ) ) )
						return;

					var resultOfBoolean = await this._client.SynchInventoryItemAsync( this._credentials, this.AccountId, item ).ConfigureAwait( false );
					CheckCaSuccess( resultOfBoolean.SynchInventoryItemResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				if( !isCreateNew )
				{
					var existSkus = this.DoSkusExist( items.Select( x => x.Sku ), mark ).Select( x => x.Sku );
					items = items.Where( x => existSkus.Contains( x.Sku ) );
				}

				items.DoWithPages( 100, i => AP.Submit.Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
					var resultOfBoolean = this._client.SynchInventoryItemList( this._credentials, this.AccountId, i.ToArray() );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
				} ) );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				if( !isCreateNew )
				{
					var existSkus = ( this.DoSkusExist( items.Select( x => x.Sku ), mark ) ).Select( x => x.Sku );
					items = items.Where( x => existSkus.Contains( x.Sku ) );
				}

				await items.DoWithPagesAsync( 100, async i => await AP.SubmitAsync.Do( async () =>
				{
					await AP.SubmitAsync.Do( async () =>
					{
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
						var resultOfBoolean = await this._client.SynchInventoryItemListAsync( this._credentials, this.AccountId, i.ToArray() ).ConfigureAwait( false );
						CheckCaSuccess( resultOfBoolean.SynchInventoryItemListResult );
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
					} ).ConfigureAwait( false );
				} ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );

				AP.Submit.Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );
					var resultOfBoolean = this._client.UpdateInventoryItemQuantityAndPrice( this._credentials, this.AccountId, itemQuantityAndPrice );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );
				} );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );

				await AP.SubmitAsync.Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );
					var resultOfBoolean = await this._client.UpdateInventoryItemQuantityAndPriceAsync( this._credentials, this.AccountId, itemQuantityAndPrice ).ConfigureAwait( false );
					CheckCaSuccess( resultOfBoolean.UpdateInventoryItemQuantityAndPriceResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );
				} ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrice.ToJson() ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
				itemQuantityAndPrices.DoWithPages( 5000, itemsPage => AP.Submit.Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
					var resultOfBoolean = this._client.UpdateInventoryItemQuantityAndPriceList( this._credentials, this.AccountId, itemsPage.ToArray() );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
				} ) );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );

				await itemQuantityAndPrices.DoWithPagesAsync( 500, async i => await AP.SubmitAsync.Do( async () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
					var result = await this._client.UpdateInventoryItemQuantityAndPriceListAsync( this._credentials, this.AccountId, i.ToArray() ).ConfigureAwait( false );
					CheckCaSuccess( result.UpdateInventoryItemQuantityAndPriceListResult );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : result.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
				} ).ConfigureAwait( false ) ).ConfigureAwait( false );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : "void", additionalInfo : this.AdditionalLogInfoString, methodParameters : itemQuantityAndPrices.ToJson() ) );
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
				ChannelAdvisorLogger.LogTraceStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );

				skus.DoWithPages( 500, s => AP.Submit.Do( () =>
				{
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
					var resultOfBoolean = this._client.RemoveLabelListFromInventoryItemList( this._credentials, this.AccountId, labels, s.ToArray(), reason );
					CheckCaSuccess( resultOfBoolean );
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, methodResult : resultOfBoolean.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
				} ) );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : parameters.ToJson() ) );
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

			await skus.DoWithPagesAsync( 500, async s => await AP.SubmitAsync.Do( async () =>
			{
				var resultOfBoolean = await this._client.RemoveLabelListFromInventoryItemListAsync( this._credentials, this.AccountId, labels, s.ToArray(), reason ).ConfigureAwait( false );
				CheckCaSuccess( resultOfBoolean.RemoveLabelListFromInventoryItemListResult );
			} ).ConfigureAwait( false ) ).ConfigureAwait( false );
		}

		public void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark = null )
		{
			Condition.Requires( labels, "labels" ).IsShorterOrEqual( 3, "Only up to 3 labels allowed." ).IsNotNull();

			skus.DoWithPages( 500, s => AP.Submit.Do( () =>
			{
				var resultOfBoolean = this._client.AssignLabelListToInventoryItemList( this._credentials, this.AccountId, labels, createLabelIfNotExist, s.ToArray(), reason );
				CheckCaSuccess( resultOfBoolean );
			} ) );
		}

		public async Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark = null )
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