using System.Linq;
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
		protected const int TestOrderId = 182948;
		protected const int TestOrderId2 = 182949;

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

		[ Test ]
		public async Task GetOrdersAsync_Taxes()
		{
			var criteria = new OrderCriteria
			{
				OrderIDList = new int[] { TestOrderId, TestOrderId2 },
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria );

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
				OrderIDList = new int[] { TestOrderId, TestOrderId2 },
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync<OrderResponseDetailComplete>(criteria);

			OrderCart shoppingCart = result.First().ShoppingCart;
			//Always returned as 0 from the CA api
			//shoppingCart.LineItemSKUList.Any(x => x.ItemPromoList != null && x.ItemPromoList.Any()).Should().BeTrue();
			shoppingCart.LineItemPromoList.Any( x => x.UnitPrice != 0 ).Should().BeTrue();
		}
	}
}