using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.REST.Shared;
using ChannelAdvisorAccess.Services.Admin;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;

namespace ChannelAdvisorAccess.Services
{
	public interface IChannelAdvisorServicesFactory
	{
		IAdminService CreateAdminService();
		IOrdersService CreateOrdersService( ChannelAdvisorConfig config, ChannelAdvisorTimeouts timeouts );
		IItemsService CreateItemsService( ChannelAdvisorConfig config, ChannelAdvisorTimeouts timeouts );
		IShippingService CreateShippingService( string accountName, string accountId );
		IListingService CreateListingService( string accountName, string accountId );
		IItemsService CreateItemsPagingService( ChannelAdvisorConfig config, ChannelAdvisorTimeouts timeouts );
	}
}