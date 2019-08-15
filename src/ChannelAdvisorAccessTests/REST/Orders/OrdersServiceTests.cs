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
		protected const int TestOrderId = 180457;
		protected const int TestOrderId2 = 178499;
		protected const int TestOrderId3 = 181648;

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
				OrderCreationFilterEndTimeGMT = new DateTime(2019, 4, 01)
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterThan( 10 );
		}

		[ Test ]
		public async Task GetOrdersGreaterEqualThanDateByOrderStatusUpdate()
		{
			var criteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = new DateTime(2019, 01, 01),
				StatusUpdateFilterEndTimeGMT = new DateTime(2019, 05, 01)
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterOrEqualTo( 2 );
		}

		[ Test ]
		public async Task GetOrdersGreaterEqualThanDateByPaymentStatusUpdate()
		{
			var criteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = new DateTime(2019, 06, 03, 12, 0, 0),
				StatusUpdateFilterEndTimeGMT = new DateTime(2019, 06, 03, 13, 0, 0)
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterOrEqualTo( 1 );
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

		[ Test ]
		public async Task GetManyOrdersById()
		{
			var orders = new List< int >();

			for ( int i = 1; i <= 20; i++ )
				orders.Add( i );

			orders.AddRange( new int[] { TestOrderId2 } );

			var criteria = new OrderCriteria
			{
				OrderIDList = orders.ToArray()
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

			result.Should().NotBeNullOrEmpty();
			result.Should().HaveCount(1);
		}

		[ Test ]
		public async Task GetOrderWithDifferentItemsDC()
		{
			var sku1 = "testSku3";
			var sku2 = "testsku2";

			var criteria = new OrderCriteria
			{
				OrderIDList = new int[] { TestOrderId3 }
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );
			var order = result.FirstOrDefault();
			order.Should().NotBeNull();
			var orderItem1 = order.ShoppingCart.LineItemSKUList.FirstOrDefault( item => item.SKU.Equals( sku1 ) ) as OrderLineItemItemResponse;
			var orderItem2 = order.ShoppingCart.LineItemSKUList.FirstOrDefault( item => item.SKU.Equals( sku2 ) ) as OrderLineItemItemResponse;
			orderItem1.DistributionCenterCode.Should().Be( "Louisville" );
			orderItem2.DistributionCenterCode.Should().Be( "DC4" );
		}
	}
}
