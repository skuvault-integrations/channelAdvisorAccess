using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Orders;
using RestModels = ChannelAdvisorAccess.REST.Models;
using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Orders
{
	[ TestFixture ]
	public class OrdersServiceTests: TestsBase
	{
		protected const int TestOrderId = 185936;
		protected const int TestOrderId2 = 186058;

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersByIdsAsync()
		{
			var result = await this.OrdersService.GetOrdersByIdsAsync( new[] { TestOrderId, TestOrderId2 }, this.Mark, CancellationToken.None );

			result.Should().NotBeEmpty();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_ShouldReturnTaxes()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( startDate, endDate, this.Mark, CancellationToken.None );

			result.SelectMany( x => x.ShoppingCart.LineItemInvoiceList.Where( y => y.UnitPrice != 0 && y.LineItemType == "SalesTax" ) ).Any().Should().BeTrue();
			//Always returned as 0 from the CA api
			//result[x].ShoppingCart.LineItemSKUList.Any( x => x.TaxCost != null && x.TaxCost != 0 ).Should().BeTrue();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersByIdsAsync_Promotions()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( startDate, endDate, this.Mark, CancellationToken.None );

			result.SelectMany( x => x.ShoppingCart.LineItemPromoList.Where( y => y.UnitPrice != 0 ) ).Any().Should().BeTrue();
			//Always returned as 0 from the CA api
			//result[x].ShoppingCart.LineItemSKUList.Any(x => x.ItemPromoList != null && x.ItemPromoList.Any()).Should().BeTrue();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_ShouldReturnShippingCost()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( startDate, endDate, this.Mark );

			result.Any( o => o.ShoppingCart.LineItemInvoiceList.Any( i => i.LineItemType == "Shipping" ) ).Should().BeTrue();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_WhenGetOrdersAsyncIsCalled_ThenModifiedLastActivityTimeIsExpected()
		{
			var activityTimeBeforeMakingAnyRequest = this.OrdersService.LastActivityTime;
			var startDate = DateTime.UtcNow.AddDays( -14 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( startDate, endDate, this.Mark );

			this.OrdersService.LastActivityTime.Should().BeAfter( activityTimeBeforeMakingAnyRequest );
		}

		[ Explicit ]
		[ Test ]
		public void AdminService_IsDisposable()
		{
			var factory = new ChannelAdvisorServicesFactory( Credentials.DeveloperKey, Credentials.DeveloperPassword, null, null );
			OrdersService service;
			
			using ( service = ( OrdersService )factory.CreateOrdersService( "test", Credentials.AccountId ) )
			{
				Assert.That( service.Disposed, Is.False ); // not be disposed yet
			}

			try
			{
				Assert.That( service.Disposed, Is.True ); // expecting an exception.
			}
			catch ( Exception ex )
			{
				Assert.That( ex, Is.TypeOf<ObjectDisposedException>() );
			}
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_WhenFilterByDates_ReturnOrdersHaveLastUpdateDateInTheSpecifiedDateRange()
		{
			var startDate = DateTime.UtcNow.AddMonths( -2 );
			var endDate = DateTime.UtcNow;

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( startDate, endDate, this.Mark );

			Assert.That( result.All( x => startDate <= x.LastUpdateDate && x.LastUpdateDate <= endDate ), Is.True );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_WhenOrderStatusWasUpdatedDuringTheFilterDateRange_butCreatedBefore_ThenReturnSuchOrder()
		{
			//OrderID = 186059
			var startDate = DateTime.SpecifyKind( DateTime.Parse( "2/24/2023 9:36:41 PM", CultureInfo.InvariantCulture ), DateTimeKind.Utc );
			var endDate = DateTime.SpecifyKind( DateTime.Parse( "2/24/2023 9:38:41 PM", CultureInfo.InvariantCulture ), DateTimeKind.Utc );

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( startDate, endDate, this.Mark );

			Assert.That( result.Any( x => x.OrderTimeGMT <= startDate || endDate <= x.OrderTimeGMT ), Is.True );
			Assert.That( result.All( x => startDate <= x.LastUpdateDate && x.LastUpdateDate <= endDate ), Is.True );
		}

		private void ValidateLastActivityDateTimeUpdated()
		{
			this.OrdersService.LastActivityTime.Should().NotBe( this.serviceLastActivityDateTime );			
		}
	}
}