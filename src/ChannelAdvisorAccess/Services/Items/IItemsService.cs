using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.REST.Models;

namespace ChannelAdvisorAccess.Services.Items
{
	public interface IItemsService
	{
		/// <summary>
		/// Gets the account name.
		/// </summary>
		string Name{ get; }

		/// <summary>
		/// Gets the account id.
		/// </summary>
		string AccountId{ get; }

		void Ping( Mark mark = null );
		Func< string > AdditionalLogInfo{ get; set; }
		Task PingAsync( Mark mark = null );
		bool DoesSkuExist( string sku, CancellationToken token, Mark mark = null );
		Task< bool > DoesSkuExistAsync( string sku, CancellationToken token, Mark mark = null );
		IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable< string > skus, CancellationToken token, Mark mark = null );
		Task< IEnumerable< DoesSkuExistResponse > > DoSkusExistAsync( IEnumerable< string > skus, CancellationToken token, Mark mark = null );
		IEnumerable< InventoryItemResponse > GetAllItems( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Gets the items by skus.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <param name="mark"></param>
		/// <returns>Enumerator to process items. <c>null</c> is returned
		/// for non-existing skus.</returns>
		/// <remarks>Items are pulled 1 at a time to handle non-existing skus.
		/// This results in slower performance.</remarks>
		IEnumerable< InventoryItemResponse > GetItems( IEnumerable< string > skus, CancellationToken token, Mark mark = null );

		Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable< string > skus, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="startPage">The first page number to query.</param>
		/// <param name="pageLimit">The max number of pages to query.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< PagedApiResponse< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, int startPage, CancellationToken token, int pageLimit, Mark mark = null );

		AttributeInfo[] GetAttributes( string sku, CancellationToken token, Mark mark = null );
		Task< AttributeInfo[] > GetAttributesAsync( string sku, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <param name="mark"></param>
		/// <returns>Item quantities.</returns>
		/// <remarks>This is required since <see cref="ItemsService.GetItems"/> returns
		/// only available quantity.</remarks>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		QuantityInfoResponse GetItemQuantities( string sku, CancellationToken token, Mark mark = null );

		Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku, CancellationToken token, Mark mark = null );
		ClassificationConfigurationInformation[] GetClassificationConfigurationInformation( CancellationToken token, Mark mark = null );
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigurationInformationAsync( CancellationToken token, Mark mark = null );
		StoreInfo GetStoreInfo( string sku, CancellationToken token, Mark mark = null );
		Task< StoreInfo > GetStoreInfoAsync( string sku, CancellationToken token, Mark mark = null );
		ImageInfoResponse[] GetImageList( string sku, CancellationToken token, Mark mark = null );
		Task< ImageInfoResponse[] > GetImageListAsync( string sku, CancellationToken token, Mark mark = null );

		DistributionCenterInfoResponse[] GetShippingInfo( string sku, CancellationToken token, Mark mark = null );
		Task< DistributionCenterInfoResponse[] > GetShippingInfoAsync( string sku, CancellationToken token, Mark mark = null );
		VariationInfo GetVariationInfo( string sku, CancellationToken token, Mark mark = null );
		Task< VariationInfo > GetVariationInfoAsync( string sku, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Gets the available quantity.
		/// </summary>
		/// <param name="sku">The sku of the item.</param>
		/// <param name="mark"></param>
		/// <returns>
		/// The Available quantity for the specified sku.
		/// </returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryQuantity"/>
		int GetAvailableQuantity( string sku, CancellationToken token, Mark mark = null );

		Task< int > GetAvailableQuantityAsync( string sku, CancellationToken token, Mark mark = null );
		IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus, CancellationToken token, Mark mark = null, int delatInMs = 5000 );
		Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus, CancellationToken token, Mark mark = null );
		IEnumerable< string > GetAllSkus( CancellationToken token, Mark mark = null );
		Task< IEnumerable< string > > GetAllSkusAsync( CancellationToken token, Mark mark = null );
		IEnumerable< string > GetFilteredSkus( ItemsFilter filter, CancellationToken token, Mark mark = null );
		Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter, CancellationToken token, Mark mark = null );
		Task< PagedApiResponse< string > > GetFilteredSkusAsync( ItemsFilter filter, int startPage, int pageLimit, CancellationToken token, Mark mark = null );
		void SynchItem( InventoryItemSubmit item, CancellationToken token, bool isCreateNew = false, Mark mark = null );
		Task SynchItemAsync( InventoryItemSubmit item, CancellationToken token, bool isCreateNew = false, Mark mark = null );
		Task< DistributionCenter[] > GetDistributionCentersAsync( CancellationToken token, Mark mark = null );
		void SynchItems( IEnumerable< InventoryItemSubmit > items, CancellationToken token, bool isCreateNew = false, Mark mark = null );
		Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items, CancellationToken token, bool isCreateNew = false, Mark mark = null );
		void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice, CancellationToken token, Mark mark = null );
		Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice, CancellationToken token, Mark mark = null );
		void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, CancellationToken token, Mark mark = null );
		Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, CancellationToken token, Mark mark = null );
		void RemoveLabelListFromItemList( string[] labels, IEnumerable< string > skus, string reason, CancellationToken token, Mark mark = null );
		Task RemoveLabelListFromItemListAsync( string[] labels, IEnumerable< string > skus, string reason, CancellationToken token, Mark mark = null );
		void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, CancellationToken token, Mark mark = null );
		Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, CancellationToken token, Mark mark = null );
		void DeleteItem( string sku, CancellationToken token, Mark mark = null );
		Task DeleteItemAsync( string sku, CancellationToken token, Mark mark = null );
		ClassificationConfigurationInformation[] GetClassificationConfigInfo( CancellationToken token, Mark mark = null );
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigInfoAsync( CancellationToken token, Mark mark = null );

		DistributionCenterResponse[] GetDistributionCenterList( CancellationToken token, Mark mark = null );
		Task< DistributionCenterResponse[] > GetDistributionCenterListAsync( CancellationToken token, Mark mark = null );
	}
}