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
			SetSecurityProtocol();
			var adminCredentials = new AdminService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new Admin.AdminService( adminCredentials );
		}

		/// <summary>
		///	Returns orders service
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public IOrdersService CreateOrdersService( ChannelAdvisorConfig config )
		{
			if ( config.ApiVersion == ChannelAdvisorApiVersion.Soap )
				return CreateOrdersService( config.AccountName, config.AccountId );
			else
				return CreateOrdersRestService( config.AccountName, config.AccountId, config.AccessToken, config.RefreshToken, config.SoapCompatibilityAuth );
		}

		/// <summary>
		///	Returns Soap orders service for concrete tenant
		/// </summary>
		/// <param name="accountName">User friendly account name</param>
		/// <param name="accountId">Tenant account Id  </param>
		/// <returns></returns>
		public IOrdersService CreateOrdersService( string accountName, string accountId )
		{
			SetSecurityProtocol();
			var ordersCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new OrdersService( ordersCredentials, accountName, accountId, this._cache );
		}

		/// <summary>
		///	Returns Rest service with soap compatible authorization flow for existing tenants
		/// </summary>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="accountId">Tenant account id</param>
		/// <returns></returns>
		public IOrdersService CreateOrdersRestServiceWithSoapCompatibleAuth( string accountName, string accountId )
		{
			SetSecurityProtocol();
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );
			var soapCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			
			return new REST.Services.Orders.OrdersService( credentials, soapCredentials, accountId, accountName, this._cache );
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
		public IOrdersService CreateOrdersRestService( string accountName, string accountId, string accessToken, string refreshToken, bool soapCompatibleAuth = false )
		{
			SetSecurityProtocol();

			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );

			if ( soapCompatibleAuth )
				return this.CreateOrdersRestServiceWithSoapCompatibleAuth( accountName, accountId );
			else
				return new REST.Services.Orders.OrdersService( credentials, accountName, accessToken, refreshToken );
		}

		/// <summary>
		///	Returns Items service
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public IItemsService CreateItemsService( ChannelAdvisorConfig config )
		{
			if ( config.ApiVersion == ChannelAdvisorApiVersion.Soap )
				return CreateItemsService( config.AccountName, config.AccountId );
			else
				return CreateItemsRestService( config.AccountName, config.AccountId, config.AccessToken, config.RefreshToken, config.SoapCompatibilityAuth );
		}

		/// <summary>
		///	Returns Soap items service for concrete tenant
		/// </summary>
		/// <param name="accountName">User friendly account name</param>
		/// <param name="accountId">Tenant account id (GUID)</param>
		/// <returns></returns>
		public IItemsService CreateItemsService( string accountName, string accountId )
		{
			SetSecurityProtocol();

			var inventoryCredentials = new InventoryService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new ItemsService( inventoryCredentials, accountName, accountId, this._cache ){ SlidingCacheExpiration = this._slidingCacheExpiration };
		}

		/// <summary>
		///	Returns Rest items service with SOAP compatible authorization flow
		/// </summary>
		/// <param name="accountName"></param>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public IItemsService CreateItemsRestServiceWithSoapCompatibleAuth( string accountName, string accountId )
		{
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );
			var soapCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };

			return new REST.Services.Items.ItemsService( credentials, soapCredentials, accountId, accountName, this._cache );
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
		public IItemsService CreateItemsRestService( string accountName, string accountId, string accessToken, string refreshToken, bool soapCompatibleAuth = false )
		{
			var credentials = new RestCredentials( this._applicationId, this._sharedSecret );

			if ( soapCompatibleAuth )
				return this.CreateItemsRestServiceWithSoapCompatibleAuth( accountName, accountId );
			else
				return new REST.Services.Items.ItemsService( credentials, accountName, accessToken, refreshToken );
		}

		public IShippingService CreateShippingService( string accountName, string accountId )
		{
			SetSecurityProtocol();
			var shippingCredentials = new ShippingService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new Shipping.ShippingService( shippingCredentials, accountName, accountId );
		}

		public IListingService CreateListingService( string accountName, string accountId )
		{
			SetSecurityProtocol();
			var listingCredentials = new ListingService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new Listing.ListingService( listingCredentials, accountName, accountId );
		}

		#region SSL certificate hack
		public static void SetSecurityProtocol()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
		}
		#endregion
	}
}