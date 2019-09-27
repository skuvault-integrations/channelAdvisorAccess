using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	/// <summary>
	/// When a purchased item is a bundle, the bundle component data exists as a property within the OrderItem and includes basic SKU and quantity information of the products that comprise the bundle
	/// </summary>
	public class OrderItemBundleComponent
	{
		/// <summary>
		/// Uniquely identifies the order item within the ChannelAdvisor account
		/// </summary>
		public int OrderItemID { get; set; }
		/// <summary>
		/// Uniquely identifies the ChannelAdvisor account
		/// </summary>
		public int ProfileID { get; set; }
		/// <summary>
		/// Uniquely identifies the order within the ChannelAdvisor account
		/// </summary>
		public int OrderID { get; set; }
		/// <summary>
		/// Uniquely identifies the product within the ChannelAdvisor account.
		/// </summary>
		public int ProductID { get; set; }
		/// <summary>
		/// Uniquely identifies the bundle to which the component belongs within the ChannelAdvisor account
		/// </summary>
		public int BundleProductID { get; set; }
		/// <summary>
		/// Identifier for the component product provided by the seller
		/// </summary>
		public string Sku { get; set; }
		/// <summary>
		/// Identifier for the bundle product provided by the seller
		/// </summary>
		public string BundleSku { get; set; }
		/// <summary>
		/// Title of the component
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// The number of units ordered (value is provided in the order already multiplied by OrderItem Quantity for the Bundle Sku)
		/// </summary>
		public int Quantity { get; set; }
		/// <summary>
		/// Reference to the order item bundle to which the component belongs
		/// </summary>
		public OrderItem OrderItem { get; set; }
		/// <summary>
		/// The fulfilled items related to the component
		/// </summary>
		public FulfillmentItem[] FulfillmentItems { get; set; }
	}
}
