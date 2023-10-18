using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	public class Product
	{
		/// <summary>
		/// Unique identifier of the product within the ChannelAdvisor account.
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// Identifies the ChannelAdvisor account.
		/// </summary>
		public int ProfileID { get; set; }
		/// <summary>
		/// The date the product was created in the ChannelAdvisor system.
		/// </summary>
		public DateTime CreateDateUtc { get; set; }
		/// <summary>
		/// Whether or not the product is in a parent/child relationship. Parents and children will be true, standalone products will be false.
		/// </summary>
		public bool IsInRelationship { get; set; }
		/// <summary>
		/// True if the product is a parent with children.
		/// </summary>
		public bool IsParent { get; set; }
		/// <summary>
		/// If the product is in a relationship, this value represents the relationship type.  The types are user-defined within ChannelAdvisor.
		/// </summary>
		public string RelationshipName { get; set; }
		/// <summary>
		/// If the product is a child, this will contain the ID of the parent product.
		/// </summary>
		public int? ParentProductID { get; set; }
		/// <summary>
		/// Ignore. Deprecated Field.
		/// </summary>
		public bool IsAvailableInStore { get; set; }
		/// <summary>
		/// A blocked product will not list on a marketplace
		/// </summary>
		public bool IsBlocked { get; set; }
		/// <summary>
		/// If true, will prevent listing and fulfillment from Externally-Managed DCs.
		/// </summary>
		public bool IsExternalQuantityBlocked { get; set; }
		/// <summary>
		/// Comment field associated with IsBlocked property.
		/// </summary>
		public string BlockComment { get; set; }
		public DateTime? BlockedDateUtc { get; set; }
		/// <summary>
		/// The date the product was received in inventory.
		/// </summary>
		public DateTime? ReceivedDateUtc { get; set; }
		/// <summary>
		/// The date the product was last sold.
		/// </summary>
		public DateTime? LastSaleDateUtc { get; set; }
		/// <summary>
		/// The date any quantity or data field of the product was last updated.
		/// </summary>
		public DateTime? UpdateDateUtc { get; set; }
		/// <summary>
		/// The date any quantity of the product was last updated.
		/// </summary>
		public DateTime? QuantityUpdateDateUtc { get; set; }
		/// <summary>
		/// Amazon Standard Identification Number
		/// </summary>
		public string ASIN { get; set; }
		public string Brand { get; set; }
		/// <summary>
		/// "New", "Used", "Refurbished", "Reconditioned", or "Like New" are the only valid values for this field.
		/// </summary>
		public string Condition { get; set; }
		public string Description { get; set; }
		/// <summary>
		/// European Article Number (Now called International Article Number)
		/// </summary>
		public string EAN { get; set; }
		/// <summary>
		/// Provides a short description of the flag associated with this product.
		/// </summary>
		public string FlagDescription { get; set; }
		/// <summary>
		/// Sets the flag style on a product
		/// </summary>
		public FlagType Flag { get; set; }
		public string HarmonizedCode { get; set; }
		/// <summary>
		/// 	International Standard Book Number
		/// </summary>
		public string ISBN { get; set; }
		public string Manufacturer { get; set; }
		/// <summary>
		/// Manufacturer Part Number
		/// </summary>
		public string MPN { get; set; }
		public string ShortDescription { get; set; }
		public string Sku { get; set; }
		public string Subtitle { get; set; }
		public string TaxProductCode { get; set; }
		public string Title { get; set; }
		/// <summary>
		/// Universal Product Code
		/// </summary>
		public string UPC { get; set; }
		public string WarehouseLocation { get; set; }
		public string Warrantly { get; set; }
		/// <summary>
		/// Default unit in US profiles is "Inches". All other locales are "Centimeters".
		/// </summary>
		public decimal? Height { get; set; }
		/// <summary>
		/// Default unit in US profiles is "Inches". All other locales are "Centimeters".
		/// </summary>
		public decimal? Length { get; set; }
		/// <summary>
		/// Default unit in US profiles is "Inches". All other locales are "Centimeters".
		/// </summary>
		public decimal? Width { get; set; }
		/// <summary>
		/// Default unit in US profiles is "Pounds". All other locales are "Kilograms".
		/// </summary>
		public decimal? Weight { get; set; }
		/// <summary>
		/// The price that the seller paid for this item.
		/// </summary>
		public decimal? Cost { get; set; }
		/// <summary>
		/// Profit margin for a product
		/// </summary>
		public decimal? Margin { get; set; }
		/// <summary>
		/// Retail price for this item
		/// </summary>
		public decimal? RetailPrice { get; set; }
		/// <summary>
		/// For an eBay listing, the initial bid starting point.
		/// </summary>
		public decimal? StartingPrice { get; set; }
		/// <summary>
		/// For an eBay listing, the minimum price for an auction to sell.
		/// </summary>
		public decimal? ReservePrice { get; set; }
		/// <summary>
		/// Selling price of a product
		/// </summary>
		public decimal? BuyItNowPrice { get; set; }
		public decimal? StorePrice { get; set; }
		/// <summary>
		/// Price above which to offer underbidders of this item a second chance offer.
		/// </summary>
		public decimal? SecondChancePrice { get; set; }
		/// <summary>
		/// Minimum price of a product.
		/// </summary>
		public decimal? MinPrice { get; set; }
		/// <summary>
		/// Maximum price of a product.
		/// </summary>
		public decimal? MaxPrice { get; set; }
		/// <summary>
		/// The name of the supplier for this item
		/// </summary>
		public string SupplierName { get; set; }
		/// <summary>
		/// Code for the supplier of this item (must be created in ChannelAdvisor prior to use)
		/// </summary>
		public string SupplierCode { get; set; }
		/// <summary>
		/// Purchase Order associated with this supplier
		/// </summary>
		public string SupplierPO { get; set; }
		/// <summary>
		/// The inventory classification to assign to this item.
		/// </summary>
		public string Classification { get; set; }
		public bool? IsDisplayInStore { get; set; }
		public string StoreTitle { get; set; }
		public string StoreDescription { get; set; }
		public BundleType BundleType { get; set; }
		public int? TotalAvailableQuantity { get; set; }
		public long OpenAllocatedQuantity { get; set; }
		public long OpenAllocatedQuantityPooled { get; set; }
		public long PendingCheckoutQuantity { get; set; }
		public long PendingCheckoutQuantityPooled { get; set; }
		public long PendingPaymentQuantity { get; set; }
		public long PendingPaymentQuantityPooled { get; set; }
		public long PendingShipmentQuantity { get; set; }
		public long PendingShipmentQuantityPooled { get; set; }
		public long TotalQuantity { get; set; }
		public long TotalQuantityPooled { get; set; }
		public int? MultipackQuantity { get; set; }
		public AttributeValue[] Attributes { get; set; }
		public DCQuantity[] DCQuantities { get; set; }
		public Image[] Images { get; set; }
		public ProductLabel[] Labels { get; set; }
		public ProductBundleComponent[] BundleComponents { get; set; }
		public ChildRelationship[] Children { get; set; }
	}

	public enum BundleType
	{
		None = 0,
		BundleComponent = 1,
		BundleItem = 2,
		AssemblyBundleItem = 3
	}

	public class AttributeValue
	{
		public int ProductID { get; set; }
		public int ProfileID { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		public Product Product { get; set; }
	}

	public class DCQuantity
	{
		public int ProductID { get; set; }
		public int ProfileID { get; set; }
		public int DistributionCenterID { get; set; }
		public int AvailableQuantity { get; set; }
		public Product Product { get; set; }
	}

	public class Image
	{
		public int ProductID { get; set; }
		public int ProfileID { get; set; }
		public string PlacementName { get; set; }
		public string Abbreviation { get; set; }
		public string Url { get; set; }
		public Product Product { get; set; }
	}

	public class ProductLabel
	{
		public int ProductID { get; set; }
		public int ProfileID { get; set; }
		/// <summary>
		/// Name of the label to be applied to this product
		/// </summary>
		public string Name { get; set; }
		public Product Product { get; set; }
	}

	public class ProductBundleComponent
	{
		public int ProductID { get; set; }
		public int ComponentID { get; set; }
		public int ProfileID { get; set; }
		public string ComponentSku { get; set; }
		public int Quantity { get; set; }
		public Product Product { get; set; }
	}

	public class ChildRelationship
	{
		public int ParentProductID { get; set; }
		public int ProfileID { get; set; }
		public int ChildProductID { get; set; }
		public Product ChildProduct { get; set; }
	}
}
