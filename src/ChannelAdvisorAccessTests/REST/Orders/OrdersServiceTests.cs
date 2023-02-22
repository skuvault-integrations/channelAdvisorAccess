using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.REST.Shared;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChannelAdvisorAccessTests.REST.Orders
{
	public class OrdersServiceTests : RestAPITestBase
	{
		protected const int TestOrderId = 182948;
		protected const int TestOrderId2 = 178499;
		protected const int TestOrderId3 = 181648;

		[ Test ]
		public async Task GetOrdersAsyncByDate()
		{
			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( DateTime.UtcNow.AddMonths( -3 ), DateTime.UtcNow, this.Mark );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetOrdersGreaterEqualThanDateMoreThanOnePage()
		{
			var criteria = new OrderCriteria
			{
				OrderCreationFilterBeginTimeGMT = new DateTime(2018, 11, 01)
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

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

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterThan( 10 );
		}

		[ Test ]
		public async Task GetOrdersGreaterEqualThanDateByOrderStatusUpdate()
		{
			var criteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = DateTime.UtcNow.AddMonths( -2 ),
				StatusUpdateFilterEndTimeGMT = DateTime.UtcNow,
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterOrEqualTo( 2 );
			Assert.IsFalse( result.Any( r => r.ShippingInfo.ShipmentList == null ) );
		}

		[ Test ]
		public async Task GetOrdersGreaterEqualThanDateByPaymentStatusUpdate()
		{
			var criteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = new DateTime(2019, 06, 03, 12, 0, 0),
				StatusUpdateFilterEndTimeGMT = new DateTime(2019, 06, 03, 13, 0, 0)
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

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

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

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

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

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

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );
			var order = result.FirstOrDefault();
			order.Should().NotBeNull();
			var orderItem1 = order.ShoppingCart.LineItemSKUList.FirstOrDefault( item => item.SKU.Equals( sku1 ) ) as OrderLineItemItemResponse;
			var orderItem2 = order.ShoppingCart.LineItemSKUList.FirstOrDefault( item => item.SKU.Equals( sku2 ) ) as OrderLineItemItemResponse;
			orderItem1.DistributionCenterCode.Should().Be( "Louisville" );
			orderItem2.DistributionCenterCode.Should().Be( "DC4" );
		}

		[ Test ]
		public async Task GetOrdersAsync_Taxes()
		{
			var criteria = new OrderCriteria
			{
				OrderIDList = new int[] { TestOrderId },
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			OrderCart shoppingCart = result.First().ShoppingCart;
			//Always returned as 0 from the CA api
			//shoppingCart.LineItemSKUList.Any( x => x.TaxCost != null && x.TaxCost != 0 ).Should().BeTrue();
			shoppingCart.LineItemInvoiceList.Any( x => x.UnitPrice != 0 && x.LineItemType == "SalesTax" ).Should().BeTrue();
		}

		[ Test ]
		public async Task GetOrdersAsync_Promotions()
		{
			var criteria = new OrderCriteria
			{
				OrderIDList = new int[] { TestOrderId },
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			OrderCart shoppingCart = result.First().ShoppingCart;
			//Always returned as 0 from the CA api
			//shoppingCart.LineItemSKUList.Any( x => x.ItemPromoList != null && x.ItemPromoList.Any() ).Should().BeTrue();
			shoppingCart.LineItemPromoList.Any( x => x.UnitPrice != 0 ).Should().BeTrue();
		}

		[ Test ]
		public async Task GetOrdersAsync_ShouldReturnShippingCost()
		{
			var criteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = DateTime.UtcNow.AddMonths( -2 ),
				StatusUpdateFilterEndTimeGMT = DateTime.UtcNow,
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );
			
			result.Any( o => o.ShoppingCart.LineItemInvoiceList.Any( i => i.LineItemType == "Shipping" ) ).Should().BeTrue();
		}

		[ Test ]
		public void GivenTooSmallTimeout_WhenGetOrdersAsyncIsCalled_ThenExceptionIsReturned()
		{
			var timeouts = new ChannelAdvisorTimeouts();
			var tinyTimeout = new ChannelAdvisorOperationTimeout( 10 );
			timeouts.Set( ChannelAdvisorOperationEnum.ListOrdersRest, tinyTimeout );

			var ordersService = ServicesFactory.CreateOrdersRestService( RestCredentials.AccountName, null, RestCredentials.AccessToken, RestCredentials.RefreshToken, timeouts );

			var ex = Assert.Throws< ChannelAdvisorException >( async () => {
				var orders = await ordersService.GetOrdersAsync< OrderResponseDetailComplete >( DateTime.UtcNow.AddDays( -3 ), DateTime.UtcNow, this.Mark );
			} );
			
			ex.Should().NotBeNull();
			ex.InnerException.Should().NotBeNull();
			ex.InnerException.InnerException.GetType().Should().Be( typeof( TaskCanceledException ) );
		}

		[ Test ]
		public async Task WhenGetOrdersAsyncIsCalled_ThenModifiedLastActivityTimeIsExpected()
		{
			var activityTimeBeforeMakingAnyRequest = DateTime.UtcNow;
			await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( DateTime.UtcNow.AddDays( -14 ), DateTime.UtcNow, this.Mark );

			this.OrdersService.LastActivityTime.Should().BeAfter( activityTimeBeforeMakingAnyRequest );
		}
		
		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_WhenFilterByImportDatesAndStatusUpdateDates_ReturnOrdersHaveLastUpdateDateInTheSpecifiedDateRange()
		{
			var beginDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;
			var criteria = new OrderCriteria
			{
				ImportDateFilterBeginTimeGMT = beginDate,
				ImportDateFilterEndTimeGMT = endDate,
				StatusUpdateFilterBeginTimeGMT = beginDate,
				StatusUpdateFilterEndTimeGMT = endDate,
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			Assert.True( result.All( x => beginDate <= x.LastUpdateDate && x.LastUpdateDate <= endDate ) );
		}
	}
}
