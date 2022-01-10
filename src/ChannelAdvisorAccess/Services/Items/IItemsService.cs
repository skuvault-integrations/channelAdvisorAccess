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

		void Ping( Mark mark, CancellationToken token );
		Func< string > AdditionalLogInfo{ get; set; }
		Task PingAsync( Mark mark, CancellationToken token );
		bool DoesSkuExist( string sku, Mark mark, CancellationToken token );
		Task< bool > DoesSkuExistAsync( string sku, Mark mark, CancellationToken token );
		IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable< string > skus, Mark mark, CancellationToken token );
		Task< IEnumerable< DoesSkuExistResponse > > DoSkusExistAsync( IEnumerable< string > skus, Mark mark, CancellationToken token );
		IEnumerable< InventoryItemResponse > GetAllItems( Mark mark, CancellationToken token );

		/// <summary>
		/// Gets the items by skus.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <param name="mark"></param>
		/// <returns>Enumerator to process items. <c>null</c> is returned
		/// for non-existing skus.</returns>
		/// <remarks>Items are pulled 1 at a time to handle non-existing skus.
		/// This results in slower performance.</remarks>
		IEnumerable< InventoryItemResponse > GetItems( IEnumerable< string > skus, Mark mark, CancellationToken token );

		Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable< string > skus, Mark mark, CancellationToken token );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter, Mark mark, CancellationToken token );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, Mark mark, CancellationToken token );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="startPage">The first page number to query.</param>
		/// <param name="pageLimit">The max number of pages to query.</param>
		/// <param name="mark"></param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< PagedApiResponse< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, int startPage, Mark mark, int pageLimit, CancellationToken token );

		AttributeInfo[] GetAttributes( string sku, Mark mark, CancellationToken token );
		Task< AttributeInfo[] > GetAttributesAsync( string sku, Mark mark, CancellationToken token );

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <param name="mark"></param>
		/// <returns>Item quantities.</returns>
		/// <remarks>This is required since <see cref="ItemsService.GetItems"/> returns
		/// only available quantity.</remarks>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		QuantityInfoResponse GetItemQuantities( string sku, Mark mark, CancellationToken token );

		Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku, Mark mark, CancellationToken token );
		ClassificationConfigurationInformation[] GetClassificationConfigurationInformation( Mark mark, CancellationToken token );
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigurationInformationAsync( Mark mark, CancellationToken token );
		StoreInfo GetStoreInfo( string sku, Mark mark, CancellationToken token );
		Task< StoreInfo > GetStoreInfoAsync( string sku, Mark mark, CancellationToken token );
		ImageInfoResponse[] GetImageList( string sku, Mark mark, CancellationToken token );
		Task< ImageInfoResponse[] > GetImageListAsync( string sku, Mark mark, CancellationToken token );

		DistributionCenterInfoResponse[] GetShippingInfo( string sku, Mark mark, CancellationToken token );
		Task< DistributionCenterInfoResponse[] > GetShippingInfoAsync( string sku, Mark mark, CancellationToken token );
		VariationInfo GetVariationInfo( string sku, Mark mark, CancellationToken token );
		Task< VariationInfo > GetVariationInfoAsync( string sku, Mark mark, CancellationToken token );

		/// <summary>
		/// Gets the available quantity.
		/// </summary>
		/// <param name="sku">The sku of the item.</param>
		/// <param name="mark"></param>
		/// <returns>
		/// The Available quantity for the specified sku.
		/// </returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryQuantity"/>
		int GetAvailableQuantity( string sku, Mark mark, CancellationToken token );

		Task< int > GetAvailableQuantityAsync( string sku, Mark mark, CancellationToken token );
		IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus, Mark mark, int delatInMs, CancellationToken token );
		Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus, Mark mark, CancellationToken token );
		IEnumerable< string > GetAllSkus( Mark mark, CancellationToken token );
		Task< IEnumerable< string > > GetAllSkusAsync( Mark mark, CancellationToken token );
		IEnumerable< string > GetFilteredSkus( ItemsFilter filter, Mark mark, CancellationToken token );
		Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter, Mark mark, CancellationToken token );
		Task< PagedApiResponse< string > > GetFilteredSkusAsync( ItemsFilter filter, int startPage, int pageLimit, Mark mark, CancellationToken token );
		void SynchItem( InventoryItemSubmit item, Mark mark, bool isCreateNew, CancellationToken token );
		Task SynchItemAsync( InventoryItemSubmit item, Mark mark, bool isCreateNew, CancellationToken token );
		Task< DistributionCenter[] > GetDistributionCentersAsync( Mark mark, CancellationToken token );
		void SynchItems( IEnumerable< InventoryItemSubmit > items, Mark mark, bool isCreateNew, CancellationToken token );
		Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items, Mark mark, bool isCreateNew, CancellationToken token );
		void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark, CancellationToken token );
		Task UpdateQuantityAndPriceAsync( InventoryItemQuantityAndPrice itemQuantityAndPrice, Mark mark, CancellationToken token );
		void UpdateQuantityAndPrices( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark, CancellationToken token );
		Task UpdateQuantityAndPricesAsync( IEnumerable< InventoryItemQuantityAndPrice > itemQuantityAndPrices, Mark mark, CancellationToken token );
		void RemoveLabelListFromItemList( string[] labels, IEnumerable< string > skus, string reason, Mark mark, CancellationToken token );
		Task RemoveLabelListFromItemListAsync( string[] labels, IEnumerable< string > skus, string reason, Mark mark, CancellationToken token );
		void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark, CancellationToken token );
		Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, IEnumerable< string > skus, string reason, Mark mark, CancellationToken token );
		void DeleteItem( string sku, Mark mark, CancellationToken token );
		Task DeleteItemAsync( string sku, Mark mark, CancellationToken token );
		ClassificationConfigurationInformation[] GetClassificationConfigInfo( Mark mark, CancellationToken token );
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigInfoAsync( Mark mark, CancellationToken token );

		DistributionCenterResponse[] GetDistributionCenterList( Mark mark, CancellationToken token );
		Task< DistributionCenterResponse[] > GetDistributionCenterListAsync( Mark mark, CancellationToken token );

		/// <summary>
		///	This property can be used by the client to monitor the last access library's network activity time.
		/// </summary>
		DateTime LastActivityTime { get; }
	}
}