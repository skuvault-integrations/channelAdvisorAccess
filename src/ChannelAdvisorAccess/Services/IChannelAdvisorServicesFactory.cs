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
		IOrdersService CreateOrdersService( string accountName, string accountId );
		IOrdersService CreateOrdersRestServiceWithSoapCompatibleAuth( string accountName, string accountId );
		IOrdersService CreateOrdersRestService( string accountName, string accountId, string accessToken, string refreshToken, bool soapCompatibleAuth );
		IItemsService CreateItemsService( string accountName, string accountId );
		IItemsService CreateItemsRestServiceWithSoapCompatibleAuth( string accountName, string accountId );
		IItemsService CreateItemsRestService( string accountName, string accountId, string accessToken, string refreshToken, bool soapCompatibleAuth );
		IShippingService CreateShippingService( string accountName, string accountId );
		IListingService CreateListingService( string accountName, string accountId );
	}
}