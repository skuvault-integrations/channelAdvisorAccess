using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.Services.Items;

namespace ChannelAdvisorAccessTests.REST.Inventory
{
	public class ItemsServiceTests : RestAPITestBase
	{
		protected const string TestSku = "testSku1";
		protected const string TestSku2 = "testSku10";
		protected const string TestDistributionCenterCode = "Louisville";
		protected const string TestSkuLocation = "A4(1000)";
		protected const string TestSkuUPC = "7991999";

		[ Test ]
		public void UpdateSkuQuantity()
		{
			this.ClearSkuQuantityForEachDc( TestSku );

			this.ItemsService.UpdateQuantityAndPrice( CreateItemQuantityAndPrice( TestSku, 10 ) );

			//------------ Act
			this.ItemsService.UpdateQuantityAndPrice( CreateItemQuantityAndPrice( TestSku, 120 ) );

			//------------ Assert
			this.ItemsService.GetAvailableQuantity( TestSku ).Should().Be( 120 );
		}

		[ Test ]
		public void UpdateSkusQuantity()
		{
			//ClearSkuQuantityForEachDC( TestSku );
			//ClearSkuQuantityForEachDC( TestSku2 );

			var request = new InventoryItemQuantityAndPrice[]
			{
				CreateItemQuantityAndPrice( TestSku, 100 ),
				CreateItemQuantityAndPrice( TestSku2, 110 )
			};

			this.ItemsService.UpdateQuantityAndPricesAsync( request ).GetAwaiter().GetResult();

			var quantities = this.ItemsService.GetAvailableQuantitiesAsync( new string[] { TestSku, TestSku2 }).GetAwaiter().GetResult();

			quantities.Should().NotBeNullOrEmpty();
			quantities.Should().HaveCount( 2 );
			quantities.First( item => item.SKU.Equals( TestSku ) ).Quantity.Should().Be( 100 );
			quantities.First( item => item.SKU.Equals( TestSku2 ) ).Quantity.Should().Be( 110 );
		}

		private void ClearSkuQuantityForEachDc( string sku )
		{
			string[] distributionCentersCodes = this.ItemsService.GetDistributionCenterList().Select( dc => dc.DistributionCenterCode ).ToArray();

			foreach( string distributionCenterCode in distributionCentersCodes )
			{
				this.ItemsService.UpdateQuantityAndPrice( CreateItemQuantityAndPrice( sku, 0, distributionCenterCode ) );
			}
		}

		private static InventoryItemQuantityAndPrice CreateItemQuantityAndPrice( string sku, int quantity )
		{
			return CreateItemQuantityAndPrice( sku, quantity, TestDistributionCenterCode );
		}

		private static InventoryItemQuantityAndPrice CreateItemQuantityAndPrice( string sku, int quantity, string distributionCenterCode )
		{
			return new InventoryItemQuantityAndPrice { Sku = sku, Quantity = quantity, UpdateType = InventoryQuantityUpdateTypes.Available, DistributionCenterCode = distributionCenterCode };
		}

		[ Test ]
		public void UpdateSkuLocation()
		{
			var submitItem = new InventoryItemSubmit()
			{
				 Sku = TestSku,
				 WarehouseLocation = TestSkuLocation
			};

			this.ItemsService.SynchItemAsync( submitItem ).GetAwaiter().GetResult();

			var result = this.ItemsService.GetItemsAsync( new string[] { TestSku } ).GetAwaiter().GetResult().FirstOrDefault( item => item.Sku.ToLower().Equals( TestSku.ToLower() ) );

			result.Should().NotBeNull();
			result.WarehouseLocation.Should().Equals( TestSkuLocation );
		}

		[ Test ]
		public void UpdateSkuUPC()
		{
			InventoryItemSubmit submitItem = new InventoryItemSubmit()
			{
				 Sku = TestSku,
				 UPC = TestSkuUPC
			};

			this.ItemsService.SynchItemAsync( submitItem ).GetAwaiter().GetResult();

			var result = this.ItemsService.GetItemsAsync( new string[] { TestSku } ).GetAwaiter().GetResult().FirstOrDefault( item => item.Sku.ToLower().Equals( TestSku.ToLower() ) );

			result.Should().NotBeNull();
			result.UPC.Should().Equals( TestSkuUPC );
		}

		[ Test ]
		public void UpdateSkuPrice()
		{
			InventoryItemSubmit submitItem = new InventoryItemSubmit()
			{
				 Sku = TestSku,
				 PriceInfo = new PriceInfo()
				 {
					RetailPrice = 11.0m, 
					StorePrice = 10.0m,
					Cost = 12.0m
				 }
			};

			this.ItemsService.SynchItemAsync( submitItem ).GetAwaiter().GetResult();

			var result = this.ItemsService.GetItemsAsync( new string[] { TestSku } ).GetAwaiter().GetResult().FirstOrDefault( item => item.Sku.ToLower().Equals( TestSku.ToLower() ) );

			result.Should().NotBeNull();
			result.PriceInfo.RetailPrice.Should().Be( 11.0m );
			result.PriceInfo.StorePrice.Should().Be( 10.0m );
			result.PriceInfo.Cost.Should().Be( 12.0m );
		}

		[ Test ]
		public void GetItemQuantitiesAsync()
		{
			var result = this.ItemsService.GetItemQuantitiesAsync( TestSku ).GetAwaiter().GetResult();

			result.Should().NotBeNull();
			result.Available.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetItems()
		{
			var incorrectSku = TestSku + Guid.NewGuid();

			var result = this.ItemsService.GetItems( new[] { TestSku, incorrectSku } );

			result.Should().NotBeNullOrEmpty();
			result.Count().ShouldBeEquivalentTo( 1 );
			result.First().Sku.ToLower().ShouldBeEquivalentTo( TestSku.ToLower() );
		}

		[ Test ]
		public void GetFilteredNewSkus()
		{
			var filter = new ItemsFilter
			{
				 Criteria = new InventoryItemCriteria(){
					DateRangeField = TimeStampFields.CreateDate,
					DateRangeStartGMT = DateTime.Now.AddMonths( -1 ),
					DateRangeEndGMT = DateTime.Now
				 }
			};

			var skus = this.ItemsService.GetFilteredSkusAsync( filter ).GetAwaiter().GetResult();

			skus.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void GetFilteredSkusByUpdateDate()
		{
			var filter = new ItemsFilter
			{
				 Criteria = new InventoryItemCriteria(){
					DateRangeField = TimeStampFields.LastUpdateDate,
					DateRangeStartGMT = DateTime.Now.AddDays ( -10 ),
					DateRangeEndGMT = DateTime.Now
				 }
			};

			var skus = this.ItemsService.GetFilteredSkusAsync( filter ).GetAwaiter().GetResult();

			skus.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void GetFilteredSkusComplexQuery()
		{
			var filter = new ItemsFilter
			{
				 Criteria = new InventoryItemCriteria(){
					DateRangeField = TimeStampFields.LastUpdateDate,
					DateRangeStartGMT = DateTime.Now.AddMonths ( -1 ),
					DateRangeEndGMT = DateTime.Now, 
					ClassificationName = "Footwear",
					LabelName = "updateLabel"
				 }
			};

			var skus = this.ItemsService.GetFilteredSkusAsync( filter ).GetAwaiter().GetResult();

			skus.Should().NotBeNullOrEmpty();
		}
		
		[ Test ]
		[ Ignore ]
		public void GetAllItems()
		{
			var result = this.ItemsService.GetAllItems();

			result.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void DoesSkuExist_When_Exist()
		{
			var result = this.ItemsService.DoesSkuExist( TestSku );
			result.Should().BeTrue();
		}

		[ Test ]
		public void DoesSkuExist_When_Not_Exist()
		{
			var result = this.ItemsService.DoesSkuExist( TestSku + Guid.NewGuid() );

			result.Should().BeFalse();
		}

		[ Test ]
		public void DoSkusExist()
		{
			var incorrectSku = TestSku + Guid.NewGuid();
			var result = this.ItemsService.DoSkusExist( new[] { TestSku, incorrectSku } );

			result.ShouldBeEquivalentTo( new List< DoesSkuExistResponse >() { new DoesSkuExistResponse() { Sku = TestSku, Result = true }, new DoesSkuExistResponse() { Sku = incorrectSku, Result = false } } );
		}

		[ Test ]
		public void DoSkusWithSpecialName()
		{
			var testSku = "ROYAL-252-#-02";
			var result = this.ItemsService.DoSkusExist( new[] { testSku } );

			result.First().Result.Should().BeFalse();
		}

		[ Test ]
		public void DoSkusForKit()
		{
			var kitSku = "kit2019-03-16T00:08:10.002";

			var result = this.ItemsService.DoSkusExist( new[] { kitSku } );

			result.First().Result.Should().BeFalse();
		}

		[ Test ]
		public void GetAvailableQuantities()
		{
			var incorrectSku = TestSku + Guid.NewGuid();

			var result = this.ItemsService.GetAvailableQuantities( new[] { TestSku, incorrectSku } ).ToArray();

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
		public void GetAvailableQuantity()
		{
			int quantity = this.ItemsService.GetAvailableQuantityAsync( TestSku ).GetAwaiter().GetResult();

			quantity.Should().BeGreaterThan(0);
		}

		[ Test ]
		[ Ignore ]
		public void GetClassificationConfigurationInformation()
		{
			var result = this.ItemsService.GetClassificationConfigurationInformation();

			result.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void GetStoreInfo()
		{
			var result = this.ItemsService.GetStoreInfo( TestSku );

			result.Should().NotBeNull();
		}

		[ Test ]
		public void GetShippingInfo()
		{
			var result = this.ItemsService.GetShippingInfo( TestSku );

			result.Should().NotBeNull();
			result.Length.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetSkuAttributes()
		{
			var result = this.ItemsService.GetAttributes( TestSku );

			result.Should().NotBeNullOrEmpty();
			result.Length.Should().BeGreaterOrEqualTo( 3 );

			result.Select( attr => attr.Name ).Intersect( new string[] { "Color", "DefaultPackingTypes", "Restaraunts" }).Count().Should().Be( 3 );
		}


		[ Test ]
		public void GetVariationInfo()
		{
			var variationSku = "ALTERNATE 1";
			var result = this.ItemsService.GetVariationInfo( variationSku );

			result.Should().NotBeNull();
		}

		[ Test ]
		public void GetSkuImages()
		{
			var result = this.ItemsService.GetImageList( TestSku );

			result.Should().NotBeNullOrEmpty();
			result.Should().HaveCount(1);
		}

		[ Test ]
		public void GetItemsByClassification()
		{
			string classificationName = "Footwear";

			ItemsFilter filter = new ItemsFilter();
			filter.Criteria = new InventoryItemCriteria()
			{
				ClassificationName = classificationName
			};

			var result = this.ItemsService.GetFilteredItems( filter );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterThan( 10 );
			result.Where( item => item.Classification != null )
				.Any( item => !item.Classification.Equals( classificationName ) )
				.Should()
				.BeFalse();
		}

		[ Test ]
		[ Ignore ]
		public void GetItemsByLabel()
		{
			string labelName = "updateLabel";

			ItemsFilter filter = new ItemsFilter();
			filter.Criteria = new InventoryItemCriteria()
			{
				LabelName = labelName
			};

			var result = this.ItemsService.GetFilteredItems( filter );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().BeGreaterThan( 10 );
		}
	}
}
