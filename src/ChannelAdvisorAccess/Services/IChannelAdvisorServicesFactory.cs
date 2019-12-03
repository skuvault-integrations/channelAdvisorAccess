using ChannelAdvisorAccess.Misc;
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
		IOrdersService CreateOrdersService( ChannelAdvisorConfig config );
		IItemsService CreateItemsService( ChannelAdvisorConfig config, LogDetailsEnum logDetailsEnum = LogDetailsEnum.Undefined );
		IShippingService CreateShippingService( string accountName, string accountId );
		IListingService CreateListingService( string accountName, string accountId );
	}
}