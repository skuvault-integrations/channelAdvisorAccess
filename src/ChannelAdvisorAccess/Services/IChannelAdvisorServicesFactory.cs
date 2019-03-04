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
		IOrdersService CreateOrdersService( string accountName, string accountId, bool useRestVersion = false, bool useCompatibleAuth = true, string accessToken = null, string refreshToken = null );
		IItemsService CreateItemsService( string accountName, string accountId, bool useRestVersion = false, bool useCompatibleAuth = true, string accessToken = null, string refreshToken = null );
		IShippingService CreateShippingService( string accountName, string accountId );
		IListingService CreateListingService( string accountName, string accountId );
	}
}