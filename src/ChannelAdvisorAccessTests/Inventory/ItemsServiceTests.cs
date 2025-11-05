using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Items;
using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Inventory
{
	[ Explicit ]
	public class ItemsServiceTests: TestsBase
	{
		[ Test ]
		public void UpdateSkuQuantity()
		{
			//------------ Arrange

			// clear product quantity for all distribution centers
			string[] distributionCentersCodes = this.ItemsService.GetDistributionCenterList( this.Mark ).Select(dc => dc.DistributionCenterCode).ToArray();

			foreach(string distributionCenterCode in distributionCentersCodes)
				this.ItemsService.UpdateQuantityAndPrice( CreateItemQuantityAndPrice( 0, distributionCenterCode ), this.Mark );

			this.ItemsService.UpdateQuantityAndPrice( CreateItemQuantityAndPrice( 2 ), this.Mark );

			//------------ Act
			this.ItemsService.UpdateQuantityAndPrice( CreateItemQuantityAndPrice( 5 ), this.Mark );

			//------------ Assert
			this.ItemsService.GetAvailableQuantity( TestSku, this.Mark ).Should().Be( 5 );
			ValidateLastActivityDateTimeUpdated();
		}

		private static InventoryItemQuantityAndPrice CreateItemQuantityAndPrice( int quantity )
		{
			return CreateItemQuantityAndPrice(quantity, TestDistributionCenterCode);
		}

		private static InventoryItemQuantityAndPrice CreateItemQuantityAndPrice( int quantity, string distributionCenterCode )
		{
			return new InventoryItemQuantityAndPrice { Sku = TestSku, Quantity = quantity, UpdateType = InventoryQuantityUpdateTypes.Available, DistributionCenterCode = distributionCenterCode };
		}

		[ Test ]
		public void Ping()
		{
			//------------ Arrange

			//------------ Act
			this.ItemsService.Ping( this.Mark );

			//------------ Assert
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void PingAsync()
		{
			//------------ Arrange

			//------------ Act
			this.ItemsService.PingAsync( this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void DoesSkuExist_When_Exist()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoesSkuExist( TestSku, this.Mark );

			//------------ Assert
			result.Should().BeTrue();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void DoesSkuExist_When_Not_Exist()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoesSkuExist( TestSku + Guid.NewGuid(), this.Mark );

			//------------ Assert
			result.Should().BeFalse();			
		}

		[ Test ]
		public void DoesSkuExistAsync_When_Exist()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoesSkuExistAsync( TestSku, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().BeTrue();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void DoesSkuExistAsync_When_Not_Exist()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoesSkuExistAsync( TestSku + Guid.NewGuid(), this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().BeFalse();
		}

		[ Test ]
		public void DoSkusExist()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();
			//------------ Act
			var result = this.ItemsService.DoSkusExist( new[] { TestSku, incorrectSku }, this.Mark );

			//------------ Assert
			result.Should().BeEquivalentTo( new List< DoesSkuExistResponse >() { new DoesSkuExistResponse() { Sku = TestSku, Result = true }, new DoesSkuExistResponse() { Sku = incorrectSku, Result = false } } );
		}

		[ Test ]
		public void DoSkusExist_ManyPagesWithIncorrectSkus()
		{
			//------------ Arrange
			var incorrectSkuList = new List< string >();
			for( var i = 0; i < 1500; i++ )
				incorrectSkuList.Add( Guid.NewGuid().ToString() );
			//------------ Act
			var result = this.ItemsService.DoSkusExist( incorrectSkuList, this.Mark );

			//------------ Assert
			result.Should().BeEquivalentTo( new List< DoesSkuExistResponse >() );
		}

		[ Test ]
		public void DoSkusExist_When_Empty()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoSkusExist( new string[] { }, this.Mark );

			//------------ Assert
			result.Should().HaveCount( 0 );
		}

		[ Test ]
		public void DoSkusExist_When_Not_Exists()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.DoSkusExist( new[] { incorrectSku }, this.Mark );

			//------------ Assert
			result.Should().HaveCount( 0 );
		}

		[ Test ]
		public void DoSkusExist_When_Many_Pages()
		{
			//------------ Arrange
			var skuList = new List<string>();
			for( int i = 0; i < 2050; i++ )
				skuList.Add( Guid.NewGuid().ToString() );
			skuList.Add( TestSku );

			//------------ Act
			var result = this.ItemsService.DoSkusExist( skuList, this.Mark );

			//------------ Assert
			result.Where( r => r.Result ).Should().BeEquivalentTo( new List< DoesSkuExistResponse >() { new DoesSkuExistResponse() { Sku = TestSku, Result = true } } );
		}

		[ Test ]
		public void DoSkusExistAsync()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.DoSkusExistAsync( new[] { TestSku, incorrectSku }, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().BeEquivalentTo( new List< DoesSkuExistResponse >() { new DoesSkuExistResponse() { Sku = TestSku, Result = true }, new DoesSkuExistResponse() { Sku = incorrectSku, Result = false } } );
		}

		[ Test ]
		public void DoSkusExistAsync_When_Empty()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.DoSkusExistAsync( new string[] { }, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().HaveCount( 0 );
		}

		[ Test ]
		public void DoSkusExistAsync_When_Not_Exists()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.DoSkusExistAsync( new[] { incorrectSku }, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().HaveCount( 0 );
		}

		[ Test ]
		[ Ignore("Manual test") ]
		public void GetAllItems()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetAllItems( this.Mark );

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetItems()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.GetItems( new[] { TestSku, incorrectSku }, this.Mark );

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
			result.Count().Should().Be( 1 );
			result.First().Sku.ToLower().Should().Be( TestSku.ToLower() );
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetItemsAsync()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.GetItemsAsync( new[] { TestSku, incorrectSku }, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
			result.Count().Should().Be( 1 );
			result.First().Sku.ToLower().Should().Be( TestSku.ToLower() );
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetItemQuantities()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetItemQuantities( TestSku, this.Mark );

			//------------ Assert
			result.Should().NotBeNull();
			result.Available.Should().BeGreaterThan( 0 );
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetItemQuantitiesAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetItemQuantitiesAsync( TestSku, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNull();
			result.Available.Should().BeGreaterThan( 0 );
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetClassificationConfigurationInformation()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetClassificationConfigurationInformation( this.Mark );

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetClassificationConfigurationInformationAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetClassificationConfigurationInformationAsync( this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNullOrEmpty();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetStoreInfo()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetStoreInfo( TestSku, this.Mark );

			//------------ Assert
			result.Should().NotBeNull();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetStoreInfoAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetStoreInfoAsync( TestSku, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNull();			
		}

		[ Test ]
		public void WhenGetStoreInfoAsyncIsCalled_ThenModifiedLastActivityTimeIsExpected()
		{
			var activityTimeBeforeMakingAnyCall = DateTime.UtcNow;
			this.ItemsService.GetStoreInfoAsync( TestSku, this.Mark ).GetAwaiter().GetResult();

			this.ItemsService.LastActivityTime.Should().BeAfter( activityTimeBeforeMakingAnyCall );
		}

		[ Test ]
		public void GetShippingInfo()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetShippingInfo( TestSku, this.Mark );

			//------------ Assert
			result.Should().NotBeNull();
			result.Length.Should().BeGreaterThan( 0 );
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetShippingInfoAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetShippingInfoAsync( TestSku, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNull();
			result.Length.Should().BeGreaterThan( 0 );
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetVariationInfo()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetVariationInfo( TestSku, this.Mark );

			//------------ Assert
			result.Should().NotBeNull();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetVariationInfoAsync()
		{
			//------------ Arrange

			//------------ Act
			var result = this.ItemsService.GetVariationInfoAsync( TestSku, this.Mark ).GetAwaiter().GetResult();

			//------------ Assert
			result.Should().NotBeNull();
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetAvailableQuantities()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.GetAvailableQuantities( new[] { TestSku, incorrectSku }, this.Mark ).ToArray();

			//------------ Assert
			result.Should().NotBeNull();
			result.Length.Should().Be( 2 );
			result[ 0 ].SKU.Should().Be( TestSku );
			result[ 0 ].MessageCode.Should().Be( 0 );
			result[ 0 ].Quantity.Should().BeGreaterThan( 0 );
			result[ 1 ].SKU.Should().Be( incorrectSku );
			result[ 1 ].MessageCode.Should().BeGreaterThan( 0 );
			result[ 1 ].Quantity.Should().Be( 0 );
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		public void GetAvailableQuantitiesAsync()
		{
			//------------ Arrange
			var incorrectSku = TestSku + Guid.NewGuid();

			//------------ Act
			var result = this.ItemsService.GetAvailableQuantitiesAsync( new[] { TestSku, incorrectSku }, this.Mark ).GetAwaiter().GetResult().ToArray();

			//------------ Assert
			result.Should().NotBeNull();
			result.Length.Should().Be( 2 );
			result[ 0 ].SKU.Should().Be( TestSku );
			result[ 0 ].MessageCode.Should().Be( 0 );
			result[ 0 ].Quantity.Should().BeGreaterThan( 0 );
			result[ 1 ].SKU.Should().Be( incorrectSku );
			result[ 1 ].MessageCode.Should().BeGreaterThan( 0 );
			result[ 1 ].Quantity.Should().Be( 0 );
			ValidateLastActivityDateTimeUpdated();
		}

		[ Test ]
		[ Ignore("Manual test") ]
		public void SpeedTest()
		{
			//------------ Arrange

			//------------ Act
			var startTime = DateTime.Now;
			var allSkus = this.ItemsService.GetAllSkusAsync( this.Mark ).GetAwaiter().GetResult();
			System.Diagnostics.Debug.WriteLine( ( DateTime.Now - startTime ).TotalSeconds );
			startTime = DateTime.Now;
			var existingsSkus = this.ItemsService.DoSkusExistAsync( allSkus, this.Mark ).GetAwaiter().GetResult();
			System.Diagnostics.Debug.WriteLine( ( DateTime.Now - startTime ).TotalSeconds );
			//------------ Assert
		}

		[ Test ]
		public void ItemsService_IsDisposable()
		{
			var factory = new ChannelAdvisorServicesFactory( Credentials.DeveloperKey, Credentials.DeveloperPassword, null, null );
			ItemsService service;

			using ( service = ( ItemsService )factory.CreateItemsService( "test", Credentials.AccountId ) )
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
			this.ItemsService.LastActivityTime.Should().NotBe( this.serviceLastActivityDateTime );			
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