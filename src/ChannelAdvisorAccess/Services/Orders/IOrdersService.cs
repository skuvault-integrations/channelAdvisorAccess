using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.OrderService;

namespace ChannelAdvisorAccess.Services.Orders
{
	public interface IOrdersService : IDisposable
	{
		/// <summary>
		/// Gets the account name.
		/// </summary>
		string Name{ get; }

		Func< string > AdditionalLogInfo{ get; set; }

		/// <summary>
		/// Gets the account id.
		/// </summary>
		string AccountId{ get; }

		void Ping( Mark mark, CancellationToken token = default( CancellationToken ) );

		Task PingAsync( Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Iterator to go over orders 1 order at a time.</returns>
		/// <remarks>The best way to process orders is to use <c>foreach</c></remarks>
		IEnumerable< T > GetOrders< T >( DateTime startDate, DateTime endDate, Mark mark, CancellationToken token = default( CancellationToken ) )
			where T : OrderResponseItem;

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Iterator to go over orders 1 order at a time.</returns>
		/// <remarks>The best way to process orders is to use <c>foreach</c></remarks>
		Task< IEnumerable< T > > GetOrdersAsync< T >( DateTime startDate, DateTime endDate, Mark mark, CancellationToken token = default( CancellationToken ) )
			where T : OrderResponseItem;

		/// <summary>
		/// Get orders by orderIds
		/// </summary>
		/// <param name="orderIDs"></param>
		/// <param name="mark"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task< IEnumerable< OrderResponseDetailLow > > GetOrdersByIdsAsync( int[] orderIDs, Mark mark, CancellationToken token );
 
		/// <summary>
		/// Gets the orders and returns them in a list.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Downloads all orders matching the date and returns them in a list.</returns>
		IList< T > GetOrdersList< T >( DateTime startDate, DateTime endDate, Mark mark, CancellationToken token = default( CancellationToken ) )
			where T : OrderResponseItem;

		/// <summary>Updates the order list.</summary>
		/// <param name="orderUpdates">The order updates.</param>
		/// <returns>Result of updates.</returns>
		IEnumerable< OrderUpdateResponse > UpdateOrderList( OrderUpdateSubmit[] orderUpdates, Mark mark, CancellationToken token = default( CancellationToken ) );

		Task< IEnumerable< OrderUpdateResponse > > UpdateOrderListAsync( OrderUpdateSubmit[] orderUpdates, Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		/// Submits the order.
		/// </summary>
		/// <param name="orderSubmit">The order submit.</param>
		/// <returns>New order CA id.</returns>
		int SubmitOrder( OrderSubmit orderSubmit, Mark mark, CancellationToken token = default( CancellationToken ) );

		Task< int > SubmitOrderAsync( OrderSubmit orderSubmit, Mark mark, CancellationToken token = default( CancellationToken ) );

		/// <summary>
		///	This property can be used by the client to monitor the last access library's network activity time.
		/// </summary>
		DateTime LastActivityTime { get; }
	}
}