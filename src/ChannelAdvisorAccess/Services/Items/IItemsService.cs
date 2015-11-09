using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;

namespace ChannelAdvisorAccess.Services.Items
{
	public interface IItemsService
	{
		/// <summary>
		/// Gets the account name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the account id.
		/// </summary>
		string AccountId{ get; }

		void Ping( Mark mark = null );
		Func< string > AdditionalLogInfo{ get; set; }
		Task PingAsync( Mark mark = null );
		bool DoesSkuExist( string sku, Mark mark = null );
		Task< bool > DoesSkuExistAsync( string sku, Mark mark = null );
		IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable< string > skus, Mark mark = null );
		Task< IEnumerable< DoesSkuExistResponse > > DoSkusExistAsync( IEnumerable< string > skus, Mark mark = null );
		IEnumerable< InventoryItemResponse > GetAllItems( Mark mark = null );

		/// <summary>
		/// Gets the items by skus.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <param name="mark"></param>
		/// <returns>Enumerator to process items. <c>null</c> is returned
		/// for non-existing skus.</returns>
		/// <remarks>Items are pulled 1 at a time to handle non-existing skus.
		/// This results in slower performance.</remarks>
		IEnumerable< InventoryItemResponse > GetItems( IEnumerable< string > skus, Mark mark = null );

		Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable< string > skus, Mark mark = null );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter, Mark mark = null );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, Mark mark = null );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="startPage">The first page number to query.</param>
		/// <param name="pageLimit">The max number of pages to query.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< PagedApiResponse< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark = null );

		AttributeInfo[] GetAttributes( string sku, Mark mark = null );
		Task< AttributeInfo[] > GetAttributesAsync( string sku, Mark mark = null );

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <param name="mark"></param>
		/// <returns>Item quantities.</returns>
		/// <remarks>This is required since <see cref="ItemsService.GetItems"/> returns
		/// only available quantity.</remarks>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		QuantityInfoResponse GetItemQuantities( string sku, Mark mark = null );

		Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku, Mark mark = null );
		ClassificationConfigurationInformation[] GetClassificationConfigurationInformation( Mark mark = null );
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigurationInformationAsync( Mark mark = null );
		StoreInfo GetStoreInfo( string sku, Mark mark = null );
		Task< StoreInfo > GetStoreInfoAsync( string sku, Mark mark = null );
		ImageInfoResponse[] GetImageList( string sku, Mark mark = null );
		Task< ImageInfoResponse[] > GetImageListAsync( string sku, Mark mark = null );
		
		DistributionCenterInfoResponse[] GetShippingInfo( string sku, Mark mark = null );
		Task< DistributionCenterInfoResponse[] > GetShippingInfoAsync( string sku, Mark mark = null );
		VariationInfo GetVariationInfo( string sku, Mark mark = null );
		Task< VariationInfo > GetVariationInfoAsync( string sku, Mark mark = null );

		/// <summary>
		/// Gets the available quantity.
		/// </summary>
		/// <param name="sku">The sku of the item.</param>
		/// <param name="mark"></param>
		/// <returns>
		/// The Available quantity for the specified sku.
		/// </returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryQuantity"/>
		int GetAvailableQuantity( string sku, Mark mark = null );

		Task< int > GetAvailableQuantityAsync( string sku, Mark mark = null );
		IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus, Mark mark = null );
		Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus, Mark mark = null );
		IEnumerable< string > GetAllSkus( Mark mark = null );
		Task< IEnumerable< string > > GetAllSkusAsync( Mark mark = null );
		IEnumerable< string > GetFilteredSkus( ItemsFilter filter, Mark mark = null );
		Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter, Mark mark = null );
		Task< PagedApiResponse< string > > GetFilteredSkusAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark = null );
		void SynchItem( InventoryItemSubmit item, bool isCreateNew = false );
		Task SynchItemAsync( InventoryItemSubmit item, bool isCreateNew = false );
		void SynchItems( IEnumerable< InventoryItemSubmit > items, bool isCreateNew = false );
		Task SynchItemsAsync( IEnumerable<InventoryItemSubmit> items, bool isCreateNew = false );
		void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice );
		Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice );
		void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices );
		Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices );
		void RemoveLabelListFromItemList( string[] labels, IEnumerable< string > skus, string reason );
		Task RemoveLabelListFromItemListAsync( string[] labels, IEnumerable< string > skus, string reason );
		void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason );
		Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason );
		void DeleteItem( string sku );
		Task DeleteItemAsync( string sku );
		ClassificationConfigurationInformation[] GetClassificationConfigInfo();
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigInfoAsync();

		DistributionCenterResponse[] GetDistributionCenterList();
		Task< DistributionCenterResponse[] > GetDistributionCenterListAsync();
	}
}