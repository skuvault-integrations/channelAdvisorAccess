using System;
using System.Collections.Generic;
using System.Linq;
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

		[ Test ]
		public void Ping()
		{
			//------------ Arrange

			//------------ Act
			this.ItemsService.Ping();

			//------------ Assert
		}

		[ Test ]
		public void PingAsync()
		{
			//------------ Arrange

			//------------ Act
			this.ItemsService.PingAsync().GetAwaiter().GetResult();

			//------------ Assert
		}

		[ Test ]
		public void DoesSkuExist_When_Exist()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoesSkuExist( TestSku );

			//------------ Assert
			result.Should().BeTrue();
		}

		[ Test ]
		public void DoesSkuExist_When_Not_Exist()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoesSkuExist( TestSku + Guid.NewGuid() );

			//------------ Assert
			result.Should().BeFalse();
		}

		[ Test ]
		public void DoesSkuExistAsync_When_Exist()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoesSkuExistAsync( TestSku ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().BeTrue();
		}

		[ Test ]
		public void DoesSkuExistAsync_When_Not_Exist()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoesSkuExistAsync( TestSku + Guid.NewGuid() ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().BeFalse();
		}

		[ Test ]
		public void DoSkusExist()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();
			//------------ Act
			var result = this.ItemsService.DoSkusExist( new[] { TestSku, incorrectSku } );

			//------------ Assert
			result.ShouldBeEquivalentTo( new List< DoesSkuExistResponse >() { new DoesSkuExistResponse() { Sku = TestSku, Result = true }, new DoesSkuExistResponse() { Sku = incorrectSku, Result = false } } );
		}

		[ Test ]
		public void DoSkusExist_When_Empty()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoSkusExist( new string[] { } );

			//------------ Assert
			result.Should().HaveCount( 0 );
		}

		[ Test ]
		public void DoSkusExist_When_Not_Exists()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.DoSkusExist( new[] { incorrectSku } );

			//------------ Assert
			result.Should().HaveCount( 0 );
		}

		[ Test ]
		public void DoSkusExistAsync()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.DoSkusExistAsync( new[] { TestSku, incorrectSku } ).GetAwaiter().GetResult();

			//------------ Assert
			result.ShouldBeEquivalentTo( new List< DoesSkuExistResponse >() { new DoesSkuExistResponse() { Sku = TestSku, Result = true }, new DoesSkuExistResponse() { Sku = incorrectSku, Result = false } } );
		}

		[ Test ]
		public void DoSkusExistAsync_When_Empty()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoSkusExistAsync( new string[] { } ).GetAwaiter().GetResult();;

			//------------ Assert
			result.Should().HaveCount( 0 );
		}

		[ Test ]
		public void DoSkusExistAsync_When_Not_Exists()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.DoSkusExistAsync( new[] { incorrectSku } ).GetAwaiter().GetResult();;

			//------------ Assert
			result.Should().HaveCount( 0 );
		}

		[ Test ]
		[ Ignore ]
		public void GetAllItems()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetAllItems();

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void GetItems()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.GetItems( new[] { TestSku, incorrectSku } );

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
			result.Count().ShouldBeEquivalentTo( 1 );
			result.First().Sku.ShouldBeEquivalentTo( TestSku );
		}

		[ Test ]
		public void GetItemsAsync()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.GetItemsAsync( new[] { TestSku, incorrectSku } ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
			result.Count().ShouldBeEquivalentTo( 1 );
			result.First().Sku.ShouldBeEquivalentTo( TestSku );
		}

		[ Test ]
		public void GetItemQuantities()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetItemQuantities( TestSku );

			//------------ Assert
			result.Should().NotBeNull();
			result.Available.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetItemQuantitiesAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetItemQuantitiesAsync( TestSku ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNull();
			result.Available.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetClassificationConfigurationInformation()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetClassificationConfigurationInformation();

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void GetClassificationConfigurationInformationAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetClassificationConfigurationInformationAsync().GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void GetStoreInfo()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetStoreInfo( TestSku );

			//------------ Assert
			result.Should().NotBeNull();
		}

		[ Test ]
		public void GetStoreInfoAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetStoreInfoAsync( TestSku ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNull();
		}

		[ Test ]
		public void GetShippingInfo()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetShippingInfo( TestSku );

			//------------ Assert
			result.Should().NotBeNull();
			result.Length.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetShippingInfoAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetShippingInfoAsync( TestSku ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNull();
			result.Length.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetVariationInfo()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetVariationInfo( TestSku );

			//------------ Assert
			result.Should().NotBeNull();
		}

		[ Test ]
		public void GetVariationInfoAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetVariationInfoAsync( TestSku ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNull();
		}

		[ Test ]
		public void GetAvailableQuantities()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.GetAvailableQuantities( new[] { TestSku, incorrectSku } ).ToArray();

			//------------ Assert
			result.Should().NotBeNull();
			result.Length.ShouldBeEquivalentTo( 2 );
			result[ 0 ].SKU.ShouldBeEquivalentTo( TestSku );
			result[ 0 ].MessageCode.ShouldBeEquivalentTo( 0 );
			result[ 0 ].Quantity.Should().BeGreaterThan( 0 );
			result[ 1 ].SKU.ShouldBeEquivalentTo( incorrectSku );
			result[ 1 ].MessageCode.Should().BeGreaterThan( 0 );
			result[ 1 ].Quantity.ShouldBeEquivalentTo( 0 );
		}

		[ Test ]
		public void GetAvailableQuantitiesAsync()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.GetAvailableQuantitiesAsync( new[] { TestSku, incorrectSku } ).GetAwaiter().GetResult().ToArray();

			//------------ Assert
			result.Should().NotBeNull();
			result.Length.ShouldBeEquivalentTo( 2 );
			result[ 0 ].SKU.ShouldBeEquivalentTo( TestSku );
			result[ 0 ].MessageCode.ShouldBeEquivalentTo( 0 );
			result[ 0 ].Quantity.Should().BeGreaterThan( 0 );
			result[ 1 ].SKU.ShouldBeEquivalentTo( incorrectSku );
			result[ 1 ].MessageCode.Should().BeGreaterThan( 0 );
			result[ 1 ].Quantity.ShouldBeEquivalentTo( 0 );
		}

		[ Test ]
		[ Ignore ]
		public void SpeedTest()
		{
			//------------ Arrange

			//------------ Act
			var startTime = DateTime.Now;
			var allSkus = this.ItemsService.GetAllSkusAsync().GetAwaiter().GetResult();
			System.Diagnostics.Debug.WriteLine( ( DateTime.Now - startTime ).TotalSeconds );
			startTime = DateTime.Now;
			var existingsSkus = this.ItemsService.DoSkusExistAsync( allSkus ).GetAwaiter().GetResult();
			System.Diagnostics.Debug.WriteLine( ( DateTime.Now - startTime ).TotalSeconds );
			//------------ Assert
		}

		//IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter, Mark mark = null );
		//Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, Mark mark = null );
		//Task< PagedApiResponse< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark = null );
		//AttributeInfo[] GetAttributes( string sku, Mark mark = null );
		//Task< AttributeInfo[] > GetAttributesAsync( string sku, Mark mark = null );

		//ImageInfoResponse[] GetImageList( string sku, Mark mark = null );
		//Task< ImageInfoResponse[] > GetImageListAsync( string sku, Mark mark = null );
		//DistributionCenterInfoResponse[] GetShippingInfo( string sku, Mark mark = null );
		//Task< DistributionCenterInfoResponse[] > GetShippingInfoAsync( string sku, Mark mark = null );

		//int GetAvailableQuantity( string sku, Mark mark = null );
		//Task< int > GetAvailableQuantityAsync( string sku, Mark mark = null );
		//IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus, Mark mark = null, int delatInMs = 5000 );
		//Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus, Mark mark = null );
		//IEnumerable< string > GetAllSkus( Mark mark = null );
		//Task< IEnumerable< string > > GetAllSkusAsync( Mark mark = null );
		//IEnumerable< string > GetFilteredSkus( ItemsFilter filter, Mark mark = null );
		//Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter, Mark mark = null );
		//Task< PagedApiResponse< string > > GetFilteredSkusAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark = null );
		//void SynchItem( InventoryItemSubmit item, bool isCreateNew = false, Mark mark = null );
		//Task SynchItemAsync( InventoryItemSubmit item, bool isCreateNew = false, Mark mark = null );
		//void SynchItems( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false, Mark mark = null );
		//Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false, Mark mark = null );
		//void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark = null );
		//Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark = null );
		//void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark = null );
		//Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark = null );
		//void RemoveLabelListFromItemList( string[] labels, IEnumerable< string > skus, string reason, Mark mark = null );
		//Task RemoveLabelListFromItemListAsync( string[] labels, IEnumerable< string > skus, string reason, Mark mark = null );
		//void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark = null );
		//Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark = null );
		//void DeleteItem( string sku, Mark mark = null );
		//Task DeleteItemAsync( string sku, Mark mark = null );
		//ClassificationConfigurationInformation[] GetClassificationConfigInfo( Mark mark = null );
		//Task< ClassificationConfigurationInformation[] > GetClassificationConfigInfoAsync( Mark mark = null );
		//DistributionCenterResponse[] GetDistributionCenterList( Mark mark = null );
		//Task< DistributionCenterResponse[] > GetDistributionCenterListAsync( Mark mark = null );
	}
}