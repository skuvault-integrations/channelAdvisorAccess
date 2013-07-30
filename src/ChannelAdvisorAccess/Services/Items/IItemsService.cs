using System.Collections.Generic;
using System.Threading.Tasks;
using ChannelAdvisorAccess.InventoryService;

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

		void Ping();
		Task PingAsync();
		bool DoesSkuExist( string sku );
		Task< bool > DoesSkuExistAsync( string sku );
		IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable< string > skus );
		Task< IEnumerable< DoesSkuExistResponse > > DoSkusExistAsync( IEnumerable< string > skus );
		IEnumerable< InventoryItemResponse > GetAllItems();

		/// <summary>
		/// Gets the items by skus.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <returns>Enumerator to process items. <c>null</c> is returned
		/// for non-existing skus.</returns>
		/// <remarks>Items are pulled 1 at a time to handle non-existing skus.
		/// This results in slower performance.</remarks>
		IEnumerable< InventoryItemResponse > GetItems( IEnumerable< string > skus );

		Task< IEnumerable< InventoryItemResponse > > GetItemsAsync( IEnumerable< string > skus );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter );

		AttributeInfo[] GetAttributes( string sku );
		Task< AttributeInfo[] > GetAttributesAsync( string sku );

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <returns>Item quantities.</returns>
		/// <remarks>This is required since <see cref="ItemsService.GetItems"/> returns
		/// only available quantity.</remarks>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		QuantityInfoResponse GetItemQuantities( string sku );

		Task< QuantityInfoResponse > GetItemQuantitiesAsync( string sku );
		ClassificationConfigurationInformation[] GetClassificationConfigurationInformation();
		Task< ClassificationConfigurationInformation[] > GetClassificationConfigurationInformationAsync();
		StoreInfo GetStoreInfo( string sku );
		Task< StoreInfo > GetStoreInfoAsync( string sku );
		ImageInfoResponse[] GetImageList( string sku );
		Task< ImageInfoResponse[] > GetImageListAsync( string sku );
		ShippingRateInfo[] GetShippingInfo( string sku );
		Task< ShippingRateInfo[] > GetShippingInfoAsync( string sku );
		VariationInfo GetVariationInfo( string sku );
		Task< VariationInfo > GetVariationInfoAsync( string sku );

		/// <summary>
		/// Gets the available quantity.
		/// </summary>
		/// <param name="sku">The sku of the item.</param>
		/// <returns>
		/// The Available quantity for the specified sku.
		/// </returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryQuantity"/>
		int GetAvailableQuantity( string sku );

		Task< int > GetAvailableQuantityAsync( string sku );
		IEnumerable< InventoryQuantityResponse > GetAvailableQuantities( IEnumerable< string > skus );
		Task< IEnumerable< InventoryQuantityResponse > > GetAvailableQuantitiesAsync( IEnumerable< string > skus );
		IEnumerable< string > GetAllSkus();
		Task< IEnumerable< string > > GetAllSkusAsync();
		IEnumerable< string > GetFilteredSkus( ItemsFilter filter );
		Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter );
		void SynchItem( InventoryItemSubmit item );
		Task SynchItemAsync( InventoryItemSubmit item );
		void SynchItems( IEnumerable< InventoryItemSubmit > items );
		Task SynchItemsAsync( IEnumerable< InventoryItemSubmit > items );
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
	}
}