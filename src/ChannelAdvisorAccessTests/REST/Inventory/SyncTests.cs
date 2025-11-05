using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Constants;
using System.Diagnostics;
using System;

namespace ChannelAdvisorAccessTests.REST.Inventory
{
	[ Explicit ]
	[ TestFixture ]
	public class SyncTests : RestAPITestBase
	{
		private int averageRequestProcessingTimeInSec = 5;
		private int averageProductExportProcessingTimeInSec = 120;
		private int averageProductPageProcessingTimeInSec = 15;

		/// <summary>
		///	Given large SkuVault sku catalog, large ChannelAdvisor catalog and small number of sku to update (over 100)
		///	When do full inventory sync
		///	Then total execution time is not to exceed ( averageProductExportProcessingTimeInSec + ( caSkusNumber * 2 + 1 ) * avgRequestProcessingTimeInSec ) / threadsNumber
		/// </summary>
		[ Test ]
		public void DoFullInventorySyncWhenSkuVaultCatalogIsLargeAndCaCatalogIsLargeAndSmallNumberOfSkuToUpdate()
		{
			int threadsNumber = 4;
			int svCatalogSize = 10000;
			int caCatalogSize = ( (ChannelAdvisorAccess.REST.Services.Items.ItemsService)this.ItemsService ).GetCatalogSize( this.Mark ).GetAwaiter().GetResult();
			List< string > svSkus = new List< string >();

			for ( int i = 0; i < svCatalogSize; i++ )
				svSkus.Add( "testSku" + i.ToString() );

			var sv = Stopwatch.StartNew();

			// we should use CA product export feature to pull catalog as fast as possible
			var caSkus = this.ItemsService.DoSkusExist( svSkus, this.Mark );
			// we should pull products directly by sku to get available quantity (can't get available quantity via export)
			var caSkusQuantity = this.ItemsService.GetAvailableQuantities( caSkus.Where( item => item.Result ).Select( item => item.Sku ), this.Mark );

			List< InventoryItemQuantityAndPrice > requests = new List<InventoryItemQuantityAndPrice>();
			var dc = this.ItemsService.GetDistributionCenterList( this.Mark ).First();

			foreach ( var caSkuQuantity in caSkusQuantity )
			{
				requests.Add( new InventoryItemQuantityAndPrice()
				{
					Sku = caSkuQuantity.SKU,
					UpdateType = InventoryQuantityUpdateTypes.Available,
					Quantity = new Random().Next(1, 100),
					DistributionCenterCode = dc.DistributionCenterCode
				});
			}

			this.ItemsService.UpdateQuantityAndPrices( requests, this.Mark );

			Assert.That( sv.Elapsed.TotalSeconds < ( averageProductExportProcessingTimeInSec + averageRequestProcessingTimeInSec * ( caSkusQuantity.Count() * 2 + 1 ) / threadsNumber ), Is.True );
		}

		/// <summary>
		///	Given small SkuVault sku catalog, large ChannelAdvisor catalog and small number of sku to update (over 100)
		///	When do full inventory sync
		///	Then total execution time is not to exceed ( caSkusNumber * 2 + 1 ) * avgRequestProcessingTimeInSec 
		/// </summary>
		[ Test ]
		public void DoFullInventorySyncWhenSkuVaultCatalogIsSmallAndCaCatalogIsLargeAndSmallNumberOfSkuToUpdate()
		{
			int threadsNumber = 4;
			int svCatalogSize = 200;
			int caCatalogSize = ( (ChannelAdvisorAccess.REST.Services.Items.ItemsService)this.ItemsService ).GetCatalogSize( this.Mark ).GetAwaiter().GetResult();
			List< string > svSkus = new List< string >();

			for ( int i = 0; i < svCatalogSize; i++ )
				svSkus.Add( "testSku" + i.ToString() );

			var sv = Stopwatch.StartNew();

			// we should pull product data directly by sku
			var caSkus = this.ItemsService.DoSkusExist( svSkus, this.Mark );
			// we should pull products directly by sku to get available quantity (can't get available quantity via export)
			var caSkusQuantity = this.ItemsService.GetAvailableQuantities( caSkus.Where( item => item.Result ).Select( item => item.Sku ), this.Mark );

			List< InventoryItemQuantityAndPrice > requests = new List<InventoryItemQuantityAndPrice>();
			var dc = this.ItemsService.GetDistributionCenterList( this.Mark ).First();

			foreach ( var caSkuQuantity in caSkusQuantity )
			{
				requests.Add( new InventoryItemQuantityAndPrice()
				{
					Sku = caSkuQuantity.SKU,
					UpdateType = InventoryQuantityUpdateTypes.Available,
					Quantity = new Random().Next(1, 100),
					DistributionCenterCode = dc.DistributionCenterCode
				});
			}

			this.ItemsService.UpdateQuantityAndPrices( requests, this.Mark );

			Assert.That( sv.Elapsed.TotalSeconds < ( averageRequestProcessingTimeInSec * ( caSkusQuantity.Count() * 2 + 1 ) / threadsNumber ), Is.True );
		}

		/// <summary>
		///	Given large SkuVault sku catalog, small ChannelAdvisor catalog and small number of sku to update (over 100)
		///	When do full inventory sync
		///	Then total execution time is not to exceed ( totalSkusPages * averageProductPageProcessingTimeInSec + caSkusNumber + 1 ) * avgRequestProcessingTimeInSec 
		/// </summary>
		[ Test ]
		public void DoFullInventorySyncWhenSkuVaultCatalogIsLargeAndCaCatalogIsSmall()
		{
			int threadsNumber = 4;
			int svCatalogSize = 10000;
			int caCatalogSize = ( (ChannelAdvisorAccess.REST.Services.Items.ItemsService )base.LightWeightItemsService).GetCatalogSize( this.Mark ).GetAwaiter().GetResult();
			List< string > svSkus = new List< string >();

			for ( int i = 0; i < svCatalogSize; i++ )
				svSkus.Add( "testSku" + i.ToString() );

			var sv = Stopwatch.StartNew();

			// we should pull product data directly by sku
			var caSkus = base.LightWeightItemsService.DoSkusExist( svSkus, this.Mark );
			// we should pull products directly by sku to get available quantity (can't get available quantity via export)
			var caSkusQuantity = base.LightWeightItemsService.GetAvailableQuantities( caSkus.Where( item => item.Result ).Select( item => item.Sku ), this.Mark );

			List< InventoryItemQuantityAndPrice > requests = new List<InventoryItemQuantityAndPrice>();
			var dc = base.LightWeightItemsService.GetDistributionCenterList( this.Mark ).First();

			foreach ( var caSkuQuantity in caSkusQuantity )
			{
				requests.Add( new InventoryItemQuantityAndPrice()
				{
					Sku = caSkuQuantity.SKU,
					UpdateType = InventoryQuantityUpdateTypes.Available,
					Quantity = new Random().Next(1, 100),
					DistributionCenterCode = dc.DistributionCenterCode
				});
			}

			base.LightWeightItemsService.UpdateQuantityAndPrices( requests, this.Mark );

			int totalSkusPages = (int)Math.Ceiling( (double) caCatalogSize / 100 );
			Assert.That( sv.Elapsed.TotalSeconds < ( averageRequestProcessingTimeInSec * ( totalSkusPages * averageProductPageProcessingTimeInSec + caSkusQuantity.Count() + 1 ) / threadsNumber ), Is.True );
		}

		[ Test ]
		public void DoLocationSyncWhenSkuVaultCatalogIsLarge()
		{
			var svCatalogSize = 50000;
			var warehouseLocationPrefix = "01-AA-";
			List< InventoryItemSubmit > svProducts = new List< InventoryItemSubmit >();

			for ( int i = 0; i < svCatalogSize; i++ )
				svProducts.Add( new InventoryItemSubmit() { Sku = "testSku" + i.ToString(), WarehouseLocation = warehouseLocationPrefix + (i + 1).ToString() } );

			var caSkus = base.LightWeightItemsService.DoSkusExist( svProducts.Select( item => item.Sku ), this.Mark )
										.Where( r => r.Result )
										.Select( r => r.Sku );
			var commonSkus = svProducts.Where( pr => caSkus.Contains( pr.Sku ) );
			base.LightWeightItemsService.SynchItems( commonSkus, this.Mark );

			var commonSkusDetails = base.LightWeightItemsService.GetItems( commonSkus.Select( s => s.Sku ), this.Mark );
			Assert.That( commonSkusDetails.FirstOrDefault( d => d.WarehouseLocation == null || d.WarehouseLocation.IndexOf( warehouseLocationPrefix ) < 0 ) == null, Is.True );
		}
	}
}
