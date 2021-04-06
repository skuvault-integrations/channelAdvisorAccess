using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services.Items;

namespace ChannelAdvisorAccess.Services
{
	public static class ChannelAdvisorManagerExtensions
	{
		#region Orders
		public static IEnumerable< T > GetOrders< T >( this IChannelAdvisorManager manager, DateTime start, DateTime end, string accountId, CancellationToken token )
			where T : OrderResponseItem
		{
			var orderService = manager.GetOrdersServiceByAccountId( accountId );
			return orderService.GetOrders< T >( start, end, token );
		}

		/// <summary>
		/// Gets the orders matching supplied criteria.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="manager">Manager through which to get orders.</param>
		/// <param name="orderCriteria">The order criteria.</param>
		/// <param name="accountId">The account id.</param>
		/// <returns>Orders matching supplied criteria.</returns>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/OrderCriteria">OrderCriteria</seealso>
		/// <example>Getting orders updated between specific dates.
		/// <code>
		/// var orderCriteria = new OrderCriteria
		///		{
		///			StatusUpdateFilterBeginTimeGMT = startDate,
		///			StatusUpdateFilterEndTimeGMT = endDate
		///		};
		///	var orders = this.GetOrders&lt; OrderResponseDetailHigh >( orderCriteria );
		/// </code>
		/// </example>
		public static IEnumerable< T > GetOrders< T >( this IChannelAdvisorManager manager, OrderCriteria orderCriteria, string accountId, CancellationToken token )
			where T : OrderResponseItem
		{
			var orderService = manager.GetOrdersServiceByAccountId( accountId );
			return orderService.GetOrders< T >( orderCriteria, token );
		}

		public static IList< T > GetOrdersList< T >( this IChannelAdvisorManager manager, DateTime start, DateTime end, string accountId, CancellationToken token )
			where T : OrderResponseItem
		{
			var orderService = manager.GetOrdersServiceByAccountId( accountId );
			return orderService.GetOrdersList< T >( start, end, token );
		}
		#endregion

		#region Inventory
		/// <summary>
		/// Gets all items in the inventory for the specified account.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <returns>
		/// Collection of all items in the account inventory.
		/// </returns>
		public static IEnumerable< InventoryItemResponse > GetAllItems( this IChannelAdvisorManager manager, string accountId, CancellationToken token )
		{
			var itemService = manager.GetItemsServiceByAccountId( accountId );
			return itemService.GetAllItems( token );
		}

		/// <summary>
		/// Gets the items by skus.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="skus">The skus.</param>
		/// <returns>List of items matching supplied SKUs.</returns>
		public static IEnumerable< InventoryItemResponse > GetItems( this IChannelAdvisorManager manager, string accountId, string[] skus, CancellationToken token )
		{
			var itemService = manager.GetItemsServiceByAccountId( accountId );
			return itemService.GetItems( skus, token );
		}

		/// <summary>
		/// Gets the items using specified filter.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="filter">The filter.</param>
		/// <returns>
		/// Items found using <paramref name="filter"/>
		/// </returns>
		/// <remarks>You can filter by partial SKUs using <see cref="ItemsFilter.Criteria"/>.
		/// See <see href="http://developer.channeladvisor.com/display/cadn/InventoryItemCriteria"/> for more details.</remarks>
		/// <example>Filtering by partial SKU
		/// <code>itemsFilter.Criteria.PartialSku = "partSku";</code></example>
		public static IEnumerable< InventoryItemResponse > GetFilteredItems( this IChannelAdvisorManager manager, string accountId, ItemsFilter filter, CancellationToken token )
		{
			var itemService = manager.GetItemsServiceByAccountId( accountId );
			return itemService.GetFilteredItems( filter, token );
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="sku">The item sku.</param>
		/// <returns>
		/// Returns the item or <c>null</c> if item with specified SKU doesn't exist.
		/// </returns>
		/// <example>Get single item by SKU.
		/// <code>
		/// var item = ChannelAdvisorFacade.GetItem( "account id", "sku to get" );
		/// </code></example>
		public static InventoryItemResponse GetItem( this IChannelAdvisorManager manager, string accountId, string sku, CancellationToken token )
		{
			var items = GetItems( manager, accountId, new[] { sku }, token );
			return items.FirstOrDefault();
		}

		/// <summary>
		/// Gets the item attributes.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="sku">The sku.</param>
		/// <returns>Item attributes collection.</returns>
		public static AttributeInfo[] GetAttributes( this IChannelAdvisorManager manager, string accountId, string sku, CancellationToken token )
		{
			var itemsService = manager.GetItemsServiceByAccountId( accountId );
			return itemsService.GetAttributes( sku, token ); 
		}

		/// <summary>
		/// Gets the additional item quantities.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="sku">The sku.</param>
		/// <returns>Item quantities.</returns>
		/// <remarks>This is required since <see cref="GetItems(IChannelAdvisorManager,string,string[])"/> returns
		/// only available quantity.</remarks>
		/// <see href="http://developer.channeladvisor.com/display/cadn/GetInventoryItemQuantityInfo"/>
		public static QuantityInfoResponse GetItemQuantities( this IChannelAdvisorManager manager, string accountId, string sku, CancellationToken token )
		{
			var itemsService = manager.GetItemsServiceByAccountId( accountId );
			return itemsService.GetItemQuantities( sku, token );
		}

		/// <summary>
		/// Creates or update the items.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="items">The items to add / update.</param>
		/// <remarks>If SKU exists, item is updated, otherwise it's created.
		/// <para>For update, populate only properties you want updated. Other properties remain unchanged.</para></remarks>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/SynchInventoryItemList"/>
		public static void SynchItems( this IChannelAdvisorManager manager, string accountId, List< InventoryItemSubmit > items, CancellationToken token )
		{
			var itemService = manager.GetItemsServiceByAccountId( accountId );
			itemService.SynchItems( items, token );
		}

		/// <summary>
		/// Creates or update an item.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="item">An item to add / update.</param>
		/// <remarks>If SKU exists, item is updated, otherwise it's created.
		/// <para>For update, populate only properties you want updated. Other properties remain unchanged.</para></remarks>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/SynchInventoryItemList"/>
		public static void SynchItem( this IChannelAdvisorManager manager, string accountId, InventoryItemSubmit item, CancellationToken token )
		{
			var itemService = manager.GetItemsServiceByAccountId( accountId );
			itemService.SynchItem( item, token );
		}

		/// <summary>
		/// Updates the quantity and price.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="itemQuantityAndPrice">The item quantity and price.</param>
		public static void UpdateQuantityAndPrice( this IChannelAdvisorManager manager, string accountId, InventoryItemQuantityAndPrice itemQuantityAndPrice, CancellationToken token )
		{
			var itemService = manager.GetItemsServiceByAccountId( accountId );
			itemService.UpdateQuantityAndPrice( itemQuantityAndPrice, token );
		}

		/// <summary>
		/// Updates the quantity and prices on all supplied items.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="itemQuantityAndPrice">The item quantity and price.</param>
		public static void UpdateQuantityAndPrices( this IChannelAdvisorManager manager, string accountId, List< InventoryItemQuantityAndPrice > itemQuantityAndPrice, CancellationToken token )
		{
			var itemService = manager.GetItemsServiceByAccountId( accountId );
			itemService.UpdateQuantityAndPrices( itemQuantityAndPrice, token );
		}

		/// <summary>
		/// Deletes the item.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="sku">The sku of the item to delete.</param>
		public static void DeleteItem( this IChannelAdvisorManager manager, string accountId, string sku, CancellationToken token )
		{
			var itemService = manager.GetItemsServiceByAccountId( accountId );
			itemService.DeleteItem( sku, token );
		}
		#endregion

		#region Shipping
		/// <summary>
		/// Marks the order shipped.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="accountId">The account ID.</param>
		/// <param name="orderId">The order ID.</param>
		/// <param name="carrierCode">The carrier code.</param>
		/// <param name="classCode">The class code.</param>
		/// <param name="trackingNumber">The tracking number.</param>
		/// <param name="dateShipped">The date shipped when order was shipped (will be converted to UTC).</param>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/OrderShipped"/>
		public static void MarkOrderShipped( this IChannelAdvisorManager manager, string accountId, int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			var shippingService = manager.GetShippingServiceByAccountId( accountId );
			shippingService.MarkOrderShipped( orderId, carrierCode, classCode, trackingNumber, dateShipped );
		}
		#endregion
	}
}