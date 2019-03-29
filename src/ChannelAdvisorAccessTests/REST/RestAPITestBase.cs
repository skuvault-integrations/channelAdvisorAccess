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
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccessTests.REST
{
	public class RestServiceCredentials
	{
		public string ApplicationId { get; set; }
		public string SharedSecret { get; set; }
		public string DeveloperKey{ get; set; }
		public string DeveloperPassword{ get; set; }
		public string AccountId{ get; set; }
		public string AccountName { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set;}
		public bool useSoapCredentials { get; set; }
	}

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
			var credentials = LoadCredentials();
			var factory = new ChannelAdvisorServicesFactory( credentials.DeveloperKey, credentials.DeveloperPassword, credentials.ApplicationId, credentials.SharedSecret );

			if ( credentials.useSoapCredentials )
			{
				this.OrdersService = factory.CreateOrdersRestServiceWithSoapCompatibleAuth( credentials.AccountName, credentials.AccountId );
				this.ItemsService = factory.CreateItemsRestServiceWithSoapCompatibleAuth( credentials.AccountName, credentials.AccountId );
			}
			else
			{
				this.OrdersService = factory.CreateOrdersRestService( credentials.AccountName, credentials.AccessToken, credentials.RefreshToken );
				this.ItemsService = factory.CreateItemsRestService( credentials.AccountName, credentials.AccessToken, credentials.RefreshToken );
			}

			NetcoLogger.LoggerFactory = new NLogLoggerFactory();
		}

		private RestServiceCredentials LoadCredentials()
		{
			var path = new Uri( Path.GetDirectoryName( Assembly.GetExecutingAssembly().CodeBase ) ).LocalPath;

			using( var reader = new StreamReader( path + @"\..\..\rest-credentials.csv" ) )
			{
				return new RestServiceCredentials
				{
					ApplicationId = reader.ReadLine(),
					SharedSecret = reader.ReadLine(),
					DeveloperKey = reader.ReadLine(),
					DeveloperPassword = reader.ReadLine(),
					AccountId = reader.ReadLine(),
					AccountName = reader.ReadLine(),
					AccessToken = reader.ReadLine(),
					RefreshToken = reader.ReadLine(),
					useSoapCredentials = bool.Parse( reader.ReadLine() )
				};
			}
		}

		public IShippingService ShippingService{ get; private set; }

		public IOrdersService OrdersService{ get; private set; }

		public IListingService ListingService{ get; private set; }

		public IItemsService ItemsService{ get; private set; }

		public IAdminService AdminService{ get; private set; }
	}
}
