using System.Collections.Generic;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;

namespace ChannelAdvisorAccess.Services
{
	public interface IChannelAdvisorManager
	{
		Dictionary< string, ChannelAdvisorAccount > AllAccountsByName { get; }
		Dictionary< string, ChannelAdvisorAccount > ActiveAccountsByName { get; }
		Dictionary< string, ChannelAdvisorAccount > AllAccountsById { get; }
		Dictionary< string, ChannelAdvisorAccount > ActiveAccountsById { get; }
		IEnumerable< string > AllAccountNames { get; }
		IEnumerable< string > AllAccountCodes { get; }
		IEnumerable< string > ActiveAccountCodes { get; }
		IEnumerable< string > ActiveAccountNames { get; }
		IItemsService GetItemsServiceByAccountName( string accountName );
		IItemsService GetItemsServiceByAccountId( string accountId );
		IOrdersService GetOrdersServiceByAccountName( string accountName );
		IOrdersService GetOrdersServiceByAccountId( string accountId );
		IShippingService GetShippingServiceByAccountName( string accountName );
		IShippingService GetShippingServiceByAccountId( string accountId );
	}
}