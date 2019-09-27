using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	public class UpdateQuantityRequest
	{
		public string UpdateType { get; set; }
		public UpdateQuantityDC[] Updates { get; set; }
	}

	public class UpdateQuantityDC
	{
		public int DistributionCenterID { get; set; }
		public int Quantity { get; set; }
	}

	public enum QuantityUpdateType
	{
		Absolute = 0,
		Relative = 1,
		Available = 2,
		InStock = 3,
		UnShipped = 4
	}
}
