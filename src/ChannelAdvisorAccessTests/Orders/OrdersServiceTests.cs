using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.OrderService;
using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Inventory
{
	[ TestFixture ]
	public class OrdersServiceTests: TestsBase
	{
		protected const int TestOrderId = 185936;
		protected const int TestOrderId2 = 185938;
		protected const int TestOrderId3 = 185939;		

		[ Test ]
		public async Task GetOrdersAsync()
		{
			var criteria = new OrderCriteria
			{
				OrderIDList = new int[] { TestOrderId, TestOrderId2 },
				DetailLevel = DetailLevelTypes.Complete
			};
			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, CancellationToken.None );

			result.Should().NotBeEmpty();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public async Task GetOrdersAsync_Taxes()
		{
			var criteria = new OrderCriteria
			{
				OrderIDList = new int[] { TestOrderId3 },
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, CancellationToken.None );

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
				OrderIDList = new int[] { TestOrderId3 },
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, CancellationToken.None );

			OrderCart shoppingCart = result.First().ShoppingCart;
			//Always returned as 0 from the CA api
			//shoppingCart.LineItemSKUList.Any(x => x.ItemPromoList != null && x.ItemPromoList.Any()).Should().BeTrue();
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

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, CancellationToken.None );

			result.Any( o => o.ShoppingCart.LineItemInvoiceList.Any( i => i.LineItemType == "Shipping" ) ).Should().BeTrue();
		}

		[ Test ]
		public async Task WhenGetOrdersAsyncIsCalled_ThenModifiedLastActivityTimeIsExpected()
		{
			var activityTimeBeforeMakingAnyRequest = this.OrdersService.LastActivityTime;
			var criteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = DateTime.UtcNow.AddDays( -14 ),
				StatusUpdateFilterEndTimeGMT = DateTime.UtcNow,
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, CancellationToken.None );

			this.OrdersService.LastActivityTime.Should().BeAfter( activityTimeBeforeMakingAnyRequest );
		}

		private void ValidateLastActivityDateTimeUpdated()
		{
			this.OrdersService.LastActivityTime.Should().NotBe( this.serviceLastActivityDateTime );			
		}
	}
}