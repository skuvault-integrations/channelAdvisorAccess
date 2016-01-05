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
	public class OrdersService: IOrdersService
	{
		private readonly APICredentials _credentials;
		private readonly OrderServiceSoapClient _client;
		public string AccountId{ get; private set; }

		private Func< string > _additionalLogInfo;

		public Func< string > AdditionalLogInfo
		{
			get { return this._additionalLogInfo ?? ( () => string.Empty ); }
			set { this._additionalLogInfo = value; }
		}

		public string Name{ get; private set; }

		public OrdersService( APICredentials credentials, string accountName, string accountId ): this( credentials, accountId )
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
			AP.CreateQuery( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
			{
				var result = this._client.Ping( this._credentials );
				this.CheckCaSuccess( result );
			} );
		}

		public async Task PingAsync()
		{
			await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
			{
				var result = await this._client.PingAsync( this._credentials ).ConfigureAwait( false );
				this.CheckCaSuccess( result.PingResult );
			} ).ConfigureAwait( false );
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

			return await this.GetOrdersAsync< T >( orderCriteria ).ConfigureAwait( false );
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
			return AP.CreateQuery( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
			{
				var results = this._client.GetOrderList( this._credentials, this.AccountId, orderCriteria );
				CheckCaSuccess( results );
				var resultData = results.ResultData ?? new OrderResponseItem[ 0 ];
				if( results.MessageCode == 1 )
				{
					var newResult = new List< OrderResponseItem >();
					var prevPageSize = orderCriteria.PageSize;
					var prevPageNumber = orderCriteria.PageNumberFilter;
					var pageNumberBy1 = prevPageSize * ( prevPageNumber - 1 );
					for( var i = 1; i <= prevPageSize; i++ )
					{
						orderCriteria.PageSize = 1;
						orderCriteria.PageNumberFilter = pageNumberBy1 + i;
						var answer = this._client.GetOrderList( this._credentials, this.AccountId, orderCriteria );
						if( answer.Status == ResultStatus.Success )
							newResult.AddRange( answer.ResultData );
					}

					orderCriteria.PageSize = prevPageSize;
					orderCriteria.PageNumberFilter = prevPageNumber;

					resultData = newResult.ToArray();
				}

				return resultData;
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
				var ordersFromPage = await this.GetOrdersPageAsync( orderCriteria ).ConfigureAwait( false );

				if( ordersFromPage == null || ordersFromPage.Length == 0 )
					break;

				orders.AddRange( ordersFromPage.OfType< T >() );
				orderCriteria.PageNumberFilter += 1;
			}

			return orders;
		}

		private async Task< OrderResponseItem[] > GetOrdersPageAsync( OrderCriteria orderCriteria )
		{
			return await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
			{
				var results = await this._client.GetOrderListAsync( this._credentials, this.AccountId, orderCriteria ).ConfigureAwait( false );
				CheckCaSuccess( results.GetOrderListResult );
				var resultData = results.GetOrderListResult.ResultData ?? new OrderResponseItem[ 0 ];
				if( results.GetOrderListResult.MessageCode == 1 )
				{
					var newResult = new List< OrderResponseItem >();
					var prevPageSize = orderCriteria.PageSize;
					var prevPageNumber = orderCriteria.PageNumberFilter;
					var pageNumberBy1 = prevPageSize * ( prevPageNumber - 1 );
					for( var i = 1; i <= prevPageSize; i++ )
					{
						orderCriteria.PageSize = 1;
						orderCriteria.PageNumberFilter = pageNumberBy1 + i;
						var answer = await this._client.GetOrderListAsync( this._credentials, this.AccountId, orderCriteria );
						if( answer.GetOrderListResult.Status == ResultStatus.Success )
							newResult.AddRange( answer.GetOrderListResult.ResultData );
					}

					orderCriteria.PageSize = prevPageSize;
					orderCriteria.PageNumberFilter = prevPageNumber;

					resultData = newResult.ToArray();
				}

				return resultData;
			} ).ConfigureAwait( false );
		}

		/// <summary>
		/// Submits the order.
		/// </summary>
		/// <param name="orderSubmit">The order submit.</param>
		/// <returns>New order CA id.</returns>
		public int SubmitOrder( OrderSubmit orderSubmit )
		{
			return AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
			{
				var apiResults = this._client.SubmitOrder( this._credentials, this.AccountId, orderSubmit );
				this.CheckCaSuccess( apiResults );
				return apiResults.ResultData;
			} );
		}

		public async Task< int > SubmitOrderAsync( OrderSubmit orderSubmit )
		{
			return await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
			{
				var apiResults = await this._client.SubmitOrderAsync( this._credentials, this.AccountId, orderSubmit ).ConfigureAwait( false );
				this.CheckCaSuccess( apiResults.SubmitOrderResult );
				return apiResults.SubmitOrderResult.ResultData;
			} ).ConfigureAwait( false );
		}
		#endregion

		public IEnumerable< OrderUpdateResponse > UpdateOrderList( OrderUpdateSubmit[] orderUpdates )
		{
			return AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
			{
				var results = this._client.UpdateOrderList( this._credentials, this.AccountId, orderUpdates );
				this.CheckCaSuccess( results );
				return results.ResultData;
			} );
		}

		public async Task< IEnumerable< OrderUpdateResponse > > UpdateOrderListAsync( OrderUpdateSubmit[] orderUpdates )
		{
			return await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
			{
				var results = await this._client.UpdateOrderListAsync( this._credentials, this.AccountId, orderUpdates ).ConfigureAwait( false );
				this.CheckCaSuccess( results.UpdateOrderListResult );
				return results.UpdateOrderListResult.ResultData;
			} ).ConfigureAwait( false );
		}

		private static void CheckCaSuccess( APIResultOfArrayOfOrderResponseItem orderList )
		{
			if( orderList.Status != ResultStatus.Success )
			{
				if( orderList.MessageCode != 1 )
					throw new ChannelAdvisorException( orderList.MessageCode, orderList.Message );
			}
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