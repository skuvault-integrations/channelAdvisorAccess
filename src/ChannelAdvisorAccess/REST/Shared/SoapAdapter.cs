using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.REST.Models;
using System.Collections.Generic;
using System.Linq;
using ChannelAdvisorAccess.Misc;
using SoapOrderService = ChannelAdvisorAccess.OrderService;

namespace ChannelAdvisorAccess.REST.Shared
{
	public static class SoapAdapter
	{
		/// <summary>
		///	Converts REST order response to SOAP response
		/// </summary>
		/// <param name="order"></param>
		/// <param name="distributionCenters"></param>
		/// <returns></returns>
		public static OrderResponseDetailComplete ToOrderResponseDetailComplete( this Models.Order order, DistributionCenter[] distributionCenters )
		{
			var response = new OrderResponseDetailComplete()
			{
				OrderID = order.ID,
				ClientOrderIdentifier = order.SiteOrderID,
				SellerOrderID = order.SellerOrderID,
				OrderTimeGMT = order.CreatedDateUtc,
				TransactionNotes = order.PrivateNotes,
				ShippingInfo = new ShippingInfoResponse()
				{
					ShippingInstructions = order.SpecialInstructions,
					DeliveryDate = order.DeliverByDateUtc,
					EstimatedShipDate = order.EstimatedShipDateUtc,
					Title = order.ShippingTitle,
					FirstName = order.ShippingFirstName,
					LastName = order.ShippingLastName,
					AddressLine1 = order.ShippingAddressLine1,
					AddressLine2 = order.ShippingAddressLine2,
					City = order.ShippingCity,
					CompanyName = order.ShippingCompanyName,
					CountryCode = order.ShippingCountry,
					JobTitle = order.ShippingCompanyJobTitle,
					PhoneNumberDay = order.ShippingDaytimePhone,
					PhoneNumberEvening = order.ShippingEveningPhone,
					PostalCode = order.ShippingPostalCode,
					Region = order.ShippingStateOrProvince,
					RegionDescription = order.ShippingStateOrProvinceName,
					Suffix = order.ShippingSuffix,
					ShipmentList = order.Fulfillments.ToShipmentList()
				},
				ShoppingCart = new OrderCart()
				{
					CheckoutSource = order.CheckoutSourceID,
					VATGiftWrapOption = order.GiftOptionsTaxType.ToString(),
					VATShippingOption = order.ShippingTaxType.ToString(),
					VATTaxCalculationOption = order.OrderTaxType.ToString(),
					LineItemInvoiceList = OrderExtensions.ToLineItemInvoiceList( order.TotalTaxPrice, order.TotalShippingPrice ),
					LineItemPromoList = order.AdditionalCostOrDiscount.ToLineItemPromoList()
				},
				BillingInfo = new BillingInfo()
				{
					Title = order.BillingTitle,
					AddressLine1 = order.BillingAddressLine1,
					AddressLine2 = order.BillingAddressLine2,
					City = order.BillingCity,
					CompanyName = order.BillingCompanyName,
					CountryCode = order.BillingCountry,
					FirstName = order.BillingFirstName,
					LastName = order.BillingLastName,
					JobTitle = order.BillingCompanyJobTitle,
					PhoneNumberDay = order.BillingDaytimePhone,
					PhoneNumberEvening = order.BillingEveningPhone,
					PostalCode = order.BillingPostalCode,
					Region = order.BillingStateOrProvince,
					RegionDescription = order.BillingStateOrProvinceName,
					Suffix = order.BillingSuffix
				},
				PaymentInfo = new PaymentInfoResponse()
				{
					 PaymentType = order.PaymentMethod,
					 PaymentTransactionID = order.PaymentTransactionID,
					 PayPalID = order.PaymentPaypalAccountID,
					 CreditCardLast4 = order.PaymentCreditCardLast4,
					 MerchantReferenceNumber = order.PaymentMerchantReferenceNumber
				},
				OrderStatus = new OrderStatus()
				{
					CheckoutDateGMT = order.CheckoutDateUtc.GetValueOrDefault(),
					CheckoutStatus = order.CheckoutStatus.ToString(),
					PaymentDateGMT = order.PaymentDateUtc.GetValueOrDefault(),
					PaymentStatus = order.PaymentStatus.ToString(),
					ShippingDateGMT = order.ShippingDateUtc.GetValueOrDefault(),
					ShippingStatus = order.ShippingStatus.ToString()
				},
				FlagStyle = order.FlagID.ToString(),
				FlagDescription = order.FlagDescription,
				TotalOrderAmount = order.TotalPrice,
				ResellerID = order.ResellerID,
				BuyerEmailAddress = order.BuyerEmailAddress,
				EmailOptIn = order.BuyerEmailOptIn
			};

			List< OrderLineItemItem > shoppingCartItems = new List< OrderLineItemItem >();

			foreach( var item in order.Items )
			{
				var orderLineItem = new OrderLineItemItemResponse()
				{
					 LineItemID = item.ID,
					 SalesSourceID = item.SiteOrderItemID,
					 TaxCost = item.TaxPrice,
					 ShippingCost = item.ShippingPrice,
					 ShippingTaxCost = item.ShippingTaxPrice,
					 RecyclingFee = item.RecyclingFee,
					 GiftMessage = item.GiftMessage,
					 GiftWrapLevel = item.GiftNotes,
					 GiftWrapCost = item.GiftPrice,
					 GiftWrapTaxCost = item.GiftTaxPrice,
					 SKU = item.SKU,
					 UnitPrice = item.UnitPrice.Value,
					 Title = item.Title,
					 Quantity = item.Quantity,
				};
				
				var itemFulFillment = order.Fulfillments.FirstOrDefault( f => f.Items.FirstOrDefault( fi => fi.OrderItemID == item.ID ) != null );

				if ( itemFulFillment != null )
				{
					var distributionCenter = distributionCenters.FirstOrDefault( dc => dc.ID == itemFulFillment.DistributionCenterID.Value );
					orderLineItem.DistributionCenterCode = distributionCenter.Code;
				}
				
				List< OrderLineItemItemPromo > promotions = new List< OrderLineItemItemPromo >();

				foreach( var promotion in item.Promotions )
				{
					promotions.Add( new OrderLineItemItemPromo()
					{
						PromoCode = promotion.Code,
						UnitPrice = promotion.Amount,
						ShippingPrice = promotion.ShippingAmount
					} );
				}

				orderLineItem.ItemPromoList = promotions.ToArray();

				shoppingCartItems.Add( orderLineItem );
			}

			// adjustments now in the separate structure
			foreach( var adjustment in order.Adjustments )
			{
				response.OrderStatus.OrderRefundStatus = adjustment.RequestStatus.ToString();
			}

			// set order last update timestamp
			foreach( var fullfillment in order.Fulfillments )
			{
				if (response.LastUpdateDate == null || response.LastUpdateDate < fullfillment.UpdatedDateUtc)
					response.LastUpdateDate = fullfillment.UpdatedDateUtc;
			}

			// check order state based on all fulfillments status
			if ( order.Fulfillments.All( fulfillment => fulfillment.DeliveryStatus == Models.FulfillmentDeliveryStatus.Canceled ) )
				response.OrderState = "Cancelled";
				
			response.ShoppingCart.LineItemSKUList = shoppingCartItems.ToArray();
			//Return the REST SiteID (marketplace) via the SOAP equivalent field
			if( response.ShoppingCart.LineItemSKUList.Any() )
				response.ShoppingCart.LineItemSKUList.First().ItemSaleSource = order.SiteID.ToString();

			List< CustomValue > customValues = new List< CustomValue >();

			foreach( var field in order.CustomFields )
			{
				customValues.Add( new CustomValue()
				{
					ID = field.FieldID,
					Value = field.Value
				} );
			}

			response.CustomValueList = customValues.ToArray();

			return response;
		}

		/// <summary>
		///	Converts REST distribution center to SOAP response
		/// </summary>
		/// <param name="distributionCenter"></param>
		/// <returns></returns>
		public static DistributionCenterResponse ToDistributionCenterResponse( this Models.DistributionCenter distributionCenter )
		{
			var response = new DistributionCenterResponse()
			{
				 Address1 = distributionCenter.Address1,
				 Address2 = distributionCenter.Address2,
				 City = distributionCenter.City,
				 ContactEmail = distributionCenter.ContactEmail,
				 ContactName = distributionCenter.ContactName,
				 ContactPhone = distributionCenter.ContactPhone,
				 CountryName = distributionCenter.Country,
				 DistributionCenterCode = distributionCenter.Code,
				 DistributionCenterName = distributionCenter.Name,
				 DistributionCenterType = distributionCenter.Type.ToString(),
				 IsPickupLocation = distributionCenter.PickupLocation,
				 IsExternallyManaged = distributionCenter.IsExternallyManaged,
				 IsShipLocation = distributionCenter.ShipLocation,
				 PostalCode = distributionCenter.PostalCode,
				 RegionName = distributionCenter.StateOrProvince
			};

			return response;
		}

		/// <summary>
		///	Converts REST product object to SOAP response
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		public static QuantityInfoResponse ToQuantityInfoResponse( this Models.Product product )
		{
			QuantityInfoResponse response = new QuantityInfoResponse()
			{
				 Available = product.TotalAvailableQuantity,
				 OpenAllocated = (int)product.OpenAllocatedQuantity,
				 OpenAllocatedPooled = (int)product.OpenAllocatedQuantityPooled,
				 PendingCheckout = (int)product.PendingCheckoutQuantity,
				 PendingCheckoutPooled = (int)product.PendingCheckoutQuantityPooled,
				 PendingPayment = (int)product.PendingPaymentQuantity,
				 PendingPaymentPooled = (int)product.PendingPaymentQuantityPooled,
				 PendingShipment = (int)product.PendingShipmentQuantity,
				 PendingShipmentPooled = (int)product.PendingShipmentQuantityPooled,
				 Total = (int)product.TotalQuantity,
				 TotalPooled = (int)product.TotalQuantityPooled
			};

			return response;
		}
		
		/// <summary>
		///	Converts REST product object to SOAP response
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		public static InventoryItemResponse ToInventoryItemResponse( this Models.Product product )
		{
			InventoryItemResponse response = new InventoryItemResponse()
			{
				 ASIN = product.ASIN,
				 BlockComment = product.BlockComment,
				 IsBlocked = product.IsBlocked,
				 Brand = product.Brand,
				 Classification = product.Classification,
				 Condition = product.Condition,
				 Description = product.Description,
				 EAN = product.EAN,
				 FlagDescription = product.FlagDescription,
				 FlagStyle = product.Flag.ToString(),
				 HarmonizedCode = product.HarmonizedCode,
				 Height = product.Height,
				 Width = product.Width,
				 Length = product.Length,
				 ISBN = product.ISBN,
				 Manufacturer = product.Manufacturer,
				 MPN = product.MPN,
				 Weight = product.Weight,
				 Sku = product.Sku,
				 Quantity = product.ToQuantityInfoResponse(),
				 Warranty = product.Warrantly,
				 WarehouseLocation = product.WarehouseLocation,
				 UPC = product.UPC,
				 Title = product.Title,
				 TaxProductCode = product.TaxProductCode,
				 SupplierPO = product.SupplierPO,
				 SupplierCode = product.SupplierCode,
				 Subtitle = product.Subtitle,
				 ShortDescription = product.ShortDescription,
				 ProductMargin = product.Margin,
				 PriceInfo = new PriceInfo()
				 {
					 Cost = product.Cost,
					 ReservePrice = product.ReservePrice,
					 RetailPrice = product.RetailPrice,
					 SecondChanceOfferPrice = product.SecondChancePrice,
					 StartingPrice = product.StartingPrice,
					 StorePrice = product.StorePrice
				 }
			};

			return response;
		}

		public static Shipment [] ToShipmentList( this Fulfillment [] fulfillments )
		{
			if ( fulfillments == null )
				return null;

			return fulfillments.Select( o => new Shipment
			{
				ShippingCarrier = o.ShippingCarrier,
				ShippingClass = o.ShippingClass,
				TrackingNumber = o.TrackingNumber
			} ).ToArray();
		}

		/// <summary>
		/// Convert the REST orderCriteria passed from v1 to SOAP
		/// </summary>
		/// <param name="orderCriteria"></param>
		/// <returns></returns>
		public static SoapOrderService.OrderCriteria ToSoapOrderCriteria( this Models.OrderCriteria orderCriteria )
		{
			return new SoapOrderService.OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = orderCriteria.StatusUpdateFilterBeginTimeGMT,
				StatusUpdateFilterEndTimeGMT = orderCriteria.StatusUpdateFilterEndTimeGMT,
				OrderIDList = orderCriteria.OrderIDList,
				DetailLevel = orderCriteria.DetailLevel
			};
		}
	}
}
