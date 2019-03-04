using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.REST.Misc;
using ChannelAdvisorAccess.REST.Models;
using ChannelAdvisorAccess.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Services
{
	public class ItemsService : RestServiceBaseAbstr, IItemsService
	{
		protected readonly string distributionCentersUrl = "v1/DistributionCenters";
		protected readonly string productsUrl = "v1/Products";

		/// <summary>
		///	Channel advisor page size for products end point
		/// </summary>
		private const int pageSize = 100;

		/// <summary>
		///	User-friendly channel advisor account name
		/// </summary>
		public string Name => this.AccountName;

		/// <summary>
		///	REST authorization
		/// </summary>
		/// <param name="apiUrl"></param>
		/// <param name="accountName"></param>
		/// <param name="application"></param>
		/// <param name="accessToken"></param>
		/// <param name="refreshToken"></param>
		public ItemsService( RestApplication application, string accountName, string accessToken, string refreshToken ) 
			: base( accountName, application, accessToken, refreshToken ) 
		{ }

		/// <summary>
		///	SOAP compatible authorization flow for REST service
		/// </summary>
		/// <param name="apiUrl"></param>
		/// <param name="soapCredentials"></param>
		/// <param name="accountName"></param>
		/// <param name="accountID"></param>
		public ItemsService( RestApplication application, string accountName, string accountID, string developerKey, string developerPassword ) 
			: base( application, accountName, accountID, developerKey, developerPassword, null )
		{ }

		/// <summary>
		///	Checks asynchronously if sku exists 
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< bool > DoesSkuExistAsync( string sku, Mark mark = null )
		{
			return await GetProductBySku( sku, mark ).ConfigureAwait( false ) != null;
		}

		/// <summary>
		///	Checks if sku exists 
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public bool DoesSkuExist( string sku, Mark mark = null )
		{
			return DoesSkuExistAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Checks if skus exist
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable<string> skus, Mark mark = null )
		{
			return DoSkusExistAsync( skus, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Checks asynchronously if skus exist
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< DoesSkuExistResponse > > DoSkusExistAsync( IEnumerable<string> skus, Mark mark = null )
		{
			List< DoesSkuExistResponse > response = new List< DoesSkuExistResponse >();

			var products = await GetItemsAsync( skus, mark ).ConfigureAwait( false );

			foreach( string sku in skus )
			{
				var product = products.Where( pr => pr.Sku.ToLower().Equals( sku.ToLower() )).FirstOrDefault();

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
			return GetAllItemsAsync( mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Get all items asynchronously from stock
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable < InventoryItemResponse > > GetAllItemsAsync( Mark mark = null )
		{
			var products = await GetProducts( null, mark ).ConfigureAwait( false );

			return products.Select( product => product.ToInventoryItemResponse() );
		}

		/// <summary>
		///	Gets all skus
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable< string > GetAllSkus( Mark mark = null )
		{
			return GetAllSkusAsync( mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets all skus asynchronously
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< string > > GetAllSkusAsync(Mark mark = null)
		{
			var products = await GetAllItemsAsync( mark ).ConfigureAwait( false );

			return products.Select( product => product.Sku ).ToArray();
		}

		/// <summary>
		///	Gets available quantity for each sku
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <param name="delatInMs"></param>
		/// <returns></returns>
		public IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable<string> skus, Mark mark = null, int delatInMs = 5000 )
		{
			return GetAvailableQuantitiesAsync( skus, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets available quantity asynchronously for each sku
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable<string> skus, Mark mark = null )
		{
			List< InventoryQuantityResponse > inventoryQuantity = new List< InventoryQuantityResponse >();

			var products = await GetItemsAsync( skus, mark ).ConfigureAwait( false );

			foreach( string sku in skus )
			{
				var product = products.Where( pr => pr.Sku.ToLower().Equals( sku.ToLower() )).FirstOrDefault();

				if ( product == null )
					inventoryQuantity.Add( new InventoryQuantityResponse() { SKU = sku, MessageCode = 113, Message = $"The specified SKU { sku } does not exist" });
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
			return GetAvailableQuantityAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku available quantity asynchrously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< int > GetAvailableQuantityAsync( string sku, Mark mark = null )
		{
			int quantity = 0;

			var product = await GetProductBySku( sku, mark ).ConfigureAwait( false );

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
			return GetDistributionCenterListAsync( mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets distribution centers list asynchronously
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< DistributionCenterResponse[] > GetDistributionCenterListAsync( Mark mark = null )
		{
			var distributionCenters = new List<DistributionCenterResponse>();

			var response = await GetDistributionCentersAsync( mark ).ConfigureAwait( false );

			distributionCenters.AddRange( response.Select( dc => dc.ToDistributionCenterResponse() ) );
			
			return distributionCenters.ToArray();
		}

		/// <summary>
		///	Get distribution centers as is with identificators
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

				var distributionCentersFromRequest = await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					var response = await GetResponseAsync( distributionCentersUrl ).ConfigureAwait( false );

					return await response.Content.ReadAsAsync< ODataResponse< DistributionCenter > >();
				}).ConfigureAwait( false );

				distributionCenters.AddRange( distributionCentersFromRequest.Value.Where( dc => !dc.IsDeleted ) );

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
			return GetItemQuantitiesAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Gets SKU quantity asynchoronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku, Mark mark = null )
		{
			QuantityInfoResponse qualityInfo = null;

			var product = await GetProductBySku( sku, mark ).ConfigureAwait ( false );

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
		public IEnumerable< InventoryItemResponse > GetItems( IEnumerable<string> skus, Mark mark = null )
		{
			return GetItemsAsync( skus, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///  Gets items asynchoronously
		/// </summary>
		/// <param name="skus"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable<string> skus, Mark mark = null )
		{
			List< InventoryItemResponse > items = new List< InventoryItemResponse >();

			foreach( string sku in skus )
			{
				if ( !string.IsNullOrEmpty( sku ) )
				{
					var product = await GetProductBySku( sku, mark ).ConfigureAwait( false );

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
			UpdateQuantityAndPriceAsync( itemQuantityAndPrice, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Updates sku quantity asynchronously in CA
		/// </summary>
		/// <param name="itemQuantityAndPrice"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark = null )
		{
			var product = await GetProductBySku( itemQuantityAndPrice.Sku, mark ).ConfigureAwait( false );

			if (product != null)
			{
				var distributionCenters = await GetDistributionCentersAsync( mark ).ConfigureAwait( false );
				var distributionCenter = distributionCenters.Where( dc => dc.Code.ToLower().Equals( itemQuantityAndPrice.DistributionCenterCode.ToLower() )).FirstOrDefault();

				if (distributionCenter != null)
				{
					UpdateQuantityRequest request = new UpdateQuantityRequest()
					{
						UpdateType = itemQuantityAndPrice.UpdateType,
						Updates = new UpdateQuantityDC[] { new UpdateQuantityDC() { DistributionCenterID = distributionCenter.ID, Quantity = itemQuantityAndPrice.Quantity.Value } }
					};

					try
					{
						ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

						await AP.CreateQueryAsync(ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
						{
							string url = $"{ productsUrl }({ product.ID })/UpdateQuantity";
							var responseStatusCode = await PostAsync( url, new { Value = request } ).ConfigureAwait( false );

							if (responseStatusCode != System.Net.HttpStatusCode.NoContent)
								throw new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), new Exception("Update sku quantity failed!") );
						}).ConfigureAwait( false );
						
						ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : distributionCenters.ToJson(), additionalInfo : this.AdditionalLogInfo()) );
					}
					catch(Exception exception)
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
			UpdateQuantityAndPricesAsync( itemQuantityAndPrices, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Updates skus quantities asynchronously in CA
		/// </summary>
		/// <param name="itemQuantityAndPrices"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark = null )
		{
			foreach(var item in itemQuantityAndPrices)
				await UpdateQuantityAndPriceAsync( item, mark ).ConfigureAwait( false );
		}

		/// <summary>
		///	Gets sku store info (deprecated in the REST API)
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public StoreInfo GetStoreInfo( string sku, Mark mark = null )
		{
			return GetStoreInfoAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku quantity in the each distribution center
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public DistributionCenterInfoResponse[] GetShippingInfo(string sku, Mark mark = null)
		{
			return GetShippingInfoAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku quantity asynchronously in the each distribution center
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task<DistributionCenterInfoResponse[]> GetShippingInfoAsync(string sku, Mark mark = null)
		{
			List< DistributionCenterInfoResponse > response = new List< DistributionCenterInfoResponse >();
			var product = await GetProductBySku( sku, mark, true );

			if ( product != null )
			{
				var distributionCenters = await GetDistributionCentersAsync( mark ).ConfigureAwait( false );

				foreach( var dcQuantity in product.DCQuantities )
				{
					var distributionCenter = distributionCenters.Where( dc => dc.ID == dcQuantity.DistributionCenterID ).FirstOrDefault();

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
		public async Task<StoreInfo> GetStoreInfoAsync(string sku, Mark mark = null)
		{
			StoreInfo storeInfo = null;

			var product = await GetProductBySku( sku, mark ).ConfigureAwait( false );

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
		public AttributeInfo[] GetAttributes(string sku, Mark mark = null)
		{
			return GetAttributesAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku attributes asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< AttributeInfo[] > GetAttributesAsync( string sku, Mark mark = null )
		{
			List< AttributeInfo > attributes = new List< AttributeInfo >();

			var product = await GetProductBySku( sku, mark, includeAttributes: true ).ConfigureAwait( false );

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
		///	Gets all classifications and their attributes
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public ClassificationConfigurationInformation[] GetClassificationConfigurationInformation( Mark mark = null )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///	Gets all classifications and their attributes asynchonously
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

		/// <summary>
		///	Gets sku variation
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public VariationInfo GetVariationInfo( string sku, Mark mark = null )
		{
			return GetVariationInfoAsync( sku, mark ).GetAwaiter().GetResult();
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

			var product = await GetProductBySku( sku, mark ).ConfigureAwait( false );

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
					var parentProduct = await GetProductByID( product.ParentProductID.Value, mark ).ConfigureAwait( false );

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
		public ImageInfoResponse[] GetImageList(string sku, Mark mark = null)
		{
			return GetImageListAsync( sku, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets sku images asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task<ImageInfoResponse[]> GetImageListAsync(string sku, Mark mark = null)
		{
			List< ImageInfoResponse > productImages = new List< ImageInfoResponse >();

			var product = await GetProductBySku( sku, mark, includeImages: true ).ConfigureAwait( false );

			if ( product != null )
			{
				if ( product.Images != null )
				{
					foreach( var image in product.Images )
						productImages.Add ( new ImageInfoResponse()
						{
							 Url = image.Url,
							 PlacementName = image.PlacementName
						});
				}
			}

			return productImages.ToArray();
		}

		/// <summary>
		///	Gets filtered items
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable<InventoryItemResponse> GetFilteredItems(ItemsFilter filter, Mark mark = null)
		{
			return GetFilteredItemsAsync( filter, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets filtered items asynchronously
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task<IEnumerable<InventoryItemResponse>> GetFilteredItemsAsync(ItemsFilter filter, Mark mark = null)
		{
			var pagedResult = await GetFilteredItemsAsync( filter, 1, pageSize, mark ).ConfigureAwait( false );

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
		public async Task<PagedApiResponse<InventoryItemResponse>> GetFilteredItemsAsync(ItemsFilter filter, int startPage, int pageLimit, Mark mark = null)
		{
			List< InventoryItemResponse > result = new List< InventoryItemResponse >();

			string filterParam = GetFilter( filter );

			var products = await GetProducts( filterParam, mark, startPage: startPage ).ConfigureAwait( false );

			foreach( var product in products )
			{
				result.Add( product.ToInventoryItemResponse() );
			}

			int finalPage = 0;

			if ( products.Count() > 0 )
				finalPage = (int)Math.Ceiling( products.Count() / pageSize * 1.0 );

			return new PagedApiResponse< InventoryItemResponse >( result.ToArray(), finalPage, true );
		}

		/// <summary>
		///	Gets filtered skus
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public IEnumerable<string> GetFilteredSkus(ItemsFilter filter, Mark mark = null)
		{
			return GetFilteredSkusAsync( filter, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Gets filtered skus asynchronously
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter, Mark mark = null )
		{
			var pagedResponse = await GetFilteredSkusAsync( filter, 1, pageSize, mark ).ConfigureAwait( false );

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
			var pagedItems = await GetFilteredItemsAsync( filter, startPage, pageLimit, mark ).ConfigureAwait( false );
			var items = pagedItems.Response;

			int finalPage = 0;

			if ( items.Count() > 0 )
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
			SynchItemAsync( item, isCreateNew, mark ).GetAwaiter().GetResult();
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
			var product = await GetProductBySku( item.Sku, mark ).ConfigureAwait( false );

			if ( product == null )
				return;

			var request = GetUpdateFields( item );

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				await AP.CreateQueryAsync(ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					string url = $"{ productsUrl }({ product.ID })";
					var responseStatusCode = await PutAsync( url, request ).ConfigureAwait( false );

					if (responseStatusCode != System.Net.HttpStatusCode.NoContent)
						throw new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo()), new Exception("Update product fields failed!") );
				}).ConfigureAwait( false );
						
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
		public void SynchItems( IEnumerable<InventoryItemSubmit> items, bool isCreateNew = false, Mark mark = null )
		{
			SynchItemsAsync( items, isCreateNew, mark ).GetAwaiter().GetResult();
		}

		/// <summary>
		///	Updates products fields asynchronously
		/// </summary>
		/// <param name="items"></param>
		/// <param name="isCreateNew"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task SynchItemsAsync(IEnumerable<InventoryItemSubmit> items, bool isCreateNew = false, Mark mark = null)
		{
			foreach( var item in items )
				await SynchItemAsync( item, isCreateNew, mark ).ConfigureAwait( false );
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
		///	Gets product by sku asynchronously
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="mark"></param>
		/// <param name="includeDCQuantities"></param>
		/// <param name="includeAttributes"></param>
		/// <param name="includeImages"></param>
		/// <returns></returns>
		private async Task< Product > GetProductBySku( string sku, Mark mark, bool includeDCQuantities = false, bool includeAttributes = false, bool includeImages = false )
		{
			string filter = $"$filter=sku eq '{ sku }'";

			var products = await GetProducts( filter, mark, 1, includeDCQuantities, includeAttributes, includeImages ).ConfigureAwait( false );

			return products.FirstOrDefault();
		}

		/// <summary>
		///	Gets products
		/// </summary>
		/// <param name="sku"></param>
		/// <returns></returns>
		private async Task< Product[] > GetProducts( string filter, Mark mark, int startPage = 1, bool includeDCQuantities = false, bool includeAttributes = false, bool includeImages = false )
		{
			List<Product> products = new List<Product>();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			string url = productsUrl;

			List< string > requestParams = new List< string >();

			// filter products by sku
			if (!string.IsNullOrEmpty(filter))
				requestParams.Add(filter);

			// include extra data in server response
			if ( includeDCQuantities
				|| includeAttributes
				|| includeImages )
			{
				string expandParam = "$expand=";

				if ( includeDCQuantities )
					expandParam += "DCQuantities,";
				else if ( includeAttributes )
					expandParam += "Attributes,";
				else if ( includeImages )
					expandParam += "Images";

				requestParams.Add( expandParam );
			}

			// paging
			if ( startPage > 1)
				requestParams.Add( $"$skip={ ( startPage - 1 ) * pageSize }");

			try
			{
				string nextLink = url;

				// server returns large dataset by pages
				while( nextLink != null )
				{
					if ( requestParams.Count > 0 )
						url = productsUrl + "?" + string.Join("&", requestParams.ToArray());

					ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo()));

					var productResponse = await AP.CreateQueryAsync(ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
					{
						var response = await GetResponseAsync( url ).ConfigureAwait( false );
						var data = await response.Content.ReadAsAsync< ODataResponse< Product > >().ConfigureAwait( false );
					
						return data;
					}).ConfigureAwait( false );

					
					nextLink = productResponse.NextLink;
					products.AddRange(productResponse.Value);
					
					// extra dataset
					if (!string.IsNullOrEmpty( nextLink ))
					{
						requestParams.Clear();
						requestParams.Add(nextLink.Split('?')[1]);
					}

					ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, 
														  methodResult : productResponse.Value != null ? productResponse.Value.ToJson() : "", 
														  additionalInfo : this.AdditionalLogInfo()) );
				}
				
			}
			catch(Exception exception)
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo()), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}

			return products.ToArray();
		}
	
		/// <summary>
		///	Gets product by ID
		/// </summary>
		/// <param name="productID"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		private async Task< Product > GetProductByID ( int productID, Mark mark )
		{
			Product product = null;

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			string url = productsUrl + "(" + productID.ToString() + ")";

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo()));

				var productResponse = await AP.CreateQueryAsync(ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					var response = await GetResponseAsync( url ).ConfigureAwait( false );

					return await response.Content.ReadAsAsync< ODataResponse< Product > >();
				}).ConfigureAwait( false );

				product = productResponse.Value.FirstOrDefault();

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, 
													  methodResult : productResponse.Value != null ? productResponse.Value.ToJson() : "", 
													  additionalInfo : this.AdditionalLogInfo()) );
			}
			catch(Exception exception)
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo()), exception );
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
			List< string > filterClauses = new List< string >();

			if ( filter.Criteria != null )
			{
				var criteria = filter.Criteria;

				if ( !string.IsNullOrEmpty( criteria.ClassificationName  ))
					filterClauses.Add( $"Classification eq '{ criteria.ClassificationName }'" );

				if ( !string.IsNullOrEmpty( criteria.LabelName ))
					filterClauses.Add( $"Labels/Any (l: l/Name eq '{ criteria.LabelName }')" );

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
							filterClauses.Add( $"{ filterFieldName } ge { this.ConvertDate( criteria.DateRangeStartGMT.Value ) } " );

						if ( criteria.DateRangeEndGMT.HasValue )
							filterClauses.Add( $"{ filterFieldName } le { this.ConvertDate( criteria.DateRangeEndGMT.Value ) }" );
					}
				}
			}

			return "$filter=" + string.Join(" and ", filterClauses.ToArray());
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
				object value = property.GetValue( item );

				if ( value != null 
					&& !value.GetType().IsArray
					&& !property.Name.Equals("Sku") )
				{
					if ( value is PriceInfo )
					{
						var priceProperties = GetUpdateFields( (PriceInfo) value );

						foreach( var priceField in priceProperties )
							fields.Add( priceField.Key, priceField.Value );
					}
					else
						fields.Add( property.Name, value );
				}
			}

			return fields;
		}
	}
}
