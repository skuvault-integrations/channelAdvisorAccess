using System.Collections.Generic;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;

namespace ChannelAdvisorAccess.Services
{
	public class ChannelAdvisorManager : IChannelAdvisorManager
	{
		public Dictionary< string, ChannelAdvisorAccount > AllAccountsByName { get; private set; }
		public Dictionary< string, ChannelAdvisorAccount > ActiveAccountsByName { get; private set; }
		public Dictionary< string, ChannelAdvisorAccount > AllAccountsById { get; private set; }
		public Dictionary< string, ChannelAdvisorAccount > ActiveAccountsById { get; private set; }

		#region Initialization
		public ChannelAdvisorManager( IEnumerable< ChannelAdvisorAccount > accounts )
		{
			this.AllAccountsByName = new Dictionary< string, ChannelAdvisorAccount >();
			this.ActiveAccountsByName = new Dictionary< string, ChannelAdvisorAccount >();
			this.AllAccountsById = new Dictionary< string, ChannelAdvisorAccount >();
			this.ActiveAccountsById = new Dictionary< string, ChannelAdvisorAccount >();
			foreach( var account in accounts )
			{
				this.AllAccountsByName.Add( account.Name, account );
				this.AllAccountsById.Add( account.Id, account );
				if( account.IsActive )
				{
					this.ActiveAccountsByName.Add( account.Name, account );
					this.ActiveAccountsById.Add( account.Id, account );
				}
			}
		}
		#endregion

		#region Services access
		public IEnumerable< string > AllAccountNames
		{
			get { return this.AllAccountsByName.Keys; }
		}

		public IEnumerable< string > AllAccountCodes
		{
			get
			{
				return this.AllAccountsById.Keys;
			}
		}

		public IEnumerable< string > ActiveAccountCodes
		{
			get
			{
				return this.ActiveAccountsById.Keys;
			}
		}

		public IEnumerable< string > ActiveAccountNames
		{
			get { return this.ActiveAccountsByName.Keys; }
		}

		public IItemsService GetItemsServiceByAccountName( string accountName )
		{
			return this.AllAccountsByName[ accountName ].ItemsService;
		}

		public IItemsService GetItemsServiceByAccountId( string accountId )
		{
			return this.AllAccountsById[ accountId ].ItemsService;
		}

		public IOrdersService GetOrdersServiceByAccountName( string accountName )
		{
			return this.AllAccountsByName[ accountName ].OrdersService;
		}

		public IOrdersService GetOrdersServiceByAccountId( string accountId )
		{
			return this.AllAccountsById[ accountId ].OrdersService;
		}

		public IShippingService GetShippingServiceByAccountName( string accountName )
		{
			return this.AllAccountsByName[ accountName ].ShippingService;
		}

		public IShippingService GetShippingServiceByAccountId( string accountId )
		{
			return this.AllAccountsById[ accountId ].ShippingService;
		}

		public IListingService GetListingServiceByAccountId( string accountId )
		{
			return this.AllAccountsById[ accountId ].ListingService;
		}
		#endregion
	}
}