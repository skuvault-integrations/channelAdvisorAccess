using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Constants;
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
		protected const int TestOrderId2 = 185938;
		protected const int TestOrderId3 = 185939;		

		[ Test ]
		public async Task GetOrdersAsync()
		{
			var criteria = new RestModels.OrderCriteria
			{
				OrderIDList = new[] { TestOrderId, TestOrderId2 },
				DetailLevel = DetailLevelTypes.Complete
			};
			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			result.Should().NotBeEmpty();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public async Task GetOrdersAsync_Taxes()
		{
			var criteria = new RestModels.OrderCriteria
			{
				OrderIDList = new[] { TestOrderId3 },
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
			var criteria = new RestModels.OrderCriteria
			{
				OrderIDList = new[] { TestOrderId3 },
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			OrderCart shoppingCart = result.First().ShoppingCart;
			//Always returned as 0 from the CA api
			//shoppingCart.LineItemSKUList.Any(x => x.ItemPromoList != null && x.ItemPromoList.Any()).Should().BeTrue();
			shoppingCart.LineItemPromoList.Any( x => x.UnitPrice != 0 ).Should().BeTrue();
		}

		[ Test ]
		public async Task GetOrdersAsync_ShouldReturnShippingCost()
		{
			var criteria = new RestModels.OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = DateTime.UtcNow.AddMonths( -2 ),
				StatusUpdateFilterEndTimeGMT = DateTime.UtcNow,
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			result.Any( o => o.ShoppingCart.LineItemInvoiceList.Any( i => i.LineItemType == "Shipping" ) ).Should().BeTrue();
		}

		[ Test ]
		public async Task WhenGetOrdersAsyncIsCalled_ThenModifiedLastActivityTimeIsExpected()
		{
			var activityTimeBeforeMakingAnyRequest = this.OrdersService.LastActivityTime;
			var criteria = new RestModels.OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = DateTime.UtcNow.AddDays( -14 ),
				StatusUpdateFilterEndTimeGMT = DateTime.UtcNow,
				DetailLevel = DetailLevelTypes.Complete
			};

			var result = await this.OrdersService.GetOrdersAsync< OrderResponseDetailComplete >( criteria, this.Mark );

			this.OrdersService.LastActivityTime.Should().BeAfter( activityTimeBeforeMakingAnyRequest );
		}

		[ Test ]
		public void AdminService_IsDisposable()
		{
			var factory = new ChannelAdvisorServicesFactory( Credentials.DeveloperKey, Credentials.DeveloperPassword, null, null );
			OrdersService service;
			
			using ( service = ( OrdersService )factory.CreateOrdersService( "test", Credentials.AccountId ) )
			{
				Debug.Assert( !service.Disposed ); // not be disposed yet
			}

			try
			{
				Debug.Assert( service.Disposed ); // expecting an exception.
			}
			catch ( Exception ex )
			{
				Debug.Assert( ex is ObjectDisposedException ); 
			}
		}

		private void ValidateLastActivityDateTimeUpdated()
		{
			this.OrdersService.LastActivityTime.Should().NotBe( this.serviceLastActivityDateTime );			
		}
	}
}