using System;
using System.Runtime.Caching;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services.Admin;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;
using System.Net;
using ChannelAdvisorAccess.REST.Shared;
using ChannelAdvisorAccess.Misc;

namespace ChannelAdvisorAccess.Services
{
	public class ChannelAdvisorServicesFactory : IChannelAdvisorServicesFactory
	{
		private readonly string _developerKey;
		private readonly string _developerPassword;
		private readonly string _applicationId;
		private readonly string _sharedSecret;
		private readonly ObjectCache _cache;
		private readonly TimeSpan _slidingCacheExpiration;

		public ChannelAdvisorServicesFactory( string developerKey, string developerPassword, string applicationId, string sharedSecret, ObjectCache cache = null ) :
				this( developerKey, developerPassword, applicationId, sharedSecret, cache, ObjectCache.NoSlidingExpiration )
		{ }

		public ChannelAdvisorServicesFactory( string developerKey, string developerPassword, string applicationId, string sharedSecret, ObjectCache cache, TimeSpan slidingCacheExpiration )
		{
			this._applicationId = applicationId;
			this._sharedSecret = sharedSecret;

			this._developerPassword = developerPassword;
			this._developerKey = developerKey;
			this._cache = cache;
			this._slidingCacheExpiration = slidingCacheExpiration;
		}

		public IAdminService CreateAdminService()
		{
			var adminCredentials = new AdminService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new Admin.AdminService( adminCredentials );
		}

		/// <summary>
		///	Returns orders service
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public IOrdersService CreateOrdersService( ChannelAdvisorConfig config, ChannelAdvisorTimeouts timeouts )
		{
			if ( config.ApiVersion == ChannelAdvisorApiVersion.Soap )
				return CreateOrdersService( config.AccountName, config.AccountId );
			else
				return CreateOrdersRestService( config.AccountName, config.AccountId, config.AccessToken, config.RefreshToken, timeouts, config.SoapCompatibilityAuth );
		}

		/// <summary>
		///	Returns Soap orders service for concrete tenant
		/// </summary>
		/// <param name="accountName">User friendly account name</param>
		/// <param name="accountId">Tenant account Id  </param>
		/// <returns></returns>
		public IOrdersService CreateOrdersService( string accountName, string accountId )
		{
			var ordersCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new OrdersService( ordersCredentials, accountName, accountId, this._cache );
		}

		/// <summary>
		///	Returns Rest service with soap compatible authorization flow for existing tenants
		/// </summary>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="accountId">Tenant account id</param>
		/// <param name="itemsService">Items service (to get Distribution Centers)</param>
		/// <returns></returns>
		public IOrdersService CreateOrdersRestServiceWithSoapCompatibleAuth( string accountName, string accountId, IItemsService itemsService, ChannelAdvisorTimeouts timeouts )
		{
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );
			var soapCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			
			return new REST.Services.Orders.OrdersService( credentials, soapCredentials, accountId, accountName, itemsService, timeouts );
		}

		/// <summary>
		///	Returns Rest service with standard authorization flow for working with orders
		/// </summary>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="accountId">Tenant account id</param>
		/// <param name="accessToken">Tenant access token</param>
		/// <param name="refreshToken">Tenant refresh token</param>
		/// <param name="soapCompatibleAuth">Soap compatible authorization flow</param>
		/// <returns></returns>
		public IOrdersService CreateOrdersRestService( string accountName, string accountId, string accessToken, string refreshToken, ChannelAdvisorTimeouts timeouts, bool soapCompatibleAuth = false )
		{
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );

			if ( soapCompatibleAuth )
			{
				var itemsService = this.CreateItemsRestServiceWithSoapCompatibleAuth( accountName, accountId, timeouts );
				return this.CreateOrdersRestServiceWithSoapCompatibleAuth( accountName, accountId, itemsService, timeouts );
			}
			else
			{ 
				var itemsService = new REST.Services.Items.ItemsService( credentials, accountName, accessToken, refreshToken, timeouts );
				return new REST.Services.Orders.OrdersService( credentials, accountName, accessToken, refreshToken, itemsService, timeouts );
			}
		}

		/// <summary>
		///	Returns Items service
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public IItemsService CreateItemsService( ChannelAdvisorConfig config, ChannelAdvisorTimeouts timeouts )
		{
			if ( config.ApiVersion == ChannelAdvisorApiVersion.Soap )
				return CreateItemsService( config.AccountName, config.AccountId );
			else
				return CreateItemsRestService( config.AccountName, config.AccountId, config.AccessToken, config.RefreshToken, timeouts, config.SoapCompatibilityAuth );
		}

		///  <summary>
		/// 	Returns Items service
		///  </summary>
		///  <param name="config"></param>
		///  <param name="timeouts"></param>
		///  <returns></returns>
		public IItemsService CreateItemsPagingService( ChannelAdvisorConfig config, ChannelAdvisorTimeouts timeouts )
		{
			return config.ApiVersion == ChannelAdvisorApiVersion.Soap
				? this.CreateItemsService( config.AccountName, config.AccountId )
				: this.CreateItemsPagingRestService( config.AccountName, config.AccountId, config.AccessToken, config.RefreshToken, timeouts, config.SoapCompatibilityAuth );
		}

		/// <summary>
		///	Returns Soap items service for concrete tenant
		/// </summary>
		/// <param name="accountName">User friendly account name</param>
		/// <param name="accountId">Tenant account id (GUID)</param>
		/// <returns></returns>
		public IItemsService CreateItemsService( string accountName, string accountId )
		{
			var inventoryCredentials = new InventoryService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new ItemsService( inventoryCredentials, accountName, accountId, this._cache ){ SlidingCacheExpiration = this._slidingCacheExpiration };
		}

		/// <summary>
		///	Returns Rest items service with SOAP compatible authorization flow
		/// </summary>
		/// <param name="accountName"></param>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public IItemsService CreateItemsRestServiceWithSoapCompatibleAuth( string accountName, string accountId, ChannelAdvisorTimeouts timeouts )
		{
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );
			var soapCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };

			return new REST.Services.Items.ItemsService( credentials, soapCredentials, accountId, accountName, timeouts );
		}

		/// <summary>
		///	Returns items Rest service with standard authorization flow
		/// </summary>
		/// <param name="accountName"></param>
		/// <param name="accountId"></param>
		/// <param name="accessToken"></param>
		/// <param name="refreshToken"></param>
		/// <param name="soapCompatibleAuth"></param>
		/// <returns></returns>
		public IItemsService CreateItemsRestService( string accountName, string accountId, string accessToken, string refreshToken, ChannelAdvisorTimeouts timeouts, bool soapCompatibleAuth = false )
		{
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );

			if ( soapCompatibleAuth )
				return this.CreateItemsRestServiceWithSoapCompatibleAuth( accountName, accountId, timeouts );
			else
				return new REST.Services.Items.ItemsService( credentials, accountName, accessToken, refreshToken, timeouts );
		}

		///  <summary>
		/// 	Returns Rest items service with SOAP compatible authorization flow which return all products in one request
		///  </summary>
		///  <param name="accountName"></param>
		///  <param name="accountId"></param>
		///  <param name="timeouts"></param>
		///  <returns></returns>
		public IItemsService CreateItemsPagingRestServiceWithSoapCompatibleAuth( string accountName, string accountId, ChannelAdvisorTimeouts timeouts )
		{
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );
			var soapCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };

			return new REST.Services.Items.ItemsPagingService( credentials, soapCredentials, accountId, accountName, timeouts );
		}

		///  <summary>
		/// 	Returns items Rest service with standard authorization flow which return all products in one request
		///  </summary>
		///  <param name="accountName"></param>
		///  <param name="accountId"></param>
		///  <param name="accessToken"></param>
		///  <param name="refreshToken"></param>
		///  <param name="timeouts"></param>
		///  <param name="soapCompatibleAuth"></param>
		///  <returns></returns>
		public IItemsService CreateItemsPagingRestService( string accountName, string accountId, string accessToken, string refreshToken, ChannelAdvisorTimeouts timeouts, bool soapCompatibleAuth = false )
		{
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );

			return soapCompatibleAuth
				? this.CreateItemsRestServiceWithSoapCompatibleAuth( accountName, accountId, timeouts )
				: new REST.Services.Items.ItemsPagingService( credentials, accountName, accessToken, refreshToken, timeouts );
		}

		public IShippingService CreateShippingService( string accountName, string accountId )
		{
			var shippingCredentials = new ShippingService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new Shipping.ShippingService( shippingCredentials, accountName, accountId );
		}

		public IListingService CreateListingService( string accountName, string accountId )
		{
			var listingCredentials = new ListingService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new Listing.ListingService( listingCredentials, accountName, accountId );
		}
	}
}