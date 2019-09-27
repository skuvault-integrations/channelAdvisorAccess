using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	/// <summary>
	/// The purchased items belonging to an Order
	/// </summary>
	public class OrderItem
	{
		/// <summary>
		///	Uniquely identifies the order item within the ChannelAdvisor account
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// Identifies the ChannelAdvisor account
		/// </summary>
		public int ProfileID { get; set; }
		/// <summary>
		/// Uniquely identifies the order within the ChannelAdvisor account
		/// </summary>
		public int? OrderID { get; set; }
		/// <summary>
		/// Uniquely identifies the product within the ChannelAdvisor account
		/// </summary>
		public int ProductID { get; set; }
		/// <summary>
		/// Order item identifier provided by the origin of the order
		/// </summary>
		public string SiteOrderItemID { get; set; }
		/// <summary>
		/// Listing identifier provided by the origin of the order
		/// </summary>
		public string SiteListingID { get; set; }
		/// <summary>
		/// Order item identifier provided by the seller
		/// </summary>
		public int? SellerOrderItemID { get; set; }
		/// <summary>
		/// Product identifier provided by the seller
		/// </summary>
		public string SKU { get; set; }
		/// <summary>
		/// Title of the order item
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// The number of units purchased.
		/// </summary>
		public int Quantity { get; set; }
		/// <summary>
		/// The cost per unit
		/// </summary>
		public decimal? UnitPrice { get; set; }
		/// <summary>
		/// The tax cost for all quantity
		/// </summary>
		public decimal TaxPrice { get; set; }
		/// <summary>
		/// The shipping cost for all quantity
		/// </summary>
		public decimal ShippingPrice { get; set; }
		/// <summary>
		/// 	The shipping tax cost for all quantity
		/// </summary>
		public decimal ShippingTaxPrice { get; set; }
		/// <summary>
		/// The recycling and waste fee for all quantity
		/// </summary>
		public decimal RecyclingFee { get; set; }
		/// <summary>
		/// The message to accompany gift wrapping
		/// </summary>
		public string GiftMessage { get; set; }
		/// <summary>
		/// Description of the gift wrapping
		/// </summary>
		public string GiftNotes { get; set; }
		/// <summary>
		/// The gift wrapping cost for all quantity
		/// </summary>
		public decimal? GiftPrice { get; set; }
		/// <summary>
		/// The gift wrapping tax cost for all quantity
		/// </summary>
		public decimal? GiftTaxPrice { get; set; }
		/// <summary>
		/// Indicates if the order item is a bundle
		/// </summary>
		public bool IsBundle { get; set; }
		/// <summary>
		/// The URL of the order item on the origin website
		/// </summary>
		public string ItemURL { get; set; }
		/// <summary>
		/// The harmonized code of the order item
		/// </summary>
		public string HarmonizedCode { get; set; }
		/// <summary>
		/// Discounts applied to the costs of the order item
		/// </summary>
		public Promotion[] Promotions { get; set; }
		/// <summary>
		/// The fulfilled items corresponding to the purchased order item
		/// </summary>
		public FulfillmentItem[] FulfillmentItems { get; set; }
		/// <summary>
		/// The component items, if the order item is a bundle
		/// </summary>
		public OrderItemBundleComponent[] BundleComponents { get; set; }
		/// <summary>
		/// Cancellations and refunds on the order item.
		/// </summary>
		public OrderItemAdjustment[] Adjustments { get; set; }
		/// <summary>
		/// Reference to the order to which the order item belongs.
		/// </summary>
		public Order Order { get; set; }
	}
}
