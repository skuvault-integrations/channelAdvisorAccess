using System;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.OrderService;
using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Inventory
{
	public class OrdersServiceTests: TestsBase
	{
		protected const int TestOrderId = 177739;
		protected const int TestOrderId2 = 178499;

		[ Test ]
		public async Task GetOrdersAsync()
		{
			var criteria = new OrderCriteria
			{
				OrderIDList = new int[] { TestOrderId, TestOrderId2 },
				DetailLevel = DetailLevelTypes.Complete
			};
			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeEmpty();
		}
	}
}