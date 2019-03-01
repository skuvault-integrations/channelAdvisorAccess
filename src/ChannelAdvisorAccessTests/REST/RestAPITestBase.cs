using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Admin;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;
using ChannelAdvisorAccess.REST.Services;
using ChannelAdvisorAccessTests;
using Netco.Logging;
using Netco.Logging.NLogIntegration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccessTests.REST
{
	public class RestAPITestBase
	{
		protected const string applicationID = "f7g356m232wudcskihy3auo292aug7dx";
		protected const string sharedSecret = "q2eg7dH8uUG9W58Chatp3w";
		protected const string accessToken = "G_n3HbkCcCohMrt2uVkpGL7oEgCRdlB-PO3XlRX2h0M-24985";
		protected const string refreshToken = "6NHCkxT3aWd-MYZubUKVKGHHoCkR2XsFpnJyPSJ4-E0";
		protected const string apiUrl = "https://api.channeladvisor.com";

		[ SetUp ]
		public void Init()
		{
			var factory = new ChannelAdvisorAccess.REST.Services.ChannelAdvisorServicesFactory( apiUrl, applicationID, sharedSecret, accessToken, refreshToken );
			this.OrdersService = factory.CreateOrdersService( null, null );
			this.ItemsService = factory.CreateItemsService(null, null );

			NetcoLogger.LoggerFactory = new NLogLoggerFactory();
		}

		public IShippingService ShippingService{ get; private set; }

		public IOrdersService OrdersService{ get; private set; }

		public IListingService ListingService{ get; private set; }

		public IItemsService ItemsService{ get; private set; }

		public IAdminService AdminService{ get; private set; }
	}
}
