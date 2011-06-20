using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;

namespace ChannelAdvisorAccess.Services
{
	public interface IChannelAdvisorServicesFactory
	{
		IOrdersService CreateOrdersService( string accountName, string accountId );
		IItemsService CreateItemsService( string accountName, string accountId );
		IShippingService CreateShippingService( string accountName, string accountId );
		IListingService CreateListingService( string accountName, string accountId );
	}
}