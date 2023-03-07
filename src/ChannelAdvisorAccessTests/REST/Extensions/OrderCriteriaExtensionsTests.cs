using ChannelAdvisorAccess.REST.Extensions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.REST.Extensions
{
	public class OrderCriteriaExtensionsTests
	{
		[ Test ]
		public void ToRequestFilterString_WhenOrderIdFiltersSpecified_ReturnsOrderIdFilterString()
		{
			const int orderId = 1234;
			
			var result = orderId.ToRequestFilterString();
			
			Assert.That( result, Is.EqualTo( $"{OrderCriteriaExtensions.OrderIdFieldName} eq {orderId} " ) );
		}
	}
}