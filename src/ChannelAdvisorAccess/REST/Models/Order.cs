using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	/// <summary>
	/// The details of an order
	/// </summary>
	public class Order
	{
		/// <summary>
		/// Uniquely identifies the order within the ChannelAdvisor account 
		/// (note: this is not the marketplace Order ID).
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// Identifies the ChannelAdvisor account.
		/// </summary>
		public int ProfileID { get; set; }
		/// <summary>
		/// Identifies the origin of the order, such as a marketplace
		/// </summary>
		public int SiteID { get; set; }
		/// <summary>
		/// Name of the origin marketplace or webstore of the order
		/// The default value is 'Checkout Direct'
		/// </summary>
		public string SiteName { get; set; }
		/// <summary>
		/// Identifies the seller account on the marketplace or webstore
		/// </summary>
		public int? SiteAccountID { get; set; }
		/// <summary>
		/// Order identifier provided by the origin of the order. This is usually the marketplace Order ID
		/// </summary>
		public string SiteOrderID { get; set; }
		/// <summary>
		/// 	Secondary identifier provided by the origin of the order. This is a secondary marketplace-generated Order ID. Will not be populated most of the time.
		/// </summary>
		public string SecondarySiteOrderID { get; set; }
		/// <summary>
		/// Identifier provided by the seller
		/// </summary>
		public string SellerOrderID { get; set; }
		/// <summary>
		/// Identifies the checkout system used to place the order
		/// </summary>
		public string CheckoutSourceID { get; set; }
		/// <summary>
		/// Identifies the currency used in the order transaction
		/// </summary>
		public string Currency { get; set; }
		/// <summary>
		/// Timestamp when the order was imported into ChannelAdvisor
		/// </summary>
		public DateTime ImportDateUtc { get; set; }
		/// <summary>
		/// Timestamp when the order was created at the origin of the order
		/// </summary>
		public DateTime CreatedDateUtc { get; set; }
		/// <summary>
		/// Notes from the seller which may be included in a printed invoice or packing slip
		/// </summary>
		public string PublicNotes { get; set; }
		/// <summary>
		/// Notes from the seller which cannot be included in a printed invoice or packing slip
		/// </summary>
		public string PrivateNotes { get; set; }
		/// <summary>
		/// Notes provided by the buyer, usually pertaining to shipping instructions
		/// </summary>
		public string SpecialInstructions { get; set; }
		/// <summary>
		/// Total price paid by the buyer
		/// </summary>
		public decimal TotalPrice { get; set; }
		/// <summary>
		/// Subtotal of all tax costs, including shipping, and gift taxes
		/// </summary>
		public decimal? TotalTaxPrice { get; set; }
		/// <summary>
		/// 	Subtotal of all shipping costs
		/// 	Does not include exclusive tax
		/// 	Does not include shipping item-level shipping promotions
		/// </summary>
		public decimal? TotalShippingPrice { get; set; }
		/// <summary>
		/// Subtotal of all shipping tax costs
		/// </summary>
		public decimal? TotalShippingTaxPrice { get; set; }
		/// <summary>
		/// Subtotal of all insurance costs
		/// </summary>
		public decimal? TotalInsurancePrice { get; set; }
		/// <summary>
		/// Subtotal of all gift option costs. Does not include exclusive tax
		/// </summary>
		public decimal? TotalGiftOptionPrice { get; set; }
		/// <summary>
		/// Subtotal of all gift option tax costs
		/// </summary>
		public decimal? TotalGiftOptionTaxPrice { get; set; }
		/// <summary>
		/// Miscellaneous cost modification which may be positive to indicate a cost or negative to indicate a discount
		/// </summary>
		public decimal? AdditionalCostOrDiscount { get; set; }
		/// <summary>
		/// Timestamp estimating when the order will be fulfilled
		/// </summary>
		public DateTime? EstimatedShipDateUtc { get; set; }
		/// <summary>
		/// Timestamp indicating the deadline for fulfilling the order
		/// </summary>
		public DateTime? DeliverByDateUtc { get; set; }
		/// <summary>
		/// Identifier for a reseller agency
		/// </summary>
		public string ResellerID { get; set; }
		/// <summary>
		/// Identifies the flag type, if any
		/// </summary>
		public FlagType? FlagID { get; set; }
		/// <summary>
		/// Describes the flag
		/// </summary>
		public string FlagDescription { get; set; }
		/// <summary>
		/// Indicates the progress of the order through the checkout process
		/// </summary>
		public OrderCheckoutStatus CheckoutStatus { get; set; }
		/// <summary>
		/// Indicates the progress of the payment
		/// </summary>
		public OrderPaymentStatus PaymentStatus { get; set; }
		/// <summary>
		/// Indicates the progress of the fulfillment on the order
		/// </summary>
		public OrderShippingStatus ShippingStatus { get; set; }
		/// <summary>
		/// Timestamp indicating the latest update to CheckoutStatus
		/// </summary>
		public DateTime? CheckoutDateUtc { get; set; }
		/// <summary>
		/// Timestamp indicating the latest update to PaymentStatus
		/// </summary>
		public DateTime? PaymentDateUtc { get; set; }
		/// <summary>
		/// Timestamp indicating the latest update to ShippingStatus
		/// </summary>
		public DateTime? ShippingDateUtc { get; set; }
		/// <summary>
		/// The UserID of the buyer
		/// </summary>
		public string BuyerUserId { get; set; }
		/// <summary>
		/// The email address of the buyer
		/// </summary>
		public string BuyerEmailAddress { get; set; }
		/// <summary>
		/// Indicates if the buyer wishes to opt in for marketing emails
		/// </summary>
		public bool BuyerEmailOptIn { get; set; }
		/// <summary>
		/// The type of tax applied to the item costs
		/// </summary>
		public OrderTaxType OrderTaxType { get; set; }
		/// <summary>
		/// The type of tax applied to the shipping costs
		/// </summary>
		public OrderTaxType ShippingTaxType { get; set; }
		/// <summary>
		/// The type of tax applied to the gift option costs
		/// </summary>
		public OrderTaxType GiftOptionsTaxType { get; set; }
		/// <summary>
		/// The type of payment submitted by the buyer
		/// </summary>
		public string PaymentMethod { get; set; }
		/// <summary>
		/// The TransactionID of the payment
		/// </summary>
		public string PaymentTransactionID { get; set; }
		/// <summary>
		/// The PayPal AccountID of the buyer
		/// </summary>
		public string PaymentPaypalAccountID { get; set; }
		/// <summary>
		/// The last four digits of the credit card number
		/// </summary>
		public string PaymentCreditCardLast4 { get; set; }
		/// <summary>
		/// 	The reference number of the payment
		/// </summary>
		public string PaymentMerchantReferenceNumber { get; set; }
		/// <summary>
		/// The shipping recipient's title
		/// </summary>
		public string ShippingTitle { get; set; }
		/// <summary>
		/// The shipping recipient's first name
		/// </summary>
		public string ShippingFirstName { get; set; }
		/// <summary>
		/// The shipping recipient's last name
		/// </summary>
		public string ShippingLastName { get; set; }
		/// <summary>
		/// The shipping recipient's name suffix
		/// </summary>
		public string ShippingSuffix { get; set; }
		/// <summary>
		/// The shipping recipient's company name
		/// </summary>
		public string ShippingCompanyName { get; set; }
		/// <summary>
		/// The shipping recipient's job title
		/// </summary>
		public string ShippingCompanyJobTitle { get; set; }
		/// <summary>
		/// The shipping recipient's daytime phone number
		/// </summary>
		public string ShippingDaytimePhone { get; set; }
		/// <summary>
		/// The shipping recipient's evening phone number
		/// </summary>
		public string ShippingEveningPhone { get; set; }
		/// <summary>
		/// The first line of the shipping address
		/// </summary>
		public string ShippingAddressLine1 { get; set; }
		/// <summary>
		/// The second line of the shipping address
		/// </summary>
		public string ShippingAddressLine2 { get; set; }
		/// <summary>
		/// 	The city of the shipping address
		/// </summary>
		public string ShippingCity { get; set; }
		/// <summary>
		/// 	The region of the shipping address
		/// </summary>
		public string ShippingStateOrProvince { get; set; }
		/// <summary>
		/// The full name of the shipping address region
		/// </summary>
		public string ShippingStateOrProvinceName { get; set; }
		/// <summary>
		/// The postal code of the shipping address
		/// </summary>
		public string ShippingPostalCode { get; set; }
		/// <summary>
		/// The country of the shipping address. ISO format.
		/// Required if ShippingStateOrProvince is provided.
		/// </summary>
		public string ShippingCountry { get; set; }
		/// <summary>
		/// 	The payer's title
		/// </summary>
		public string BillingTitle { get; set; }
		/// <summary>
		/// The payer's first name
		/// </summary>
		public string BillingFirstName { get; set; }
		/// <summary>
		/// 	The payer's last name
		/// </summary>
		public string BillingLastName { get; set; }
		/// <summary>
		/// The payer's name suffix
		/// </summary>
		public string BillingSuffix { get; set; }
		/// <summary>
		/// The payer's company name
		/// </summary>
		public string BillingCompanyName { get; set; }
		/// <summary>
		/// The payer's job title
		/// </summary>
		public string BillingCompanyJobTitle { get; set; }
		/// <summary>
		/// The payer's daytime phone
		/// </summary>
		public string BillingDaytimePhone { get; set; }
		/// <summary>
		/// The payer's evening phone.
		/// </summary>
		public string BillingEveningPhone { get; set; }
		/// <summary>
		/// 	The first line of the billing address
		/// </summary>
		public string BillingAddressLine1 { get; set; }
		/// <summary>
		/// The second line of the billing address
		/// </summary>
		public string BillingAddressLine2 { get; set; }
		/// <summary>
		/// The city of the billing address
		/// </summary>
		public string BillingCity { get; set; }
		/// <summary>
		/// The region of the billing address
		/// </summary>
		public string BillingStateOrProvince { get; set; }
		/// <summary>
		/// The full name of the billing address region
		/// </summary>
		public string BillingStateOrProvinceName { get; set; }
		/// <summary>
		/// The postal code of the billing address
		/// </summary>
		public string BillingPostalCode { get; set; }
		/// <summary>
		/// The country of the billing address. ISO format.
		/// Required if BillingStateOrProvince is provided.
		/// </summary>
		public string BillingCountry { get; set; }
		/// <summary>
		/// The order-level promotional discount code
		/// </summary>
		public string PromotionCode { get; set; }
		/// <summary>
		/// 	The order-level discount amount
		/// </summary>
		public decimal? PromotionAmount { get; set; }
		/// <summary>
		/// Comma-delimited list of all tags applied to the order.
		/// </summary>
		public string OrderTags { get; set; }
		/// <summary>
		/// The type of distrbution center
		/// </summary>
		public DistributionCenterTypeRollup DistributionCenterTypeRollup { get; set; }
		/// <summary>
		/// The purchased items
		/// </summary>
		public OrderItem[] Items { get; set; }
		/// <summary>
		/// The fulfilled items
		/// </summary>
		public Fulfillment[] Fulfillments { get; set; }
		/// <summary>
		/// The cancellations and refunds on the order
		/// </summary>
		public OrderAdjustment[] Adjustments { get; set; }
		/// <summary>
		/// Additional information pertaining to the order
		/// </summary>
		public CustomField[] CustomFields { get; set; }
	}
	
	public enum OrderCheckoutStatus
	{
		/// <summary>
		/// Checkout is open
		/// </summary>
		NotVisited = 0,
		/// <summary>
		/// Checkout is closed
		/// </summary>
		Completed = 1,
		/// <summary>
		/// Checkout is open
		/// </summary>
		Visited = 2,
		/// <summary>
		/// Checkout is closed
		/// </summary>
		Disabled = 4,
		/// <summary>
		/// Checkout is closed
		/// </summary>
		CompletedAndVisited = 3,
		/// <summary>
		/// Checkout is closed
		/// </summary>
		CompletedOffline = 8,
		/// <summary>
		/// Checkout is open
		/// </summary>
		OnHold = 16
	}

	public enum OrderPaymentStatus
	{
		/// <summary>
		/// Payment is unfinished
		/// </summary>
		NotYetSubmitted = 0,
		/// <summary>
		/// Payment is finished
		/// </summary>
		Cleared = 1,
		/// <summary>
		/// Payment is unfinished
		/// </summary>
		Submitted = 2,
		/// <summary>
		/// Payment is finished
		/// </summary>
		Failed = 4,
		/// <summary>
		/// Payment is unfinished
		/// </summary>
		Deposited = 8
	}

	public enum OrderShippingStatus
	{
		/// <summary>
		/// Fulfillment is incomplete
		/// </summary>
		Unshipped = 0,
		/// <summary>
		/// Fulfillment is complete
		/// </summary>
		Shipped = 1,
		/// <summary>
		/// Fulfillment is incomplete
		/// </summary>
		PartiallyShipped = 2,
		/// <summary>
		/// Fulfillment is incomplete
		/// </summary>
		PendingShipment = 4,
		/// <summary>
		/// Fulfillment is complete
		/// </summary>
		Cancelled = 8,
		/// <summary>
		/// Fulfillment is complete
		/// </summary>
		ThirdPartyManaged = 16
	}

	public enum OrderTaxType
	{
		NoTax = 0,
		/// <summary>
		/// Tax is not included in costs
		/// </summary>
		Standard = 1,
		/// <summary>
		/// Tax is not included in costs
		/// </summary>
		ExclusiveVat = 2,
		/// <summary>
		/// Tax is included in costs
		/// </summary>
		InclusiveVat = 3
	}

	public enum DistributionCenterTypeRollup
	{
		/// <summary>
		/// The Distribution Center associated with products in the order is managed by the seller.
		/// </summary>
		SellerManaged,
		/// <summary>
		/// The Distribution Center associated with products in the order is managed by another party
		/// </summary>
		ExternallyManaged,
		/// <summary>
		/// The Distribution Centers associated with products in the order are both seller managed and externally managed
		/// </summary>
		Mixed
	}
}
