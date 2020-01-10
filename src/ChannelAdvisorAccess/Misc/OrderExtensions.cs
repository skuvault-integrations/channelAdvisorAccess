using System.Collections.Generic;
using ChannelAdvisorAccess.OrderService;

namespace ChannelAdvisorAccess.Misc
{
	public static class OrderExtensions
	{
		public static OrderLineItemInvoice[] ToLineItemInvoiceList( this decimal? totalTaxPrice )
		{
			var list = new List< OrderLineItemInvoice >();

			if ( totalTaxPrice != null ) 
			{ 
				list.Add( new OrderLineItemInvoice
					{
						UnitPrice = totalTaxPrice.Value,
						LineItemType = "SalesTax"
					}
				);
			}

			return list.ToArray();
		}

		public static OrderLineItemPromo[] ToLineItemPromoList( this decimal? AdditionalCostOrDiscount )
		{
			var list = new List< OrderLineItemPromo >();

			if ( AdditionalCostOrDiscount != null )
			{
				list.Add( new OrderLineItemPromo
					{
						UnitPrice = AdditionalCostOrDiscount.Value
					}
				);
			}

			return list.ToArray();
		}
	}
}
