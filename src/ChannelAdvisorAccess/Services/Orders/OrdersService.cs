using System;
using System.Collections.Generic;
using System.Linq;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.OrderService;

namespace ChannelAdvisorAccess.Services.Orders
{
	/// <summary>
	/// Facade to work with CA orders.
	/// </summary>
	public class OrdersService : IOrdersService
	{
		private readonly APICredentials _credentials;
		private readonly OrderServiceSoapClient _client;
		public string AccountId { get; private set; }

		public string Name { get; private set; }

		public OrdersService( APICredentials credentials, string accountName, string accountId ) : this( credentials, accountId )
		{
			this.Name = accountName;
		}

		public OrdersService( APICredentials credentials, string accountId )
		{
			this._credentials = credentials;
			this.AccountId = accountId;
			this._client = new OrderServiceSoapClient();
		}

		#region API methods
		public IEnumerable< T > GetOrders< T >( DateTime startDate, DateTime endDate )
			where T : OrderResponseItem
		{
			var orderCriteria = new OrderCriteria
			                    	{
			                    		StatusUpdateFilterBeginTimeGMT = startDate,
			                    		StatusUpdateFilterEndTimeGMT = endDate
			                    	};

			return this.GetOrders< T >( orderCriteria );
		}

		/// <summary>
		/// Gets the orders and returns them in a list.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns>Downloads all orders matching the date and returns them in a list.</returns>
		public IList< T > GetOrdersList< T >( DateTime startDate, DateTime endDate )
			where T : OrderResponseItem
		{
			return this.GetOrders< T >( startDate, endDate ).ToList();
		}

		private readonly Dictionary< string, int > _pageSizes = new Dictionary< string, int >
			{
				{ "Low", 200 },
				{ "Medium", 100 },
				{ "High", 50 },
				{ "Complete", 20 },
			};

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="orderCriteria">The order criteria.</param>
		/// <returns>Orders matching supplied criteria.</returns>
		public IEnumerable< T > GetOrders< T >( OrderCriteria orderCriteria )
			where T : OrderResponseItem
		{
			if( string.IsNullOrEmpty(  orderCriteria.DetailLevel) )
				orderCriteria.DetailLevel = "High";

			orderCriteria.PageSize = _pageSizes[ orderCriteria.DetailLevel ];
			orderCriteria.PageNumberFilter = 0;

			while( true )
			{
				var orders = ActionPolicies.CaGetPolicy.Get( () => this.GetNextOrdersPage( orderCriteria ) );

				if( orders == null )
					yield break;

				foreach( var order in orders )
				{
					var orderT = order as T;
					if( orderT != null )
						yield return orderT;
				}

				if( orders.Length == 0 ) //|| orders.Length < orderCriteria.PageSize )
					yield break;
			}
		}

		/// <summary>
		/// Submits the order.
		/// </summary>
		/// <param name="orderSubmit">The order submit.</param>
		/// <returns>New order CA id.</returns>
		public int SubmitOrder( OrderSubmit orderSubmit )
		{
			var results = ActionPolicies.CaSubmitPolicy.Get( () => this.InternalSubmitOrder( orderSubmit ) );
			return results.ResultData;
		}

		private APIResultOfInt32 InternalSubmitOrder( OrderSubmit orderSubmit )
		{
			var results = this._client.SubmitOrder( this._credentials, this.AccountId, orderSubmit );
			this.CheckCaSuccess( results );
			return results;
		}
		#endregion

		public IEnumerable< OrderUpdateResponse > UpdateOrderList( OrderUpdateSubmit[] orderUpdates )
		{
			return ActionPolicies.CaSubmitPolicy.Get( () => 
				{
					var results = this._client.UpdateOrderList( _credentials, AccountId, orderUpdates );
					this.CheckCaSuccess( results );
					return results.ResultData;
				});
		}
		
		private OrderResponseItem[] GetNextOrdersPage( OrderCriteria orderCriteria )
		{
			orderCriteria.PageNumberFilter += 1;

			var orderList = this._client.GetOrderList( this._credentials, this.AccountId, orderCriteria );
			CheckCaSuccess( orderList );

			return orderList.ResultData;
		}

		private static void CheckCaSuccess( APIResultOfArrayOfOrderResponseItem orderList )
		{
			if( orderList.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( orderList.MessageCode, orderList.Message );
		}

		private void CheckCaSuccess( APIResultOfInt32 results )
		{
			if( results.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( results.MessageCode, results.Message );
		}

		private void CheckCaSuccess( APIResultOfArrayOfOrderUpdateResponse results )
		{
			if( results.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( results.MessageCode, results.Message );
		}
	}
}