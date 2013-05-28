using System.Collections.Generic;
using System.Threading.Tasks;
using ChannelAdvisorAccess.InventoryService;

namespace ChannelAdvisorAccess.Services.Items
{
	public interface IItemsService
	{
		/// <summary>
		/// Gets all items in the inventory.
		/// </summary>
		/// <returns>Iterator to go over items 1 order at a time.</returns>
		/// <remarks>The best way to process orders is to use <c>foreach</c></remarks>
		IEnumerable< InventoryItemResponse > GetAllItems();

		/// <summary>
		/// Gets all skus.
		/// </summary>
		/// <returns>All skus for the current account.</returns>
		IEnumerable< string > GetAllSkus();

		/// <summary>
		/// Gets the item attributes.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <returns>Item attributes.</returns>
		AttributeInfo[] GetAttributes( string sku );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns>Items matching supplied filter.</returns>
		IEnumerable< InventoryItemResponse > GetFilteredItems( ItemsFilter filter );

		/// <summary>
		/// Gets the items matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns>Items matching supplied filter.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/GetFilteredInventoryItemList"/>
		Task< IEnumerable< InventoryItemResponse >> GetFilteredItemsAsync( ItemsFilter filter );

		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <returns>Items with specified skus.</returns>
		IEnumerable< InventoryItemResponse > GetItems( string [] skus );

		/// <summary>
		/// Updates the item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <remarks>Only non-default fields will be updated.</remarks>
		void SynchItem( InventoryItemSubmit item );

		/// <summary>
		/// Updates items.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <remarks>Only non-default fields will be updated.</remarks>
		void SynchItems( List< InventoryItemSubmit > items );

		/// <summary>
		/// Updates items.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <remarks>Only non-default fields will be updated.</remarks>
		Task SynchItemsAsync( List< InventoryItemSubmit > items );

		/// <summary>
		/// Deletes the item.
		/// </summary>
		/// <param name="sku">The item sku.</param>
		void DeleteItem( string sku );

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <returns>Item quantities.</returns>
		/// <remarks>This is required since <see cref="ItemsService.GetItems(string[])"/> returns
		/// only available quantity.</remarks>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		QuantityInfoResponse GetItemQuantities ( string sku );

		/// <summary>Gets classifications the configuration information.</summary>
		/// <returns>Classification configuration for the account.</returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetClassificationConfigurationInformation"/>
		ClassificationConfigurationInformation[] GetClassificationConfigurationInformation();

		/// <summary>Gets the store info.</summary>
		/// <param name="sku">The sku.</param>
		/// <returns>Store information for the specified sku.</returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemStoreInfo"/>
		StoreInfo GetStoreInfo( string sku );

		/// <summary>Gets the image list.</summary>
		/// <param name="sku">The sku.</param>
		/// <returns>Array of images related to the specified sku.</returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemImageList"/>
		ImageInfoResponse[] GetImageList( string sku );

		/// <summary>Gets the shipping info.</summary>
		/// <param name="sku">The sku.</param>
		/// <returns>Shipping rates for the specified sku.</returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemShippingInfo"/>
		ShippingRateInfo[] GetShippingInfo( string sku );

		/// <summary>Gets the variation info.</summary>
		/// <param name="sku">The sku.</param>
		/// <returns>Item variation info for the specified sku.</returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemShippingInfo"/>
		VariationInfo GetVariationInfo( string sku );

		/// <summary>
		/// Gets the available quantity.
		/// </summary>
		/// <param name="sku">The sku of the item.</param>
		/// <returns>The Available quantity for the specified sku.</returns>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryQuantity"/>
		int GetAvailableQuantity( string sku );

		/// <summary>
		/// Checks whether the sku exist.
		/// </summary>
		/// <param name="sku">The sku.</param>
		/// <returns><c>true</c> if sku exists; <c>false</c> otherwise.</returns>
		bool DoesSkuExist ( string sku );

		/// <summary>
		/// Verifies if supplied SKUs exist.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <returns>Response with information on whether each sku exists or not.</returns>
		IEnumerable< DoesSkuExistResponse > DoSkusExist( IEnumerable< string > skus );

		/// <summary>
		/// Gets the skus matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns>skus matching supplied filter.</returns>
		IEnumerable< string > GetFilteredSkus ( ItemsFilter filter );

		/// <summary>
		/// Gets the skus matching filter.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns>skus matching supplied filter.</returns>
		Task< IEnumerable< string > > GetFilteredSkusAsync( ItemsFilter filter );

		/// <summary>
		/// Updates both quantity and price on a single item.
		/// </summary>
		/// <param name="itemQuantityAndPrice">The item quantity and price.</param>
		void UpdateQuantityAndPrice( InventoryItemQuantityAndPrice itemQuantityAndPrice );

		/// <summary>
		/// Updates both quantity and price on all supplied items.
		/// </summary>
		/// <param name="itemQuantityAndPrices">The item quantity and prices.</param>
		void UpdateQuantityAndPrices( List< InventoryItemQuantityAndPrice > itemQuantityAndPrices );
		
		/// <summary>
		/// Updates both quantity and price on all supplied items.
		/// </summary>
		/// <param name="itemQuantityAndPrices">The item quantity and prices.</param>
		Task UpdateQuantityAndPricesAsync( List< InventoryItemQuantityAndPrice > itemQuantityAndPrices );

		/// <summary>
		/// Removes the list of Labels from the list of SKUs.
		/// </summary>
		/// <param name="labels">List of Labels (Maximum of 3 Labels)</param>
		/// <param name="skus">List of SKUs</param>
		/// <param name="reason">Specifies the reason for removing the assignment of the Labels</param>
		/// <see href="http://developer.channeladvisor.com/display/cadn/RemoveLabelListFromInventoryItemList"/>
		void RemoveLabelListFromItemList( string[] labels, string[] skus, string reason );

		/// <summary>
		/// Removes the list of Labels from the list of SKUs.
		/// </summary>
		/// <param name="labels">List of Labels (Maximum of 3 Labels)</param>
		/// <param name="skus">List of SKUs</param>
		/// <param name="reason">Specifies the reason for removing the assignment of the Labels</param>
		/// <see href="http://developer.channeladvisor.com/display/cadn/RemoveLabelListFromInventoryItemList"/>
		Task RemoveLabelListFromItemListAsync( string[] labels, string[] skus, string reason );

		/// <summary>
		/// Assigns the list of Labels to the specified list of SKUs.
		/// </summary>
		/// <param name="labels">List of Labels (Maximum of 3 Labels)</param>
		/// <param name="createLabelIfNotExist">Specifies whether or not to create the Label if it does not already exist.</param>
		/// <param name="skus">List of SKUs</param>
		/// <param name="reason">Specifies the reason for the assignment of the Labels</param>
		/// <see href="http://developer.channeladvisor.com/display/cadn/AssignLabelListToInventoryItemList"/>
		void AssignLabelListToItemList( string[] labels, bool createLabelIfNotExist, string[] skus, string reason );

		/// <summary>
		/// Assigns the list of Labels to the specified list of SKUs.
		/// </summary>
		/// <param name="labels">List of Labels (Maximum of 3 Labels)</param>
		/// <param name="createLabelIfNotExist">Specifies whether or not to create the Label if it does not already exist.</param>
		/// <param name="skus">List of SKUs</param>
		/// <param name="reason">Specifies the reason for the assignment of the Labels</param>
		/// <see href="http://developer.channeladvisor.com/display/cadn/AssignLabelListToInventoryItemList"/>
		Task AssignLabelListToItemListAsync( string[] labels, bool createLabelIfNotExist, string[] skus, string reason );

		/// <summary>
		/// Gets the account name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the account id.
		/// </summary>
		string AccountId{ get; }

		/// <summary>
		/// Gets the item available quantity.
		/// </summary>
		/// <param name="sku">The item sku.</param>
		/// <returns>Available quantity for the sku.</returns>
		int GetItemQuantity( string sku );

		/// <summary>
		/// Gets the available quantities for all items specified.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <returns>Collection of item quantities.</returns>
		IEnumerable< InventoryQuantityResponse > GetItemQuantities( IEnumerable< string > skus );

		/// <summary>
		/// Gets the available quantities for all items specified.
		/// </summary>
		/// <param name="skus">The skus.</param>
		/// <returns>Collection of item quantities.</returns>
		Task< IEnumerable< InventoryQuantityResponse > > GetItemQuantitiesAsync( IEnumerable< string > skus );
	}
}