using ChannelAdvisorAccess.Exceptions;
using SoapOrdersService = ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.REST.Shared;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChannelAdvisorAccessTests.REST.Orders
{
	public class OrdersServiceTests : RestAPITestBase
	{
		protected const int TestOrderId = 182948;
		protected const int TestOrderId2 = 186058;

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsyncByDate()
		{
			var startDate = DateTime.UtcNow.AddMonths( -3 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark, CancellationToken.None );

			result.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersGreaterEqualThanDateMoreThanOnePage()
		{
			var startDate = new DateTime(2018, 11, 01);
			var endDate = DateTime.Now;

			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark );

			result.Count().Should().BeGreaterOrEqualTo( 20 );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersLessEqualThanDate()
		{
			var endDate = new DateTime(2019, 4, 01);

			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( DateTime.MinValue, endDate, this.Mark );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterThan( 10 );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersGreaterEqualThanDateByOrderStatusUpdate()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterOrEqualTo( 2 );
			Assert.IsFalse( result.Any( r => r.ShippingInfo.ShipmentList == null ) );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersGreaterEqualThanDateByPaymentStatusUpdate()
		{
			var startDate = new DateTime(2019, 06, 03, 12, 0, 0);
			var endDate = new DateTime(2019, 06, 03, 13, 0, 0);

			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterOrEqualTo( 1 );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersByIdsAsync()
		{
			var result = await this.OrdersService.GetOrdersByIdsAsync( new[] { TestOrderId, TestOrderId2 }, this.Mark, CancellationToken.None );

			result.Should().NotBeNullOrEmpty();
			result.Should().HaveCount(2);
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersByIdsAsync_WhenManyOrderIds()
		{
			var orders = new List< int >();

			for ( int i = 1; i <= 20; i++ )
				orders.Add( i );

			orders.AddRange( new[] { TestOrderId2 } );

			var result = await this.OrdersService.GetOrdersByIdsAsync( orders.ToArray(), this.Mark, CancellationToken.None );

			result.Should().NotBeNullOrEmpty();
			result.Should().HaveCount(1);
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_WithDifferentItemsDC()
		{
			var sku1 = "testSku3";
			var sku2 = "testsku2";
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark, CancellationToken.None );

			var order = result.FirstOrDefault();
			order.Should().NotBeNull();
			var orderItem1 = order.ShoppingCart.LineItemSKUList.FirstOrDefault( item => item.SKU.Equals( sku1 ) ) as SoapOrdersService.OrderLineItemItemResponse;
			var orderItem2 = order.ShoppingCart.LineItemSKUList.FirstOrDefault( item => item.SKU.Equals( sku2 ) ) as SoapOrdersService.OrderLineItemItemResponse;
			orderItem1.DistributionCenterCode.Should().Be( "Louisville" );
			orderItem2.DistributionCenterCode.Should().Be( "DC4" );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_Taxes()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;
			
			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark, CancellationToken.None );

			result.SelectMany( o => o.ShoppingCart.LineItemInvoiceList.Where( x => x.UnitPrice != 0 && x.LineItemType == "SalesTax" ) ).Any().Should().BeTrue();
			//Always returned as 0 from the CA api
			//result[x].ShoppingCart.LineItemSKUList.Any( x => x.TaxCost != null && x.TaxCost != 0 ).Should().BeTrue();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_Promotions()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;
		
			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark, CancellationToken.None );

			result.SelectMany( o => o.ShoppingCart.LineItemPromoList.Where( x => x.UnitPrice != 0 ) ).Any().Should().BeTrue();
			//Always returned as 0 from the CA api
			//result[x].ShoppingCart.LineItemSKUList.Any( x => x.ItemPromoList != null && x.ItemPromoList.Any() ).Should().BeTrue();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_ShouldReturnShippingCost()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark );
			
			result.SelectMany( o => o.ShoppingCart.LineItemInvoiceList.Where( i => i.LineItemType == "Shipping" ) ).Any().Should().BeTrue();
		}

		[ Test ]
		public void GivenTooSmallTimeout_WhenGetOrdersAsyncIsCalled_ThenExceptionIsReturned()
		{
			var timeouts = new ChannelAdvisorTimeouts();
			var tinyTimeout = new ChannelAdvisorOperationTimeout( 10 );
			timeouts.Set( ChannelAdvisorOperationEnum.ListOrdersRest, tinyTimeout );

			var ordersService = ServicesFactory.CreateOrdersRestService( RestCredentials.AccountName, null, RestCredentials.AccessToken, RestCredentials.RefreshToken, timeouts );

			var ex = Assert.Throws< ChannelAdvisorException >( async () => {
				var orders = await ordersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( DateTime.UtcNow.AddDays( -3 ), DateTime.UtcNow, this.Mark );
			} );
			
			ex.Should().NotBeNull();
			ex.InnerException.Should().NotBeNull();
			ex.InnerException.InnerException.GetType().Should().Be( typeof( TaskCanceledException ) );
		}

		[ Test ]
		[ Explicit ]
		public async Task WhenGetOrdersAsyncIsCalled_ThenModifiedLastActivityTimeIsExpected()
		{
			var activityTimeBeforeMakingAnyRequest = DateTime.UtcNow;
			await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( DateTime.UtcNow.AddDays( -14 ), DateTime.UtcNow, this.Mark );

			this.OrdersService.LastActivityTime.Should().BeAfter( activityTimeBeforeMakingAnyRequest );
		}
		
		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_WhenFilterByDates_ReturnOrdersHaveLastUpdateDateInTheSpecifiedDateRange()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< SoapOrdersService.OrderResponseDetailComplete >( startDate, endDate, this.Mark );

			Assert.True( result.All( x => startDate <= x.LastUpdateDate && x.LastUpdateDate <= endDate ) );
		}
	}
}
