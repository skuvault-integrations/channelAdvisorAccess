using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services.Items;
using Netco.Extensions;

namespace ChannelAdvisorAccess.Services.Orders
{
	/// <summary>
	/// Facade to work with CA orders.
	/// </summary>
	public class OrdersService: ServiceBaseAbstr, IOrdersService
	{
		private readonly APICredentials _credentials;
		private readonly OrderServiceSoapClient _client;

		private const int MaxUnexpectedAttempt = 5;

		public string AccountId{ get; private set; }

		public string Name{ get; private set; }

		public OrdersService( APICredentials credentials, string accountName, string accountId, ObjectCache cache = null ): this( credentials, accountId, cache )
		{
			this.Name = accountName;
		}

		public OrdersService( APICredentials credentials, string accountId, ObjectCache cache = null )
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

				// If you get message code = 1 (Unexpected)
				if( results.MessageCode == 1 )
					resultData = this.HandleErrorUnexpected( orderCriteria );

				return resultData;
			} );
		}

		private OrderResponseItem[] HandleErrorUnexpected( OrderCriteria orderCriteria, [ CallerMemberName ] string callerMemberName = "" )
		{
			var result = new List< OrderResponseItem >();
			var prevPageSize = orderCriteria.PageSize;
			var prevPageNumber = orderCriteria.PageNumberFilter;
			var pageNumberBy1 = prevPageSize * ( prevPageNumber - 1 );
			for( var i = 1; i <= prevPageSize; i++ )
			{
				orderCriteria.PageSize = 1;
				orderCriteria.PageNumberFilter = pageNumberBy1 + i;
				var numberAttempt = 0;
				while( numberAttempt < MaxUnexpectedAttempt )
				{
					var answer = this._client.GetOrderList( this._credentials, this.AccountId, orderCriteria );
					if( answer.Status == ResultStatus.Success )
					{
						result.AddRange( answer.ResultData );
						break;
					}
					
					numberAttempt++;
					this.LogUnexpectedError( orderCriteria, answer, pageNumberBy1 + i, callerMemberName, numberAttempt );
					this.DoDelayUnexpectedAsync( new TimeSpan( 0, 2, 0 ), numberAttempt ).Wait();
				}
			}

			orderCriteria.PageSize = prevPageSize;
			orderCriteria.PageNumberFilter = prevPageNumber;

			return result.ToArray();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="orderCriteria">The order criteria.</param>
		/// <param name="mark"></param>
		/// <returns>Orders matching supplied criteria.</returns>
		public async Task< IEnumerable< T > > GetOrdersAsync< T >( OrderCriteria orderCriteria, Mark mark = null )
			where T : OrderResponseItem
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : orderCriteria.ToJson() ) );

				if( string.IsNullOrEmpty( orderCriteria.DetailLevel ) )
					orderCriteria.DetailLevel = "High";

				int pageSize;
				orderCriteria.PageSize = this._pageSizes.TryGetValue( orderCriteria.DetailLevel, out pageSize ) ? pageSize : 20;
				orderCriteria.PageNumberFilter = 1;

				var orders = new List< T >();
				while( true )
				{
					var ordersFromPage = await this.GetOrdersPageAsync( orderCriteria, mark ).ConfigureAwait( false );

					if( ordersFromPage == null || ordersFromPage.Length == 0 )
						break;

					orders.AddRange( ordersFromPage.OfType< T >() );
					orderCriteria.PageNumberFilter += 1;
				}

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : orders.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : orderCriteria.ToJson() ) );
				return orders;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : orderCriteria.ToJson() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		private async Task< OrderResponseItem[] > GetOrdersPageAsync( OrderCriteria orderCriteria, Mark mark = null )
		{
			return await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo, mark : mark ) ).Get( async () =>
			{
				var results = await this._client.GetOrderListAsync( this._credentials, this.AccountId, orderCriteria ).ConfigureAwait( false );
				CheckCaSuccess( results.GetOrderListResult );
				var resultData = results.GetOrderListResult.ResultData ?? new OrderResponseItem[ 0 ];

				// If you get message code = 1 (Unexpected)
				if( results.GetOrderListResult.MessageCode == 1 )
					resultData = await this.HandleErrorUnexpectedAsync( orderCriteria ).ConfigureAwait( false );

				return resultData;
			} ).ConfigureAwait( false );
		}

		private async Task< OrderResponseItem[] > HandleErrorUnexpectedAsync( OrderCriteria orderCriteria, [ CallerMemberName ] string callerMemberName = "" )
		{
			var result = new List< OrderResponseItem >();
			var prevPageSize = orderCriteria.PageSize;
			var prevPageNumber = orderCriteria.PageNumberFilter;
			var pageNumberBy1 = prevPageSize * ( prevPageNumber - 1 );
			for( var i = 1; i <= prevPageSize; i++ )
			{
				orderCriteria.PageSize = 1;
				orderCriteria.PageNumberFilter = pageNumberBy1 + i;
				var numberAttempt = 0;
				while( numberAttempt < MaxUnexpectedAttempt )
				{
					var answer = await this._client.GetOrderListAsync( this._credentials, this.AccountId, orderCriteria ).ConfigureAwait( false );
					if( answer.GetOrderListResult.Status == ResultStatus.Success )
					{
						result.AddRange( answer.GetOrderListResult.ResultData );
						break;
					}

					numberAttempt++;
					this.LogUnexpectedError( orderCriteria, answer.GetOrderListResult, orderCriteria.PageNumberFilter, callerMemberName, numberAttempt );
					await this.DoDelayUnexpectedAsync( new TimeSpan( 0, 2, 0 ), numberAttempt ).ConfigureAwait( false );
				}
			}

			orderCriteria.PageSize = prevPageSize;
			orderCriteria.PageNumberFilter = prevPageNumber;

			return result.ToArray();
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

		private void LogUnexpectedError( OrderCriteria orderCriteria, APIResultOfArrayOfOrderResponseItem orderList, int? pageNumberBy1, string callerMemberName, int attempt )
		{
			var additionalLogInfo = ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo, callerMemberName )();
			var criteria = string.Format( "StatusUpdateFilterBeginTimeGMTField: {0}. StatusUpdateFilterEndTimeGMTField: {1}. Number in sequence for pageSize equal 1 : {2}", orderCriteria.StatusUpdateFilterBeginTimeGMT.GetValueOrDefault(), orderCriteria.StatusUpdateFilterEndTimeGMT.GetValueOrDefault(), pageNumberBy1 );
			var message = "Unexpected Error. Attempt: {0}. Additional info: {1} {2}".FormatWith( attempt, additionalLogInfo, criteria );
			var exception = new ChannelAdvisorException( orderList.MessageCode, message );
			ChannelAdvisorLogger.LogTraceException( exception );
		}

		public async Task DoDelayUnexpectedAsync( TimeSpan time, int attempt )
		{
			ChannelAdvisorLogger.LogTrace( string.Format( @"Wait by reason of error 1 (Unexpected) {0} minute(s). Attempt: {1}", time.Minutes, attempt ) );
			await Task.Delay( time ).ConfigureAwait( false );
		}
	}
}