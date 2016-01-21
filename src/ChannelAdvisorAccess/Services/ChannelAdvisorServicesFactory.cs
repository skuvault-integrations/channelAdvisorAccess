using System;
using System.Runtime.Caching;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services.Admin;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;

namespace ChannelAdvisorAccess.Services
{
	public class ChannelAdvisorServicesFactory : IChannelAdvisorServicesFactory
	{
		private readonly string _developerKey;
		private readonly string _developerPassword;
		private readonly ObjectCache _cache;
		private readonly TimeSpan _slidingCacheExpiration;

		public ChannelAdvisorServicesFactory( string developerKey, string developerPassword, ObjectCache cache = null ) :
				this( developerKey, developerPassword, cache, ObjectCache.NoSlidingExpiration )
		{
			this._developerPassword = developerPassword;
			this._developerKey = developerKey;
		}

		public ChannelAdvisorServicesFactory( string developerKey, string developerPassword, ObjectCache cache, TimeSpan slidingCacheExpiration )
		{
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

		public IOrdersService CreateOrdersService( string accountName, string accountId )
		{
			var ordersCredentials = new APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new OrdersService( ordersCredentials, accountName, accountId, _cache );
		}

		public IItemsService CreateItemsService( string accountName, string accountId )
		{
			var inventoryCredentials = new InventoryService.APICredentials { DeveloperKey = this._developerKey, Password = this._developerPassword };
			return new ItemsService( inventoryCredentials, accountName, accountId, _cache ){ SlidingCacheExpiration = _slidingCacheExpiration };
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