using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using ChannelAdvisorAccess.AdminService;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;
using ChannelAdvisorAccess.Services.Admin;

namespace ChannelAdvisorAccess.REST.Services
{
	public class ChannelAdvisorServicesFactory : IChannelAdvisorServicesFactory
	{
		private readonly string _apiUrl;
		private readonly RestAPICredentials _RestCredentials;
		private readonly OrderService.APICredentials _SoapCredentials;

		public ChannelAdvisorServicesFactory( string apiUrl, string applicationID, string sharedSecret, string accessToken, string refreshToken )
		{
			_apiUrl = apiUrl;
			_RestCredentials = new RestAPICredentials( applicationID, sharedSecret, accessToken, refreshToken );
		}

		public ChannelAdvisorServicesFactory( string apiUrl, string applicationID, string developerKey, string developerPassword)
		{
			_apiUrl = apiUrl;
			
			_SoapCredentials = new OrderService.APICredentials() {  DeveloperKey = developerKey, Password = developerPassword };
		}

		public IAdminService CreateAdminService()
		{
			throw new NotImplementedException();
		}

		public IItemsService CreateItemsService(string accountName, string accountId)
		{
			return new Items.ItemsService( _apiUrl, _RestCredentials );
		}

		public IListingService CreateListingService(string accountName, string accountId)
		{
			throw new NotImplementedException();
		}

		public IOrdersService CreateOrdersService(string accountName, string accountId)
		{
			SetSecurityProtocol();

			// SOAP compatible authorization flow
			if (_SoapCredentials != null)
			{
				return new Orders.OrdersService( _SoapCredentials, accountId, _apiUrl);
			}
			else
				return new Orders.OrdersService( _apiUrl, _RestCredentials );
		}

		public IShippingService CreateShippingService(string accountName, string accountId)
		{
			throw new NotImplementedException();
		}

		#region SSL certificate hack
		public static void SetSecurityProtocol()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
		}

		IAdminService IChannelAdvisorServicesFactory.CreateAdminService()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
