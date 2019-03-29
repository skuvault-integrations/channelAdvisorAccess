using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models.Configuration
{
	public class ChannelAdvisorEndPoint
	{
		public static readonly string BaseApiUrl = "https://api.channeladvisor.com";
		public static readonly string OrdersUrl = "v1/orders";
		public static readonly string ProductsUrl = "v1/Products";
		public static readonly string DistributionCentersUrl = "v1/DistributionCenters";
	}
}
