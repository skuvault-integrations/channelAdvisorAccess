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
		[ Test ]
		public async Task GetOrdersAsync()
		{
			var criteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = DateTime.Now.AddHours( -1 ),
				StatusUpdateFilterEndTimeGMT = DateTime.Now,
				DetailLevel = DetailLevelTypes.Complete
			};
			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeEmpty();
		}
	}
}