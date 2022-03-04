using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.REST.Models;

namespace ChannelAdvisorAccess.Services.Items
{
	public interface IItemsService : IDisposable
	{
		/// <summary>
		/// Gets the account name.
		/// </summary>
		string Name{ get; }

		/// <summary>
		/// Gets the account id.
		/// </summary>
		string AccountId{ get; }

		void Ping( Mark mark, CancellationToken token = default( CancellationToken ) );
		Func< string > AdditionalLogInfo{ get; set; }
		Task PingAsync( Mark mark, CancellationToken token = default( CancellationToken ) );
		bool DoesSkuExist( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< bool > DoesSkuExistAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable< string > skus, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< IEnumerable< DoesSkuExistResponse > > DoSkusExistAsync( IEnumerable< string > skus, Mark mark, CancellationToken token = default( CancellationToken ) );
		IEnumerable< InventoryItemResponse > GetAllItems( Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		/// Gets the items by skus.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <param name="mark"></param>
		/// <returns>Enumerator to process items. <c>null</c> is returned
		/// for non-existing skus.</returns>
		/// <remarks>Items are pulled 1 at a time to handle non-existing skus.
		/// This results in slower performance.</remarks>
		IEnumerable< InventoryItemResponse > GetItems( IEnumerable< string > skus, Mark mark, CancellationToken token = default( CancellationToken ) );

		Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable< string > skus, Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter, Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="startPage">The first page number to query.</param>
		/// <param name="pageLimit">The max number of pages to query.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< PagedApiResponse< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, int startPage, Mark mark, int pageLimit, CancellationToken token = default( CancellationToken ) );

		AttributeInfo[] GetAttributes( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< AttributeInfo[] > GetAttributesAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <param name="mark"></param>
		/// <returns>Item quantities.</returns>
		/// <remarks>This is required since <see cref="ItemsService.GetItems"/> returns
		/// only available quantity.</remarks>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		QuantityInfoResponse GetItemQuantities( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );

		Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		ClassificationConfigurationInformation[] GetClassificationConfigurationInformation( Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigurationInformationAsync( Mark mark, CancellationToken token = default( CancellationToken ) );
		StoreInfo GetStoreInfo( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< StoreInfo > GetStoreInfoAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		ImageInfoResponse[] GetImageList( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< ImageInfoResponse[] > GetImageListAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );

		DistributionCenterInfoResponse[] GetShippingInfo( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< DistributionCenterInfoResponse[] > GetShippingInfoAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		VariationInfo GetVariationInfo( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< VariationInfo > GetVariationInfoAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		/// Gets the available quantity.
		/// </summary>
		/// <param name="sku">The sku of the item.</param>
		/// <param name="mark"></param>
		/// <returns>
		/// The Available quantity for the specified sku.
		/// </returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryQuantity"/>
		int GetAvailableQuantity( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );

		Task< int > GetAvailableQuantityAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus, Mark mark, int delayInMs = 5000, CancellationToken token = default( CancellationToken ) );
		Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus, Mark mark, CancellationToken token = default( CancellationToken ) );
		IEnumerable< string > GetAllSkus( Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< IEnumerable< string > > GetAllSkusAsync( Mark mark, CancellationToken token = default( CancellationToken ) );
		IEnumerable< string > GetFilteredSkus( ItemsFilter filter, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< PagedApiResponse< string > > GetFilteredSkusAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark, CancellationToken token = default( CancellationToken ) );
		void SynchItem( InventoryItemSubmit item, Mark mark, bool isCreateNew = false, CancellationToken token = default( CancellationToken ) );
		Task SynchItemAsync( InventoryItemSubmit item, Mark mark, bool isCreateNew = false, CancellationToken token = default( CancellationToken ) );
		Task< DistributionCenter[] > GetDistributionCentersAsync( Mark mark, CancellationToken token = default( CancellationToken ) );
		void SynchItems( IEnumerable< InventoryItemSubmit > items, Mark mark, bool isCreateNew = false, CancellationToken token = default( CancellationToken ) );
		Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items, Mark mark, bool isCreateNew = false, CancellationToken token = default( CancellationToken ) );
		void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark, CancellationToken token = default( CancellationToken ) );
		void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark, CancellationToken token = default( CancellationToken ) );
		void RemoveLabelListFromItemList( string[] labels, IEnumerable< string > skus, string reason, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task RemoveLabelListFromItemListAsync( string[] labels, IEnumerable< string > skus, string reason, Mark mark, CancellationToken token = default( CancellationToken ) );
		void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark, CancellationToken token = default( CancellationToken ) );
		void DeleteItem( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		Task DeleteItemAsync( string sku, Mark mark, CancellationToken token = default( CancellationToken ) );
		ClassificationConfigurationInformation[] GetClassificationConfigInfo( Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigInfoAsync( Mark mark, CancellationToken token = default( CancellationToken ) );

		DistributionCenterResponse[] GetDistributionCenterList( Mark mark, CancellationToken token = default( CancellationToken ) );
		Task< DistributionCenterResponse[] > GetDistributionCenterListAsync( Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		///	This property can be used by the client to monitor the last access library's network activity time.
		/// </summary>
		DateTime LastActivityTime { get; }
	}
}