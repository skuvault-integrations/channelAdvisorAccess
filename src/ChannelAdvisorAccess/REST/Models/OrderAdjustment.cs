using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	public class OrderAdjustment
	{
		/// <summary>
		/// Uniquely identifies the adjustment within the ChannelAdvisor account
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// Uniquely identifies the ChannelAdvisor account.
		/// </summary>
		public int ProfileID { get; set; }
		/// <summary>
		/// Uniquely identifies the order within the ChannelAdvisor account
		/// </summary>
		public int OrderID { get; set; }
		/// <summary>
		/// Indicates the adjusted quantity should be added back to available quantity
		/// </summary>
		public bool IsRestock { get; set; }
		/// <summary>
		/// Indicates why the adjustment was requested.
		/// </summary>
		public AdjustmentReason Reason { get; set; }
		/// <summary>
		/// The amount returned from item costs
		/// </summary>
		public decimal ItemAdjustment { get; set; }
		/// <summary>
		/// The amount returned from item tax costs
		/// </summary>
		public decimal TaxAdjustment { get; set; }
		/// <summary>
		/// The amount returned from shipping costs
		/// </summary>
		public decimal ShippingAdjustment { get; set; }
		/// <summary>
		/// The amount returned from shipping tax costs
		/// </summary>
		public decimal ShippingTaxAdjustment { get; set; }
		/// <summary>
		/// The amount returned from gift wrap costs
		/// </summary>
		public decimal GiftWrapAdjustment { get; set; }
		/// <summary>
		/// The amount returned from gift wrap tax costs
		/// </summary>
		public decimal GiftWrapTaxAdjustment { get; set; }
		/// <summary>
		/// The amount returned from recycling and waste fees
		/// </summary>
		public decimal RecyclingFeeAdjustment { get; set; }
		/// <summary>
		/// Indicates which kind of adjustment was requested
		/// </summary>
		public AdjustmentType Type { get; set; }
		/// <summary>
		/// Identifier provided by the seller
		/// </summary>
		public string SellerAdjustmentID { get; set; }
		/// <summary>
		/// Identifier provided by the origin of the order
		/// </summary>
		public string SiteAdjustmentID { get; set; }
		/// <summary>
		/// 	RMA Number for Buyer Initiated Returns
		/// </summary>
		public string RmaNumber { get; set; }
		/// <summary>
		/// Notes on the adjustment
		/// </summary>
		public string Comment { get; set; }
		/// <summary>
		/// Timestamp when the adjustment was created
		/// </summary>
		public DateTime CreatedDateUtc { get; set; }
		/// <summary>
		/// The progress of the adjustment
		/// </summary>
		public AdjustmentRequestStatus RequestStatus { get; set; }
		/// <summary>
		/// The progress of the restock operation
		/// </summary>
		public AdjustmentRequestStatus RestockStatus { get; set; }
		/// <summary>
		/// Fee charged to return items
		/// </summary>
		public decimal ReturnShippingFee { get; set; }
		/// <summary>
		/// Fee charged to restock items
		/// </summary>
		public decimal RestockingFee { get; set; }
		/// <summary>
		/// Tracking number or tracking URL of the return shipment
		/// </summary>
		public string ReturnTrackingNumberOrUrl { get; set; }
		/// <summary>
		/// Shipping method of the return shipment
		/// </summary>
		public string ReturnShippingMethod { get; set; }
		/// <summary>
		/// Reference to the order to which the adjustment belongs
		/// </summary>
		public Order Order { get; set; }
	}

	public class OrderItemAdjustment
	{
		/// <summary>
		/// Uniquely identifies the order item adjustment within the ChannelAdvisor system
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// Uniquely identifies the ChannelAdvisor account
		/// </summary>
		public int ProfileID { get; set; }
		/// <summary>
		/// Uniquely identifies the order within the ChannelAdvisor account
		/// </summary>
		public int OrderID { get; set; }
		/// <summary>
		/// Uniquely identifies the order item within the ChannelAdvisor system
		/// </summary>
		public int OrderItemID { get; set; }
		/// <summary>
		/// Then number of units being adjusted
		/// </summary>
		public int Quantity { get; set; }
		/// <summary>
		/// Indicates the adjusted quantity should be added back to available quantity
		/// </summary>
		public bool IsRestock { get; set; }
		/// <summary>
		/// Indicates why the adjustment was requested.
		/// </summary>
		public AdjustmentReason Reason { get; set; }
		/// <summary>
		/// The amount returned from item costs
		/// </summary>
		public decimal ItemAdjustment { get; set; }
		/// <summary>
		/// The amount returned from item tax costs
		/// </summary>
		public decimal TaxAdjustment { get; set; }
		/// <summary>
		/// The amount returned from shipping costs
		/// </summary>
		public decimal ShippingAdjustment { get; set; }
		/// <summary>
		/// The amount returned from shipping tax costs
		/// </summary>
		public decimal ShippingTaxAdjustment { get; set; }
		/// <summary>
		/// The amount returned from gift wrap costs
		/// </summary>
		public decimal GiftWrapAdjusment { get; set; }
		/// <summary>
		/// The amount returned from gift wrap tax costs
		/// </summary>
		public decimal GiftWrapTaxAdjustment { get; set; }
		/// <summary>
		/// The amount returned from recycling and waste fees
		/// </summary>
		public decimal RecyclingFeeAdjustment { get; set; }
		/// <summary>
		/// Indicates which kind of adjustment was requested
		/// </summary>
		public AdjustmentType Type { get; set; }
		/// <summary>
		/// Identifier provided by the seller
		/// </summary>
		public string SellerAdjustmentID { get; set; }
		/// <summary>
		/// Identifier provided by the origin of the order
		/// </summary>
		public string SiteAdjustmentID { get; set; }
		/// <summary>
		/// RMA Number for Buyer Initiated Returns
		/// </summary>
		public string RmaNumber { get; set; }
		/// <summary>
		/// Notes on the adjustment
		/// </summary>
		public string Comment { get; set; }
		/// <summary>
		/// Timestamp when the adjustment was created
		/// </summary>
		public DateTime CreatedDateUtc { get; set; }
		/// <summary>
		/// The progress of the adjustment
		/// </summary>
		public AdjustmentRequestStatus RequestStatus { get; set; }
		/// <summary>
		/// The progress of the restock operation
		/// </summary>
		public AdjustmentRequestStatus RestockStatus { get; set; }
		/// <summary>
		/// Fee charged to return the item
		/// </summary>
		public decimal ReturnShippingFee { get; set; }
		/// <summary>
		/// Fee charged to restock the item
		/// </summary>
		public decimal RestockingFee { get; set; }
		/// <summary>
		/// Tracking number or tracking URL of the return shipment
		/// </summary>
		public string ReturnTrackingNumberOrUrl { get; set; }
		/// <summary>
		/// Shipping method of the return shipment
		/// </summary>
		public string ReturnShippingMethod { get; set; }
		/// <summary>
		/// 	Reference to the order item to which the adjustment belongs
		/// </summary>
		public OrderItem OrderItem { get; set; }
	}
}
