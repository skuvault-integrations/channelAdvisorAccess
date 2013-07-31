using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

		#region Ping
		public void Ping()
		{
			AP.Query.Do( () =>
				{
					var result = this._client.Ping( this._credentials );
					CheckCaSuccess( result );
				} );
		}

		public async Task PingAsync()
		{
			await AP.QueryAsync.Do( async () =>
				{
					var result = await this._client.PingAsync( this._credentials );
					CheckCaSuccess( result.PingResult );
				} );
		}
		#endregion

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

		public async Task< IEnumerable< T > > GetOrdersAsync< T >( DateTime startDate, DateTime endDate )
			where T : OrderResponseItem
		{
			var orderCriteria = new OrderCriteria
				{
					StatusUpdateFilterBeginTimeGMT = startDate,
					StatusUpdateFilterEndTimeGMT = endDate
				};

			return await this.GetOrdersAsync< T >( orderCriteria );
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
			if( string.IsNullOrEmpty( orderCriteria.DetailLevel ) )
				orderCriteria.DetailLevel = "High";

			int pageSize;
			orderCriteria.PageSize = this._pageSizes.TryGetValue( orderCriteria.DetailLevel, out pageSize ) ? pageSize : 20;
			orderCriteria.PageNumberFilter = 1;

			var orders = new List< T >();

			while( true )
			{
				var ordersFromPage = this.GetOrdersPage( orderCriteria );

				if( ordersFromPage == null || ordersFromPage.Length == 0 )
					break;

				orders.AddRange( ordersFromPage.OfType< T >() );
				orderCriteria.PageNumberFilter += 1;
			}
			return orders;
		}

		private OrderResponseItem[] GetOrdersPage( OrderCriteria orderCriteria )
		{
			return AP.Query.Get( () =>
				{
					var results = this._client.GetOrderList( this._credentials, this.AccountId, orderCriteria );
					CheckCaSuccess( results );
					return results.ResultData;
				} );
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="orderCriteria">The order criteria.</param>
		/// <returns>Orders matching supplied criteria.</returns>
		public async Task< IEnumerable< T > > GetOrdersAsync< T >( OrderCriteria orderCriteria )
			where T : OrderResponseItem
		{
			if( string.IsNullOrEmpty( orderCriteria.DetailLevel ) )
				orderCriteria.DetailLevel = "High";

			int pageSize;
			orderCriteria.PageSize = this._pageSizes.TryGetValue( orderCriteria.DetailLevel, out pageSize ) ? pageSize : 20;
			orderCriteria.PageNumberFilter = 1;

			var orders = new List< T >();

			while( true )
			{
				var ordersFromPage = await this.GetOrdersPageAsync( orderCriteria );

				if( ordersFromPage == null || ordersFromPage.Length == 0 )
					break;

				orders.AddRange( ordersFromPage.OfType< T >() );
				orderCriteria.PageNumberFilter += 1;
			}

			return orders;
		}

		private async Task< OrderResponseItem[] > GetOrdersPageAsync( OrderCriteria orderCriteria )
		{
			return await AP.QueryAsync.Get( async () =>
				{
					var results = await this._client.GetOrderListAsync( this._credentials, this.AccountId, orderCriteria );
					CheckCaSuccess( results.GetOrderListResult );
					return results.GetOrderListResult.ResultData;
				} );
		}

		/// <summary>
		/// Submits the order.
		/// </summary>
		/// <param name="orderSubmit">The order submit.</param>
		/// <returns>New order CA id.</returns>
		public int SubmitOrder( OrderSubmit orderSubmit )
		{
			return AP.Submit.Get( () =>
				{
					var apiResults = this._client.SubmitOrder( this._credentials, this.AccountId, orderSubmit );
					this.CheckCaSuccess( apiResults );
					return apiResults.ResultData;
				} );
		}

		public async Task< int > SubmitOrderAsync( OrderSubmit orderSubmit )
		{
			return await AP.SubmitAsync.Get( async () =>
				{
					var apiResults = await this._client.SubmitOrderAsync( this._credentials, this.AccountId, orderSubmit );
					this.CheckCaSuccess( apiResults.SubmitOrderResult );
					return apiResults.SubmitOrderResult.ResultData;
				} );
		}
		#endregion

		public IEnumerable< OrderUpdateResponse > UpdateOrderList( OrderUpdateSubmit[] orderUpdates )
		{
			return AP.Submit.Get( () =>
				{
					var results = this._client.UpdateOrderList( this._credentials, this.AccountId, orderUpdates );
					this.CheckCaSuccess( results );
					return results.ResultData;
				} );
		}

		public async Task< IEnumerable< OrderUpdateResponse > > UpdateOrderListAsync( OrderUpdateSubmit[] orderUpdates )
		{
			return await AP.SubmitAsync.Get( async () =>
				{
					var results = await this._client.UpdateOrderListAsync( this._credentials, this.AccountId, orderUpdates );
					this.CheckCaSuccess( results.UpdateOrderListResult );
					return results.UpdateOrderListResult.ResultData;
				} );
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

		private void CheckCaSuccess( APIResultOfString result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}
	}
}