using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Caching;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Admin;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests
{
	[ SuppressMessage( "Structure", "NUnit1032:An IDisposable field/property should be Disposed in a TearDown method" ) ]
	public class TestsBase
	{
		protected const string TestDistributionCenterCode = "Louisville";
		protected const string TestSku = "testSku1";
		protected DateTime serviceLastActivityDateTime = DateTime.Now;

		protected Mark Mark
		{
			get
			{
				return Mark.CreateNew();
			}
		}

		[ SetUp ]
		public void Init()
		{
			var factory = new ChannelAdvisorServicesFactory( Credentials.DeveloperKey, Credentials.DeveloperPassword, null, null, MemoryCache.Default );
			this.AdminService = factory.CreateAdminService();
			this.ItemsService = factory.CreateItemsService( "test", Credentials.AccountId );
			this.ListingService = factory.CreateListingService( "test", Credentials.AccountId );
			this.OrdersService = factory.CreateOrdersService( "test", Credentials.AccountId );
			this.ShippingService = factory.CreateShippingService( "test", Credentials.AccountId );
			this.serviceLastActivityDateTime = DateTime.Now;
		}

		public IShippingService ShippingService{ get; private set; }

		public IOrdersService OrdersService{ get; private set; }

		public IListingService ListingService{ get; private set; }

		public IItemsService ItemsService{ get; private set; }

		public IAdminService AdminService{ get; private set; }

		private static TestCredentials _credentials;

		public static TestCredentials Credentials
		{
			get { return _credentials ?? ( _credentials = LoadCredentials() ); }
		}

		private static TestCredentials LoadCredentials()
		{
			using( var reader = new StreamReader( @"..\..\credentials.csv" ) )
			{
				return new TestCredentials
				{
					DeveloperKey = reader.ReadLine(),
					DeveloperPassword = reader.ReadLine(),
					AccountId = reader.ReadLine()
				};
			}
		}
	}

	public class TestCredentials
	{
		public string DeveloperKey{ get; set; }
		public string DeveloperPassword{ get; set; }
		public string AccountId{ get; set; }
	}
}