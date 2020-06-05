using NUnit.Framework;
using ChannelAdvisorAccess.Misc;
using FluentAssertions;

namespace ChannelAdvisorAccessTests.Misc
{
	[ TestFixture ]
	public class OrderExtensionsTests
	{
		[ Test ]
		public static void ToLineItemInvoiceList()
		{
			decimal? totalTax = 1.2m;
			decimal totalShipping = 2.3m;

			var lineItemInvoiceList = OrderExtensions.ToLineItemInvoiceList( totalTax, totalShipping );

			lineItemInvoiceList[ 0 ].UnitPrice.Should().Be( totalTax );
			lineItemInvoiceList[ 0 ].LineItemType.Should().Be( "SalesTax" );
			lineItemInvoiceList[ 1 ].UnitPrice.Should().Be( totalShipping );
			lineItemInvoiceList[ 1 ].LineItemType.Should().Be( "Shipping" );
		}

		[ Test ]
		public static void ToLineItemPromoList()
		{
			decimal? additionalCostOrDiscount = 2.3m;

			var lineItemPromoList = additionalCostOrDiscount.ToLineItemPromoList();

			lineItemPromoList[ 0 ].UnitPrice.Should().Be( additionalCostOrDiscount );
		}
	}
}
