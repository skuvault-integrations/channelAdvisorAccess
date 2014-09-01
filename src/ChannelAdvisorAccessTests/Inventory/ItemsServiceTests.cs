using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.InventoryService;
using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Inventory
{
	public class ItemsServiceTests: TestsBase
	{
		[ Test ]
		public void UpdateSkuQuantity()
		{
			//------------ Arrange
			this.ItemsService.UpdateQuantityAndPrice( CreateItemQuantityAndPrice( 2 ) );

			//------------ Act
			this.ItemsService.UpdateQuantityAndPrice( CreateItemQuantityAndPrice( 5 ) );

			//------------ Assert
			this.ItemsService.GetAvailableQuantity( TestSku ).Should().Be( 5 );
		}

		private static InventoryItemQuantityAndPrice CreateItemQuantityAndPrice( int quantity )
		{
			return new InventoryItemQuantityAndPrice { Sku = TestSku, Quantity = quantity, UpdateType = InventoryQuantityUpdateTypes.Available, DistributionCenterCode = TestDistributionCenterCode };
		}
	}
}