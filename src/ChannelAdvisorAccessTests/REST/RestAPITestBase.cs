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
		
		protected const string developerKey = "505f8e5a-b342-47ec-ba3e-5bd26a3b9d4b";
		protected const string developerPassword = "2IMOFe7zE#r@";
		
		protected const string accountId = "41d5dc98-9bed-4a66-a46a-ad9edef85734";
		protected const string accountName = "Integration Partner - Agile Harbor LLC - US";

		private const bool useSOAPCredentials = false;

		[ SetUp ]
		public void Init()
		{
			var factory = new ChannelAdvisorServicesFactory( developerKey, developerPassword, applicationID, sharedSecret );

			if ( useSOAPCredentials )
			{
				this.OrdersService = factory.CreateOrdersService( accountName, accountId, true, true, accessToken, refreshToken );
				this.ItemsService = factory.CreateItemsService( accountName, accountId, true, true, accessToken, refreshToken );
			}
			else
			{
				this.OrdersService = factory.CreateOrdersService( accountName, accountId, true, false, accessToken, refreshToken );
				this.ItemsService = factory.CreateItemsService( accountName, accountId );
			}
				
			NetcoLogger.LoggerFactory = new NLogLoggerFactory();
		}

		public IShippingService ShippingService{ get; private set; }

		public IOrdersService OrdersService{ get; private set; }

		public IListingService ListingService{ get; private set; }

		public IItemsService ItemsService{ get; private set; }

		public IAdminService AdminService{ get; private set; }
	}
}
