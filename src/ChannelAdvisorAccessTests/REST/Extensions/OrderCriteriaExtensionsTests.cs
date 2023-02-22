using System;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.REST.Extensions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.REST.Extensions
{
	public class OrderCriteriaExtensionsTests
	{
		[ Test ]
		public void ToRequestFilterString_WhenStatusUpdateFilterSpecified_ReturnsStatusUpdateFilterString()
		{
			var statusUpdateBegin = new DateTime( 2020, 1, 1, 1, 1, 1 );
			var statusUpdateEnd = new DateTime( 2022, 2, 2, 2, 2, 2 );
			var orderCriteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = statusUpdateBegin,
				StatusUpdateFilterEndTimeGMT = statusUpdateEnd
			};
			
			var result = orderCriteria.ToRequestFilterString();
			
			Assert.That( result, Is.EqualTo( $"({OrderCriteriaExtensions.CheckoutDateFieldName} ge {statusUpdateBegin.ToDateTimeOffset()} and {OrderCriteriaExtensions.CheckoutDateFieldName} le {statusUpdateEnd.ToDateTimeOffset()}) or " + 
				$"({OrderCriteriaExtensions.PaymentDateFieldName} ge {statusUpdateBegin.ToDateTimeOffset()} and {OrderCriteriaExtensions.PaymentDateFieldName} le {statusUpdateEnd.ToDateTimeOffset()}) or " + 
				$"({OrderCriteriaExtensions.ShippingDateFieldName} ge {statusUpdateBegin.ToDateTimeOffset()} and {OrderCriteriaExtensions.ShippingDateFieldName} le {statusUpdateEnd.ToDateTimeOffset()}) " ) );
		}
		
		[ Test ]
		public void ToRequestFilterString_WhenOrderIdFiltersSpecified_ReturnsOrderIdFilterString()
		{
			const int orderId = 1234;
			
			var result = orderId.ToRequestFilterString();
			
			Assert.That( result, Is.EqualTo( $"{OrderCriteriaExtensions.OrderIdFieldName} eq {orderId} " ) );
		}
		
		[ Test ]
		public void ToRequestFilterString_WhenImportDateFilterSpecified_ReturnsImportDateFilterString()
		{
			var importDateBegin = new DateTime( 2020, 1, 1, 1, 1, 1 );
			var importDateEnd = new DateTime( 2022, 2, 2, 2, 2, 2 );
			var orderCriteria = new OrderCriteria
			{
				ImportDateFilterBeginTimeGMT = importDateBegin,
				ImportDateFilterEndTimeGMT = importDateEnd
			};
			
			var result = orderCriteria.ToRequestFilterString();
			
			Assert.That( result, Is.EqualTo( $"({OrderCriteriaExtensions.ImportDateFieldName} ge {importDateBegin.ToDateTimeOffset()} and {OrderCriteriaExtensions.ImportDateFieldName} le {importDateEnd.ToDateTimeOffset()}) " ) );
		}
		
		[ Test ]
		public void ToRequestFilterString_WhenImportDateAndStatusUpdateFiltersSpecified_ReturnsFilterStringWithBoth()
		{
			var importDateBegin = new DateTime( 2020, 1, 1, 1, 1, 1 );
			var importDateEnd = new DateTime( 2022, 2, 2, 2, 2, 2 );
			var statusUpdateBegin = new DateTime( 2023, 3, 3, 3, 3, 3 );
			var statusUpdateEnd = new DateTime( 2024, 4, 4, 4, 4, 4 );
			var orderCriteria = new OrderCriteria
			{
				ImportDateFilterBeginTimeGMT = importDateBegin,
				ImportDateFilterEndTimeGMT = importDateEnd,
				StatusUpdateFilterBeginTimeGMT = statusUpdateBegin,
				StatusUpdateFilterEndTimeGMT = statusUpdateEnd
			};
			
			var result = orderCriteria.ToRequestFilterString();
			
			Assert.That( result, Is.EqualTo( $"({OrderCriteriaExtensions.CheckoutDateFieldName} ge {statusUpdateBegin.ToDateTimeOffset()} and {OrderCriteriaExtensions.CheckoutDateFieldName} le {statusUpdateEnd.ToDateTimeOffset()}) or " + 
				$"({OrderCriteriaExtensions.PaymentDateFieldName} ge {statusUpdateBegin.ToDateTimeOffset()} and {OrderCriteriaExtensions.PaymentDateFieldName} le {statusUpdateEnd.ToDateTimeOffset()}) or " + 
				$"({OrderCriteriaExtensions.ShippingDateFieldName} ge {statusUpdateBegin.ToDateTimeOffset()} and {OrderCriteriaExtensions.ShippingDateFieldName} le {statusUpdateEnd.ToDateTimeOffset()}) or " +
				$"({OrderCriteriaExtensions.ImportDateFieldName} ge {importDateBegin.ToDateTimeOffset()} and {OrderCriteriaExtensions.ImportDateFieldName} le {importDateEnd.ToDateTimeOffset()}) " ) );
		}
	}
}