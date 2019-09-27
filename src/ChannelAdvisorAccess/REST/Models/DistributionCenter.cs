using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	public class DistributionCenter
	{
		/// <summary>
		/// Distribution Center ID, which uniquely identifies a distribution center
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// Name of the distribution center, which can be changed
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Identifies the distribution center and cannot be changed
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// Name of the fulfillment partner who manages the distribution center, if applicable. Otherwise, "None"
		/// </summary>
		public string FulfillmentPartnerName { get; set; }
		/// <summary>
		/// Name of the point of contact for the distribution center
		/// </summary>
		public string ContactName { get; set; }
		/// <summary>
		/// Email address of the point of contact
		/// </summary>
		public string ContactEmail { get; set; }
		/// <summary>
		/// Phone number of the point of contact
		/// </summary>
		public string ContactPhone { get; set; }
		/// <summary>
		/// Line 1 of the address of the distribution center
		/// </summary>
		public string Address1 { get; set; }
		/// <summary>
		/// Line 2 of the address of the distribution center
		/// </summary>
		public string Address2 { get; set; }
		/// <summary>
		/// City of the distribution center
		/// </summary>
		public string City { get; set; }
		/// <summary>
		/// Region of the distribution center
		/// </summary>
		public string StateOrProvince { get; set; }
		/// <summary>
		/// Country of the distribution center
		/// </summary>
		public string Country { get; set; }
		/// <summary>
		/// Postal code of the distribution center
		/// </summary>
		public string PostalCode { get; set; }
		/// <summary>
		/// Indicates if the distribution center allows buyer pickup
		/// </summary>
		public bool PickupLocation { get; set; }
		/// <summary>
		/// Indicates if the distribution center is a shipping location
		/// </summary>
		public bool ShipLocation { get; set; }
		/// <summary>
		/// Type of distribution center
		/// </summary>
		public DistributionCenterType Type { get; set; }
		/// <summary>
		/// Indicates if the distribution center is externally managed, e.g. Fulfillment By Amazon
		/// </summary>
		public bool IsExternallyManaged { get; set; }
		/// <summary>
		/// Indicates if the distribution center has been deleted from the user interface
		/// </summary>
		public bool IsDeleted { get; set; }
		/// <summary>
		/// Date the distribution center was deleted from the user interface
		/// </summary>
		public DateTime? DeletedDateUtc { get; set; }
	}

	public enum DistributionCenterType
	{
		Warehouse = 0,
		ExternallyManaged = 1,
		DropShip = 2,
		RetailStore = 3
	}
}
