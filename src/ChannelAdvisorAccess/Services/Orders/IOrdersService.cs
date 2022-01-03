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

		void Ping( Mark mark = null );

		Task PingAsync( Mark mark = null );

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Iterator to go over orders 1 order at a time.</returns>
		/// <remarks>The best way to process orders is to use <c>foreach</c></remarks>
		IEnumerable< T > GetOrders< T >( DateTime startDate, DateTime endDate, CancellationToken token, Mark mark = null )
			where T : OrderResponseItem;

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Iterator to go over orders 1 order at a time.</returns>
		/// <remarks>The best way to process orders is to use <c>foreach</c></remarks>
		Task< IEnumerable< T > > GetOrdersAsync< T >( DateTime startDate, DateTime endDate, CancellationToken token, Mark mark = null )
			where T : OrderResponseItem;

		/// <summary>
		/// Gets the orders and returns them in a list.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Downloads all orders matching the date and returns them in a list.</returns>
		IList< T > GetOrdersList< T >( DateTime startDate, DateTime endDate, CancellationToken token, Mark mark = null )
			where T : OrderResponseItem;

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="orderCriteria">The order criteria.</param>
		/// <returns>Orders matching supplied criteria.</returns>
		IEnumerable< T > GetOrders< T >( OrderCriteria orderCriteria, CancellationToken token, Mark mark = null )
			where T : OrderResponseItem;

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="orderCriteria">The order criteria.</param>
		/// <param name="mark"></param>
		/// <returns>Orders matching supplied criteria.</returns>
		Task< IEnumerable< T > > GetOrdersAsync< T >( OrderCriteria orderCriteria, CancellationToken token, Mark mark = null )
			where T : OrderResponseItem;

		/// <summary>Updates the order list.</summary>
		/// <param name="orderUpdates">The order updates.</param>
		/// <returns>Result of updates.</returns>
		IEnumerable< OrderUpdateResponse > UpdateOrderList( OrderUpdateSubmit[] orderUpdates, CancellationToken token, Mark mark = null );

		Task< IEnumerable< OrderUpdateResponse > > UpdateOrderListAsync( OrderUpdateSubmit[] orderUpdates, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Submits the order.
		/// </summary>
		/// <param name="orderSubmit">The order submit.</param>
		/// <returns>New order CA id.</returns>
		int SubmitOrder( OrderSubmit orderSubmit, CancellationToken token, Mark mark = null );

		Task< int > SubmitOrderAsync( OrderSubmit orderSubmit, CancellationToken token, Mark mark = null );

		/// <summary>
		///	This property can be used by the client to monitor the last access library's network activity time.
		/// </summary>
		DateTime LastActivityTime { get; }
	}
}