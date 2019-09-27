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
			ClearSkuQuantityForEachDc( TestSku );
			ClearSkuQuantityForEachDc( TestSku2 );

			var request = new InventoryItemQuantityAndPrice[]
			{
				CreateItemQuantityAndPrice( TestSku, 150 ),
				CreateItemQuantityAndPrice( TestSku2, 110 )
			};

			this.ItemsService.UpdateQuantityAndPricesAsync( request ).GetAwaiter().GetResult();

			var quantities = this.ItemsService.GetAvailableQuantitiesAsync( new string[] { TestSku, TestSku2 }).GetAwaiter().GetResult();

			quantities.Should().NotBeNullOrEmpty();
			quantities.Should().HaveCount( 2 );
			quantities.First( item => item.SKU.Equals( TestSku ) ).Quantity.Should().Be( 150 );
			quantities.First( item => item.SKU.Equals( TestSku2 ) ).Quantity.Should().Be( 110 );
		}

		[ Test ]
		public void UpdateSkusQuantityInLargeCatalog()
		{
			List< InventoryItemQuantityAndPrice > request = new List<InventoryItemQuantityAndPrice>();
			string baseSku = "testSku";
			int skusNumber = 2000;

			for (int i = 1; i <= skusNumber; i++ )
			{
				var itemRequest = CreateItemQuantityAndPrice( baseSku + i.ToString(), i * 2 );
				request.Add( itemRequest );
			}

			this.ItemsService.UpdateQuantityAndPricesAsync( request ).GetAwaiter().GetResult();

			var quantities = this.ItemsService.GetAvailableQuantitiesAsync( request.Select( i => i.Sku ) ).GetAwaiter().GetResult();

			quantities.Should().NotBeNullOrEmpty();
			quantities.Should().HaveCount( skusNumber );
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
		public void UpdateSkuLocationWhenSourceCatalogIsLarge()
		{
			List< InventoryItemSubmit > inventoryItems = new List< InventoryItemSubmit >();
			var i = 1;
			var maxSkus = 10000;

			while ( i <= maxSkus )
			{
				inventoryItems.Add( new InventoryItemSubmit()
				{
					 Sku = "testSku" + i.ToString(),
					 WarehouseLocation = "ADC" + i.ToString()
				});

				i += 1;
			}

			this.ItemsService.SynchItems( inventoryItems );

			var items = this.ItemsService.GetItems( new string[] { "testSku1", "testSku150" } );
			items.Should().NotBeNullOrEmpty();
			items.First().WarehouseLocation.Should().Be( "ADC1" );
			items.Last().WarehouseLocation.Should().Be( "ADC150" );
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
					Cost = 12.0m
				 }
			};

			this.ItemsService.SynchItemAsync( submitItem ).GetAwaiter().GetResult();

			var result = this.ItemsService.GetItemsAsync( new string[] { TestSku } ).GetAwaiter().GetResult().FirstOrDefault( item => item.Sku.ToLower().Equals( TestSku.ToLower() ) );

			result.Should().NotBeNull();
			result.PriceInfo.Cost.Should().Be( 12.0m );
		}

		[ Test ]
		public void UpdateSkuPriceAndUpc()
		{
			var testUPC = "123456";
			InventoryItemSubmit submitItem = new InventoryItemSubmit()
			{
				 Sku = TestSku,
				 PriceInfo = new PriceInfo()
				 {
					Cost = 12.0m
				 },
				 UPC = testUPC
			};

			this.ItemsService.SynchItemAsync( submitItem ).GetAwaiter().GetResult();

			var result = this.ItemsService.GetItemsAsync( new string[] { TestSku } ).GetAwaiter().GetResult().FirstOrDefault( item => item.Sku.ToLower().Equals( TestSku.ToLower() ) );

			result.Should().NotBeNull();
			result.PriceInfo.Cost.Should().Be( 12.0m );
			result.UPC.Should().Be( testUPC );
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
		public void GetItemWithSingleQuoteInName()
		{
			var testSku = "AEROPOSTALE NY WOMEN'S JUNIORS ZIP HOODIE HOODED S";

			var result = this.ItemsService.GetItems( new[] { testSku } );

			result.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void GetItemsByPages()
		{
			var skus = new List< string >();
			int pages = 10;
			int pageSize = 100;

			for ( int i = 1; i <= pages * pageSize; i++ )
				skus.Add( "testSku" + i.ToString() );

			var result = this.ItemsService.GetItems( skus );

			result.Should().NotBeNullOrEmpty();
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
					DateRangeStartGMT = DateTime.Now.AddMonths( -3 ),
					DateRangeEndGMT = DateTime.Now
				 }
			};

			var skus = this.ItemsService.GetFilteredSkusAsync( filter ).GetAwaiter().GetResult();

			skus.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void GetFilteredSkusPageByUpdateDate()
		{
			var page = 5;
			var filter = new ItemsFilter
			{
				 Criteria = new InventoryItemCriteria(){
					DateRangeField = TimeStampFields.LastUpdateDate,
					DateRangeStartGMT = DateTime.Now.AddMonths( -3 ),
					DateRangeEndGMT = DateTime.Now
				 }
			};

			var result = this.ItemsService.GetFilteredSkusAsync( filter, page, 100 ).GetAwaiter().GetResult();

			result.AllPagesQueried.Should().BeFalse();
			result.FinalPageNumber.Should().Be( page );
			result.Response.Should().NotBeEmpty();
		}

		[ Test ]
		public void GetFilteredItemsPageByUpdateDate()
		{
			int page = 5;
			int pageSize = 100;
			var filter = new ItemsFilter
			{
				 Criteria = new InventoryItemCriteria(){
					DateRangeField = TimeStampFields.LastUpdateDate,
					DateRangeStartGMT = DateTime.Now.AddMonths( -3 ),
					DateRangeEndGMT = DateTime.Now
				 }
			};

			var result = this.ItemsService.GetFilteredItemsAsync( filter, page, pageSize, null ).GetAwaiter().GetResult();

			result.Response.Should().NotBeNullOrEmpty();
			result.AllPagesQueried.Should().BeFalse();
		}

		[ Test ]
		public void GetFilteredItemsByUpdateDateAndNotExistingPage()
		{
			int page = 10000;
			int pageSize = 100;
			var filter = new ItemsFilter
			{
				 Criteria = new InventoryItemCriteria(){
					DateRangeField = TimeStampFields.LastUpdateDate,
					DateRangeStartGMT = DateTime.Now.AddMonths( -3 ),
					DateRangeEndGMT = DateTime.Now
				 }
			};

			var result = this.ItemsService.GetFilteredItemsAsync( filter, page, pageSize, null ).GetAwaiter().GetResult();

			result.Response.Should().BeEmpty();
			result.AllPagesQueried.Should().BeTrue();
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
		public void DoSkusExistWhenSourceCatalogIsLarge()
		{
			var skus = new List< string >();

			for ( int i = 0; i < 50000; i++ )
				skus.Add( "testSku" + i.ToString() );

			var result = this.ItemsService.DoSkusExist( skus );

			result.Count().Should().Be( skus.Count() );
		}

		[ Test ]
		public void DoSkusExistWithSpecialName()
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
		public void GetAvailableQuantitiesWhenSourceCatalogIsLarge()
		{
			var skus = new List< string >();

			for ( int i = 0; i < 500; i++ )
				skus.Add( "testSku" + i.ToString() );

			var result = this.ItemsService.GetAvailableQuantities( skus );

			result.Should().NotBeNullOrEmpty();
			result.Count().Should().Be( skus.Count() );
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

		[ Test ]
		public void GetProductsIdUsingCache()
		{
			var itemsService = this.ItemsService as ChannelAdvisorAccess.REST.Services.Items.ItemsService;
			var cache = new Dictionary< string, int >
			{
				{ TestSku, 10 },
				{ TestSku2, 12 }
			};

			itemsService.SetProductCache( cache );
			var productsId = itemsService.GetProductsId( new string[] { TestSku, TestSku2 }, null ).Result;

			productsId.Should().NotBeEmpty();
			productsId[ TestSku ].Should().Be( 10 );
			productsId[ TestSku2 ].Should().Be( 12 );
		}

	}
}
