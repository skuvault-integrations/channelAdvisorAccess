using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.REST.Models;
using ChannelAdvisorAccess.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.REST.Models.Configuration;
using ChannelAdvisorAccess.REST.Models.Infrastructure;
using ChannelAdvisorAccess.REST.Shared;
using CuttingEdge.Conditions;
using APICredentials = ChannelAdvisorAccess.OrderService.APICredentials;

namespace ChannelAdvisorAccess.REST.Services.Items
{
	public class ItemsService : RestServiceBaseAbstr, IItemsService
	{
		/// <summary>
		///	Channel advisor page size for products end point
		/// </summary>
		private const int pageSize = 100;

		/// <summary>
		///	Rest items service with standard authorization flow
		/// </summary>
		/// <param name="credentials">Rest application credentials</param>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="accessToken">Tenant access token</param>
		/// <param name="refreshToken">Tenant refresh token</param>
		public ItemsService( RestCredentials credentials, string accountName, string accessToken, string refreshToken ) 
			: base( credentials, accountName, accessToken, refreshToken ) { }

		/// <summary>
		///	Rest items service with soap compatible authorization flow
		/// </summary>
		/// <param name="credentials">Rest application credentials</param>
		/// <param name="soapCredentials">Soap application credentials</param>
		/// <param name="accountId">Tenant account id</param>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="cache"></param>
		public ItemsService( RestCredentials credentials, APICredentials soapCredentials, string accountId, string accountName, ObjectCache cache = null ) 
			: base( credentials, soapCredentials, accountId, accountName, cache ) { }

		/// <summary>
		///	Checks asynchronously if sku exists 
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< bool > DoesSkuExistAsync( string sku, Mark mark = null )
		{
			return await this.GetProductBySku( sku, mark ).ConfigureAwait( false ) != null;
		}

		/// <summary>
		///	Checks if sku exists 
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public bool DoesSkuExist( string sku, Mark mark = null )
		{
			return this.DoesSkuExistAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Checks if skus exist
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable< string > skus, Mark mark = null )
		{
			return this.DoSkusExistAsync( skus, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Checks asynchronously if skus exist
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< DoesSkuExistResponse > > DoSkusExistAsync( IEnumerable< string > skus, Mark mark = null )
		{
			var response = new List< DoesSkuExistResponse >();

			var products = await this.GetItemsAsync( skus, mark ).ConfigureAwait( false );

			foreach( var sku in skus )
			{
				var product = products.FirstOrDefault( pr => pr.Sku.ToLower().Equals( sku.ToLower() ) );

				if (product != null)
					response.Add( new DoesSkuExistResponse() { Result = true, Sku = sku });
				else
					response.Add( new DoesSkuExistResponse() { Result = false, Sku = sku });
			}

			return response;
		}

		/// <summary>
		///	Gets all items from stock
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable< InventoryItemResponse > GetAllItems( Mark mark = null )
		{
			return this.GetAllItemsAsync( mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Get all items asynchronously from stock
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable < InventoryItemResponse > > GetAllItemsAsync( Mark mark = null )
		{
			var products = await this.GetProducts( null, mark ).ConfigureAwait( false );

			return products.Select( product => product.ToInventoryItemResponse() );
		}

		/// <summary>
		///	Gets all skus
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable< string > GetAllSkus( Mark mark = null )
		{
			return this.GetAllSkusAsync( mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets all skus asynchronously
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< string > > GetAllSkusAsync( Mark mark = null )
		{
			var products = await this.GetAllItemsAsync( mark ).ConfigureAwait( false );

			return products.Select( product => product.Sku ).ToArray();
		}

		/// <summary>
		///	Gets available quantity for each sku
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <param name="delatInMs"></param>
		/// <returns></returns>
		public IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus, Mark mark = null, int delatInMs = 5000 )
		{
			return this.GetAvailableQuantitiesAsync( skus, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets available quantity asynchronously for each sku
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus, Mark mark = null )
		{
			var inventoryQuantity = new List< InventoryQuantityResponse >();

			var products = await this.GetItemsAsync( skus, mark ).ConfigureAwait( false );

			foreach( var sku in skus )
			{
				var product = products.FirstOrDefault( pr => pr.Sku.ToLower().Equals( sku.ToLower() ) );

				if ( product == null )
					inventoryQuantity.Add( new InventoryQuantityResponse() { SKU = sku, MessageCode = 113, Message = String.Format( "The specified SKU {0} does not exist", sku ) });
				else
					inventoryQuantity.Add( new InventoryQuantityResponse() { SKU = sku, Quantity = product.Quantity.Available });
			}

			return inventoryQuantity.ToArray();
		}

		/// <summary>
		///	Gets sku available quantity
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public int GetAvailableQuantity( string sku, Mark mark = null )
		{
			return this.GetAvailableQuantityAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku available quantity asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< int > GetAvailableQuantityAsync( string sku, Mark mark = null )
		{
			var quantity = 0;

			var product = await this.GetProductBySku( sku, mark ).ConfigureAwait( false );

			if ( product != null )
				quantity = product.TotalAvailableQuantity;

			return quantity;
		}

		/// <summary>
		///	Gets distribution centers list
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public DistributionCenterResponse[] GetDistributionCenterList( Mark mark = null )
		{
			return this.GetDistributionCenterListAsync( mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets distribution centers list asynchronously
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< DistributionCenterResponse[] > GetDistributionCenterListAsync( Mark mark = null )
		{
			var distributionCenters = new List<DistributionCenterResponse>();

			var response = await this.GetDistributionCentersAsync( mark ).ConfigureAwait( false );

			distributionCenters.AddRange( response.Select( dc => dc.ToDistributionCenterResponse() ) );
			
			return distributionCenters.ToArray();
		}

		/// <summary>
		///	Get distribution centers
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< DistributionCenter[] > GetDistributionCentersAsync( Mark mark = null )
		{
			var distributionCenters = new List< DistributionCenter >();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				var distributionCentersFromRequest = await base.GetResponseAsync< DistributionCenter >( ChannelAdvisorEndPoint.DistributionCentersUrl, mark );

				distributionCenters.AddRange( distributionCentersFromRequest.Where( dc => !dc.IsDeleted ) );

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : distributionCenters.ToJson(), additionalInfo : this.AdditionalLogInfo() ) );
			}
			catch(Exception exception)
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo()), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}

			return distributionCenters.ToArray();
		}

		/// <summary>
		///  Gets SKU quantity
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public QuantityInfoResponse GetItemQuantities( string sku, Mark mark = null )
		{
			return this.GetItemQuantitiesAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Gets SKU quantity asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku, Mark mark = null )
		{
			QuantityInfoResponse qualityInfo = null;

			var product = await this.GetProductBySku( sku, mark ).ConfigureAwait ( false );

			if ( product != null )
				qualityInfo = product.ToQuantityInfoResponse();

			return qualityInfo;
		}

		/// <summary>
		/// Gets items
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable< InventoryItemResponse > GetItems( IEnumerable< string > skus, Mark mark = null )
		{
			return this.GetItemsAsync( skus, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///  Gets items asynchronously
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable< string > skus, Mark mark = null )
		{
			var items = new List< InventoryItemResponse >();

			foreach( var sku in skus )
			{
				if ( !string.IsNullOrEmpty( sku ) )
				{
					var product = await this.GetProductBySku( sku, mark ).ConfigureAwait( false );

					if ( product != null )
						items.Add( product.ToInventoryItemResponse() );
				}
			}

			return items;
		}

		/// <summary>
		///	Updates sku quantity in CA
		/// </summary>
		/// <param name="itemQuantityAndPrice"></param>
		/// <param name="mark"></param>
		public void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark = null )
		{
			this.UpdateQuantityAndPriceAsync( itemQuantityAndPrice, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Updates sku quantity asynchronously in CA
		/// </summary>
		/// <param name="itemQuantityAndPrice"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark = null )
		{
			Condition.Requires( itemQuantityAndPrice ).IsNotNull();
			Condition.Requires( itemQuantityAndPrice.Quantity ).IsNotNull();

			var product = await this.GetProductBySku( itemQuantityAndPrice.Sku, mark ).ConfigureAwait( false );

			if ( product != null )
			{
				var distributionCenters = await this.GetDistributionCentersAsync( mark ).ConfigureAwait( false );
				var distributionCenter = distributionCenters.FirstOrDefault( dc => dc.Code.ToLower().Equals( itemQuantityAndPrice.DistributionCenterCode.ToLower() ) );

				if (distributionCenter != null)
				{
					var request = new UpdateQuantityRequest()
					{
						UpdateType = itemQuantityAndPrice.UpdateType,
						Updates = new UpdateQuantityDC[] { new UpdateQuantityDC() { DistributionCenterID = distributionCenter.ID, Quantity = itemQuantityAndPrice.Quantity.Value } }
					};

					try
					{
						ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

						var url = String.Format( "{0}({1})/UpdateQuantity", ChannelAdvisorEndPoint.ProductsUrl, product.ID );
						await base.PostAsync( url, new { Value = request }, mark ).ConfigureAwait( false );
						
						ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : distributionCenters.ToJson(), additionalInfo : this.AdditionalLogInfo()) );
					}
					catch( Exception exception )
					{
						var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
						ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
						throw channelAdvisorException;
					}
				}
			}
		}

		/// <summary>
		///	Updates skus quantities in CA
		/// </summary>
		/// <param name="itemQuantityAndPrices"></param>
		/// <param name="mark"></param>
		public void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark = null)
		{
			this.UpdateQuantityAndPricesAsync( itemQuantityAndPrices, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Updates skus quantities asynchronously in CA
		/// </summary>
		/// <param name="itemQuantityAndPrices"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark = null )
		{
			foreach( var item in itemQuantityAndPrices )
				await this.UpdateQuantityAndPriceAsync( item, mark ).ConfigureAwait( false );
		}

		/// <summary>
		///	Gets sku store info (deprecated in the REST API)
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public StoreInfo GetStoreInfo( string sku, Mark mark = null )
		{
			return this.GetStoreInfoAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku quantity in the each distribution center
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public DistributionCenterInfoResponse[] GetShippingInfo( string sku, Mark mark = null )
		{
			return this.GetShippingInfoAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku quantity asynchronously in the each distribution center
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< DistributionCenterInfoResponse[] > GetShippingInfoAsync( string sku, Mark mark = null )
		{
			var response = new List< DistributionCenterInfoResponse >();
			var product = await this.GetProductBySku( sku, mark, true );

			if ( product != null )
			{
				var distributionCenters = await this.GetDistributionCentersAsync( mark ).ConfigureAwait( false );

				foreach( var dcQuantity in product.DCQuantities )
				{
					var distributionCenter = distributionCenters.FirstOrDefault( dc => dc.ID == dcQuantity.DistributionCenterID );

					if ( distributionCenter != null )
					{
						response.Add( new DistributionCenterInfoResponse()
									{
										AvailableQuantity = dcQuantity.AvailableQuantity,
										DistributionCenterCode = distributionCenter.Code
									} );
					}
				}
			}

			return response.ToArray();
		}

		/// <summary>
		///	Gets sku store info asynchronously (deprecated in the REST API)
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< StoreInfo > GetStoreInfoAsync( string sku, Mark mark = null )
		{
			StoreInfo storeInfo = null;

			var product = await this.GetProductBySku( sku, mark ).ConfigureAwait( false );

			if ( product != null )
			{
				storeInfo = new StoreInfo()
				{
					 Title = product.StoreTitle,
					 Description = product.StoreDescription,
					 DisplayInStore = product.IsDisplayInStore
				};
			}

			return storeInfo;
		}

		/// <summary>
		///	Gets sku attributes
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public AttributeInfo[] GetAttributes( string sku, Mark mark = null )
		{
			return this.GetAttributesAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku attributes asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< AttributeInfo[] > GetAttributesAsync( string sku, Mark mark = null )
		{
			var attributes = new List< AttributeInfo >();

			var product = await this.GetProductBySku( sku, mark, includeAttributes: true ).ConfigureAwait( false );

			if ( product != null )
			{
				foreach( var attributeValue in product.Attributes )
				{
					attributes.Add( new AttributeInfo()
					{
						Name = attributeValue.Name,
						Value = attributeValue.Value
					});
				}
			}

			return attributes.ToArray();
		}

		/// <summary>
		///	Gets sku variation
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public VariationInfo GetVariationInfo( string sku, Mark mark = null )
		{
			return this.GetVariationInfoAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku variation asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< VariationInfo > GetVariationInfoAsync( string sku, Mark mark = null )
		{
			VariationInfo variationInfo = null;

			var product = await this.GetProductBySku( sku, mark ).ConfigureAwait( false );

			if ( product != null )
			{
				variationInfo = new VariationInfo()
				{
					 IsInRelationship = product.IsInRelationship,
					 IsParent = product.IsParent,
					 RelationshipName = product.RelationshipName
				};
				
				if ( product.ParentProductID != null )
				{
					var parentProduct = await this.GetProductById( product.ParentProductID.Value, mark ).ConfigureAwait( false );

					if ( parentProduct != null )
						variationInfo.ParentSku = parentProduct.Sku;
				}
			}

			return variationInfo;
		}

		/// <summary>
		///	Gets sku images
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public ImageInfoResponse[] GetImageList( string sku, Mark mark = null )
		{
			return this.GetImageListAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku images asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< ImageInfoResponse[] > GetImageListAsync( string sku, Mark mark = null )
		{
			var productImages = new List< ImageInfoResponse >();
			var product = await this.GetProductBySku( sku, mark, includeImages: true ).ConfigureAwait( false );

			if ( product != null && product.Images != null )
			{
				foreach( var image in product.Images )
					productImages.Add ( new ImageInfoResponse()
					{
						Url = image.Url,
						PlacementName = image.PlacementName
					});
			}

			return productImages.ToArray();
		}

		/// <summary>
		///	Gets filtered items
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter, Mark mark = null )
		{
			return this.GetFilteredItemsAsync( filter, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets filtered items asynchronously
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, Mark mark = null )
		{
			var pagedResult = await this.GetFilteredItemsAsync( filter, 1, pageSize, mark ).ConfigureAwait( false );

			return pagedResult.Response;
		}

		/// <summary>
		///	Gets filtered items asynchronously with extra page information
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="startPage"></param>
		/// <param name="pageLimit"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< PagedApiResponse< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark = null )
		{
			var result = new List< InventoryItemResponse >();
			var filterParam = this.GetFilter( filter );

			var products = await this.GetProducts( filterParam, mark, startPage: startPage ).ConfigureAwait( false );

			foreach( var product in products )
				result.Add( product.ToInventoryItemResponse() );

			var finalPage = 0;

			if ( products.Length > 0 )
				finalPage = (int)Math.Ceiling( products.Length / pageSize * 1.0 );

			return new PagedApiResponse< InventoryItemResponse >( result.ToArray(), finalPage, true );
		}

		/// <summary>
		///	Gets filtered skus
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable< string > GetFilteredSkus( ItemsFilter filter, Mark mark = null )
		{
			return this.GetFilteredSkusAsync( filter, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets filtered skus asynchronously
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter, Mark mark = null )
		{
			var pagedResponse = await this.GetFilteredSkusAsync( filter, 1, pageSize, mark ).ConfigureAwait( false );

			return pagedResponse.Response;
		}

		/// <summary>
		///	Gets filtered skus asynchronously with extra page information
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="startPage"></param>
		/// <param name="pageLimit"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< PagedApiResponse< string > > GetFilteredSkusAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark = null )
		{
			var pagedItems = await this.GetFilteredItemsAsync( filter, startPage, pageLimit, mark ).ConfigureAwait( false );
			var items = pagedItems.Response;

			var finalPage = 0;

			if ( items.Any() )
				finalPage = (int)Math.Ceiling( items.Count() / pageSize * 1.0 );

			return new PagedApiResponse< string >( items.Select( item => item.Sku ).ToArray(), finalPage, true );
		}
		
		/// <summary>
		///	Updates product field
		/// </summary>
		/// <param name="item"></param>
		/// <param name="isCreateNew"></param>
		/// <param name="mark"></param>
		public void SynchItem( InventoryItemSubmit item, bool isCreateNew = false, Mark mark = null )
		{
			this.SynchItemAsync( item, isCreateNew, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Updates product fields asynchronously
		/// </summary>
		/// <param name="item"></param>
		/// <param name="isCreateNew"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task SynchItemAsync( InventoryItemSubmit item, bool isCreateNew = false, Mark mark = null )
		{
			var product = await this.GetProductBySku( item.Sku, mark ).ConfigureAwait( false );

			if ( product == null )
				return;

			var request = this.GetUpdateFields( item );

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				var url = String.Format( "{0}({1})", ChannelAdvisorEndPoint.ProductsUrl, product.ID );
				await base.PutAsync( url, request ).ConfigureAwait( false );
						
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : string.Empty, additionalInfo : this.AdditionalLogInfo()) );
			}
			catch(Exception exception)
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo()), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		/// <summary>
		///	Updates products fields
		/// </summary>
		/// <param name="items"></param>
		/// <param name="isCreateNew"></param>
		/// <param name="mark"></param>
		public void SynchItems( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false, Mark mark = null )
		{
			this.SynchItemsAsync( items, isCreateNew, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Updates products fields asynchronously
		/// </summary>
		/// <param name="items"></param>
		/// <param name="isCreateNew"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false, Mark mark = null )
		{
			foreach( var item in items )
				await this.SynchItemAsync( item, isCreateNew, mark ).ConfigureAwait( false );
		}

		/// <summary>
		///	Gets product by sku asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <param name="includeDcQuantities"></param>
		/// <param name="includeAttributes"></param>
		/// <param name="includeImages"></param>
		/// <returns></returns>
		private async Task< Product > GetProductBySku( string sku, Mark mark, bool includeDcQuantities = false, bool includeAttributes = false, bool includeImages = false )
		{
			var filter = String.Format( "$filter=sku eq '{0}'", Uri.EscapeDataString( sku ) );

			var products = await this.GetProducts( filter, mark, 1, includeDcQuantities, includeAttributes, includeImages ).ConfigureAwait( false );

			return products.FirstOrDefault();
		}

		/// <summary>
		///	Gets products
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <param name="startPage"></param>
		/// <param name="includeDcQuantities"></param>
		/// <param name="includeAttributes"></param>
		/// <param name="includeImages"></param>
		/// <returns></returns>
		private async Task< Product[] > GetProducts( string filter, Mark mark, int startPage = 1, bool includeDcQuantities = false, bool includeAttributes = false, bool includeImages = false )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			Product[] products = null;
			var requestParams = new List< string >();

			// filter products by sku
			if ( !string.IsNullOrEmpty( filter ) )
				requestParams.Add( filter );

			// include extra data in server response
			if ( includeDcQuantities
				|| includeAttributes
				|| includeImages )
			{
				var expandParam = "$expand=";

				if ( includeDcQuantities )
					expandParam += "DCQuantities,";
				else if ( includeAttributes )
					expandParam += "Attributes,";
				else if ( includeImages )
					expandParam += "Images";

				requestParams.Add( expandParam );
			}

			// paging
			if ( startPage > 1)
				requestParams.Add( String.Format( "$skip={0}", ( startPage - 1 ) * pageSize ) );

			try
			{
				var url = ChannelAdvisorEndPoint.ProductsUrl + "?" + string.Join( "&", requestParams.ToArray() );

				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				products = ( await base.GetResponseAsync< Product >( url, mark ).ConfigureAwait( false )).ToArray();
					
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, 
					methodResult : products.ToJson() , 
					additionalInfo : this.AdditionalLogInfo() ) );
				
			}
			catch(Exception exception)
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}

			return products.ToArray();
		}
	
		/// <summary>
		///	Gets product by ID
		/// </summary>
		/// <param name="productId"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		private async Task< Product > GetProductById ( int productId, Mark mark )
		{
			Product product = null;

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var url = ChannelAdvisorEndPoint.ProductsUrl + "(" + productId.ToString() + ")";

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				var productResponse = await base.GetResponseAsync< Product >( url, mark, false ).ConfigureAwait( false );
				product = productResponse.FirstOrDefault();

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, 
													  methodResult : productResponse.ToJson(), 
													  additionalInfo : this.AdditionalLogInfo() ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}

			return product;
		}
	
		/// <summary>
		///	Gets filter value for REST end point
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		private string GetFilter( ItemsFilter filter )
		{
			var filterClauses = new List< string >();

			if ( filter.Criteria != null )
			{
				var criteria = filter.Criteria;

				if ( !string.IsNullOrEmpty( criteria.ClassificationName ) )
					filterClauses.Add( String.Format( "Classification eq '{0}'", criteria.ClassificationName ) );

				if ( !string.IsNullOrEmpty( criteria.LabelName ))
					filterClauses.Add( String.Format( "Labels/Any (l: l/Name eq '{0}')", criteria.LabelName ) );

				if ( !string.IsNullOrEmpty( criteria.DateRangeField ))
				{
					string filterFieldName = null;

					if ( criteria.DateRangeField.Equals( TimeStampFields.CreateDate ) )
						filterFieldName = "CreateDateUtc";
					else if ( criteria.DateRangeField.Equals( TimeStampFields.LastUpdateDate ))
						filterFieldName = "UpdateDateUtc";

					if ( filterFieldName != null )
					{
						if ( criteria.DateRangeStartGMT.HasValue )
							filterClauses.Add( String.Format( "{0} ge {1} ", filterFieldName, this.ConvertDate( criteria.DateRangeStartGMT.Value ) ) );

						if ( criteria.DateRangeEndGMT.HasValue )
							filterClauses.Add( String.Format( "{0} le {1}", filterFieldName, this.ConvertDate( criteria.DateRangeEndGMT.Value ) ) );
					}
				}
			}

			return "$filter=" + string.Join(" and ", filterClauses.ToArray() );
		}
	
		/// <summary>
		///	Gets not nullable fields for product update request
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private Dictionary< string, object > GetUpdateFields< T >( T item )
		{
			var fields = new Dictionary< string, object >();

			foreach( var property in typeof( T ).GetProperties() )
			{
				var value = property.GetValue( item );

				if ( value != null 
					&& !value.GetType().IsArray
					&& !property.Name.Equals( "Sku" ) )
				{
					if ( value is PriceInfo )
					{
						var priceProperties = this.GetUpdateFields( (PriceInfo) value );

						foreach( var priceField in priceProperties )
							fields.Add( priceField.Key, priceField.Value );
					}
					else
						fields.Add( property.Name, value );
				}
			}

			return fields;
		}

		public void Ping(Mark mark = null)
		{
		}

		public Task PingAsync(Mark mark = null)
		{
			return Task.Run(() => { });
		}

		public void RemoveLabelListFromItemList(string[] labels, IEnumerable<string> skus, string reason, Mark mark = null)
		{
			throw new NotImplementedException();
		}

		public Task RemoveLabelListFromItemListAsync(string[] labels, IEnumerable<string> skus, string reason, Mark mark = null)
		{
			throw new NotImplementedException();
		}
		
		public void AssignLabelListToItemList(string[] labels, bool createLabelIfNotExist, IEnumerable<string> skus, string reason, Mark mark = null)
		{
			throw new NotImplementedException();
		}

		public Task AssignLabelListToItemListAsync(string[] labels, bool createLabelIfNotExist, IEnumerable<string> skus, string reason, Mark mark = null)
		{
			throw new NotImplementedException();
		}

		public void DeleteItem(string sku, Mark mark = null)
		{
			throw new NotImplementedException();
		}

		public Task DeleteItemAsync(string sku, Mark mark = null)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///	Gets all classifications and their attributes
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public ClassificationConfigurationInformation[] GetClassificationConfigurationInformation( Mark mark = null )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///	Gets all classifications and their attributes asynchronously
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task< ClassificationConfigurationInformation[] > GetClassificationConfigurationInformationAsync( Mark mark = null )
		{
			throw new NotImplementedException();
		}

		public ClassificationConfigurationInformation[] GetClassificationConfigInfo( Mark mark = null )
		{
			throw new NotImplementedException();
		}

		public Task< ClassificationConfigurationInformation[] > GetClassificationConfigInfoAsync( Mark mark = null )
		{
			throw new NotImplementedException();
		}
	}
}
