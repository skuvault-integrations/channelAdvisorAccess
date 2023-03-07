using System;
using ChannelAdvisorAccess.REST.Extensions;
using ChannelAdvisorAccess.REST.Models;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.REST.Orders
{
	public class OrderCriteriaTests
	{
		[ Test ]
		public void ToString_WhenDateRangeSpecified_ReturnsOrderFilterString()
		{
			var startDate = new DateTime( 2020, 1, 1, 1, 1, 1 );
			var endDate = new DateTime( 2022, 2, 2, 2, 2, 2 );
			var orderCriteria = new OrderCriteria( startDate, endDate );

			var result = orderCriteria.ToString();

			Assert.That( result, Is.EqualTo( $"({OrderCriteria.CheckoutDateFieldName} ge {startDate.ToDateTimeOffset()} and {OrderCriteria.CheckoutDateFieldName} le {endDate.ToDateTimeOffset()}) or " + 
				$"({OrderCriteria.PaymentDateFieldName} ge {startDate.ToDateTimeOffset()} and {OrderCriteria.PaymentDateFieldName} le {endDate.ToDateTimeOffset()}) or " + 
				$"({OrderCriteria.ShippingDateFieldName} ge {startDate.ToDateTimeOffset()} and {OrderCriteria.ShippingDateFieldName} le {endDate.ToDateTimeOffset()}) or " +
				$"({OrderCriteria.ImportDateFieldName} ge {startDate.ToDateTimeOffset()} and {OrderCriteria.ImportDateFieldName} le {endDate.ToDateTimeOffset()}) " ) );
		}
	}
}