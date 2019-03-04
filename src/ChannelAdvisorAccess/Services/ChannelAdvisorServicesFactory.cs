using System;
using System.Runtime.Caching;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services.Admin;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;
using System.Net;
using ChannelAdvisorAccess.Misc;

namespace ChannelAdvisorAccess.Services
{
	public class ChannelAdvisorServicesFactory : IChannelAdvisorServicesFactory
	{
		private readonly string _developerKey;
		private readonly string _developerPassword;
		private readonly RestApplication _application;
		private readonly ObjectCache _cache;
		private readonly TimeSpan _slidingCacheExpiration;

		public ChannelAdvisorServicesFactory( string developerKey, string developerPassword, string applicationId, string sharedSecret, ObjectCache cache = null ) :
				this( developerKey, developerPassword, applicationId, sharedSecret, cache, ObjectCache.NoSlidingExpiration )
		{ }

		public ChannelAdvisorServicesFactory( string developerKey, string developerPassword, string applicationId, string sharedSecret, ObjectCache cache, TimeSpan slidingCacheExpiration )
		{
			if ( applicationId != null )
				this._application = new RestApplication( applicationId, sharedSecret );

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
		///	Returns orders service for concrete tenant
		/// </summary>
		/// <param name="accountName">User friendly account name</param>
		/// <param name="accountId">Tenant account Id (GUID) </param>
		/// <param name="useRestVersion">Using CA Rest API version</param>
		/// <param name="useCompatibleAuth">Using CA Rest API version with SOAP credentials</param>
		/// <param name="accessToken">Access token</param>
		/// <param name="refreshToken">Refresh token</param>
		/// <returns></returns>
		public IOrdersService CreateOrdersService( string accountName, string accountId, bool useRestVersion = false, bool useCompatibleAuth = true, string accessToken = null, string refreshToken = null )
		{
			SetSecurityProtocol();

			if ( !useRestVersion )
			{
				var ordersCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
				return new OrdersService( ordersCredentials, accountName, accountId, _cache );
			}
			else
			{
				if ( useCompatibleAuth )
					return new REST.Services.OrdersService( _application, accountId, accountName, _developerKey, _developerPassword );
				else
					return new REST.Services.OrdersService( _application, accountName, accessToken, refreshToken );
			}

		}

		/// <summary>
		///	Returns items service for concrete tenant
		/// </summary>
		/// <param name="accountName">User friendly account name</param>
		/// <param name="accountId">Tenant account id (GUID)</param>
		/// <param name="useRestVersion">Use CA REST API version</param>
		/// <param name="useCompatibleAuth">Use CA REST API version with SOAP credentials</param>
		/// <param name="accessToken">Access token</param>
		/// <param name="refreshToken">Refresh token</param>
		/// <returns></returns>
		public IItemsService CreateItemsService( string accountName, string accountId, bool useRestVersion = false, bool useCompatibleAuth = true, string accessToken = null, string refreshToken = null )
		{
			SetSecurityProtocol();

			if ( !useRestVersion )
			{
				var inventoryCredentials = new InventoryService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
				return new ItemsService( inventoryCredentials, accountName, accountId, _cache ){ SlidingCacheExpiration = _slidingCacheExpiration };
			}
			else
			{
				if ( useCompatibleAuth )
					return new REST.Services.ItemsService( _application, accountName, accountId, _developerKey, _developerPassword );
				else
					return new REST.Services.ItemsService( _application, accountName, accessToken, refreshToken );
			}

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