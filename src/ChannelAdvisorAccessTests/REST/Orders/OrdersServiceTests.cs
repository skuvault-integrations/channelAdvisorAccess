using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.OrderService;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccessTests.REST.Orders
{
	public class OrdersServiceTests : RestAPITestBase
	{
		protected const int TestOrderId = 177739;
		protected const int TestOrderId2 = 178499;

		[ Test ]
		public async Task GetOrdersAsyncByDate()
		{
			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( new DateTime(2019, 2, 20), new DateTime(2019, 2, 21) );

			result.Should().NotBeNullOrEmpty();
			result.Should().HaveCount(3);
		}

		[ Test ]
		public async Task GetOrdersGreaterEqualThanDateMoreThanOnePage()
		{
			var criteria = new OrderCriteria
			{
				OrderCreationFilterBeginTimeGMT = new DateTime(2018, 11, 01)
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterOrEqualTo( 20 );
		}

		[ Test ]
		public async Task GetOrdersLessEqualThanDate()
		{
			var criteria = new OrderCriteria
			{
				OrderCreationFilterEndTimeGMT = new DateTime(2019, 2, 20)
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterThan( 10 );
		}

		[ Test ]
		public async Task GetOrdersGreaterEqualThanDateByStatusUpdate()
		{
			var criteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = new DateTime(2018, 11, 01),
				StatusUpdateFilterEndTimeGMT = new DateTime(2019, 01, 01)
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterOrEqualTo( 2 );
		}

		[ Test ]
		public async Task GetOrdersById()
		{
			var criteria = new OrderCriteria
			{
				OrderIDList = new int[] { TestOrderId, TestOrderId2 }
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeNullOrEmpty();
			result.Should().HaveCount(2);
		}
	}
}
