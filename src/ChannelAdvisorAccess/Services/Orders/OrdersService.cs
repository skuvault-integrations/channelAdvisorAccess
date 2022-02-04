using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading;
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
		private readonly FulfillmentService.APICredentials _fulfillmentServiceCredentials;
		private readonly FulfillmentService.FulfillmentServiceSoapClient _fulfillmentServiceClient;

		private const int MaxUnexpectedAttempt = 5;

		public string AccountId{ get; private set; }

		public string Name{ get; private set; }

		public DateTime LastActivityTime
		{
			get
			{
				return base.LastNetworkActivityTime;
			}
		}

		public OrdersService( APICredentials credentials, string accountName, string accountId, ObjectCache cache = null ): this( credentials, accountId, cache )
		{
			this.Name = accountName;
		}

		public OrdersService( APICredentials credentials, string accountId, ObjectCache cache = null )
		{
			this._credentials = credentials;
			this.AccountId = accountId;
			this._client = new OrderServiceSoapClient();
			
			this._fulfillmentServiceCredentials = new FulfillmentService.APICredentials { DeveloperKey = this._credentials.DeveloperKey, Password = this._credentials.Password };
			this._fulfillmentServiceClient = new FulfillmentService.FulfillmentServiceSoapClient();			
		}

		#region Ping
		public void Ping( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{	
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				AP.CreateQuery( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var result = this._client.Ping( this._credentials );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( result );
				} );				

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task PingAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var result = await this._client.PingAsync( this._credentials ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( result.PingResult );
				} ).ConfigureAwait( false );				

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}
		#endregion

		#region API methods
		public IEnumerable< T > GetOrders< T >( DateTime startDate, DateTime endDate, CancellationToken token, Mark mark = null )
			where T : OrderResponseItem
		{
			var orderCriteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = startDate,
				StatusUpdateFilterEndTimeGMT = endDate
			};

			return this.GetOrders< T >( orderCriteria, token, mark );
		}

		public async Task< IEnumerable< T > > GetOrdersAsync< T >( DateTime startDate, DateTime endDate, CancellationToken token, Mark mark = null )
			where T : OrderResponseItem
		{
			var orderCriteria = new OrderCriteria
			{
				StatusUpdateFilterBeginTimeGMT = startDate,
				StatusUpdateFilterEndTimeGMT = endDate
			};

			return await this.GetOrdersAsync< T >( orderCriteria, CancellationToken.None, mark ).ConfigureAwait( false );
		}

		/// <summary>
		/// Gets the orders and returns them in a list.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="mark">Session Mark</param>
		/// <returns>Downloads all orders matching the date and returns them in a list.</returns>
		public IList< T > GetOrdersList< T >( DateTime startDate, DateTime endDate, CancellationToken token, Mark mark = null  )
			where T : OrderResponseItem
		{
			return this.GetOrders< T >( startDate, endDate, token ).ToList();
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
		/// <param name="token">Cancellation Token</param>
		/// <param name="mark">Session Mark</param>
		/// <returns>Orders matching supplied criteria.</returns>
		public IEnumerable< T > GetOrders< T >( OrderCriteria orderCriteria, CancellationToken token, Mark mark = null )
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
				var ordersFromPage = this.GetOrdersPage( orderCriteria, mark );

				if( ordersFromPage == null || ordersFromPage.Length == 0 )
					break;

				orders.AddRange( ordersFromPage.OfType< T >() );
				orderCriteria.PageNumberFilter += 1;
			}

			this.CheckFulfillmentStatus( orders );

			return orders;
		}

		private OrderResponseItem[] GetOrdersPage( OrderCriteria orderCriteria, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{				
				return AP.CreateQuery( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var results = this._client.GetOrderList( this._credentials, this.AccountId, orderCriteria );				
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark: mark, additionalInfo: this.AdditionalLogInfo() ) );
					CheckCaSuccess( results );
					var resultData = results.ResultData ?? new OrderResponseItem[ 0 ];

					// If you get message code = 1 (Unexpected)
					if( results.MessageCode == 1 )
						resultData = this.HandleErrorUnexpected( orderCriteria, mark: mark );
					
					return resultData;
				} );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		private OrderResponseItem[] HandleErrorUnexpected( OrderCriteria orderCriteria, [ CallerMemberName ] string callerMemberName = "", Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{				
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

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
						this.RefreshLastNetworkActivityTime();
						ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
						var answer = this._client.GetOrderList( this._credentials, this.AccountId, orderCriteria );
						this.RefreshLastNetworkActivityTime();
						ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark: mark, additionalInfo: this.AdditionalLogInfo() ) );
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
				
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				return result.ToArray();
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <typeparam name="T">Type of order response.</typeparam>
		/// <param name="orderCriteria">The order criteria.</param>
		/// <param name="mark"></param>
		/// <returns>Orders matching supplied criteria.</returns>
		public async Task< IEnumerable< T > > GetOrdersAsync< T >( OrderCriteria orderCriteria, CancellationToken token, Mark mark = null )
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

					ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark: mark, methodResult: ordersFromPage.OfType< T >().ToJson(), additionalInfo: "\"Result of ordersFromPage.OfType< T >() for page\"" ) );
					orders.AddRange( ordersFromPage.OfType< T >() );
					orderCriteria.PageNumberFilter += 1;
				}

				await this.CheckFulfillmentStatusAsync( orders, mark );

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
				this.RefreshLastNetworkActivityTime();
				ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
				var results = await this._client.GetOrderListAsync( this._credentials, this.AccountId, orderCriteria ).ConfigureAwait( false );
				this.RefreshLastNetworkActivityTime();
				ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
				CheckCaSuccess( results.GetOrderListResult );
				var resultData = results.GetOrderListResult.ResultData ?? new OrderResponseItem[ 0 ];

				// If you get message code = 1 (Unexpected)
				if( results.GetOrderListResult.MessageCode == 1 )
					resultData = await this.HandleErrorUnexpectedAsync( mark, orderCriteria ).ConfigureAwait( false );

				return resultData;
			} ).ConfigureAwait( false );
		}

		private async Task< OrderResponseItem[] > HandleErrorUnexpectedAsync( Mark mark, OrderCriteria orderCriteria, [ CallerMemberName ] string callerMemberName = "" )
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
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : orderCriteria.ToJson() ) );
					var answer = await this._client.GetOrderListAsync( this._credentials, this.AccountId, orderCriteria ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : answer.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : orderCriteria.ToJson() ) );
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

			ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark: mark, methodResult: result.Select( o => o.OrderID ).ToJson(), additionalInfo: "\"Final result after looping\"", methodParameters: orderCriteria.ToJson() ) );
			return result.ToArray();
		}

		/// <summary>
		/// Submits the order.
		/// </summary>
		/// <param name="orderSubmit">The order submit.</param>
		/// <returns>New order CA id.</returns>
		public int SubmitOrder( OrderSubmit orderSubmit, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				return AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var apiResults = this._client.SubmitOrder( this._credentials, this.AccountId, orderSubmit );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( apiResults );
					return apiResults.ResultData;
				} );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< int > SubmitOrderAsync( OrderSubmit orderSubmit, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				return await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var apiResults = await this._client.SubmitOrderAsync( this._credentials, this.AccountId, orderSubmit ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( apiResults.SubmitOrderResult );
					return apiResults.SubmitOrderResult.ResultData;
				} ).ConfigureAwait( false );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}
		#endregion

		#region CheckFulfillmentStatus
		private void CheckFulfillmentStatus< T >( List< T > orders, Mark mark = null ) where T : OrderResponseItem
		{
			var refundedOrderIds = GetRefundedOrderIds( orders );

			if( refundedOrderIds.Count == 0 )
				return;

			int pageSize;
			if( !this._pageSizes.TryGetValue( "High", out pageSize ) )
				pageSize = 50;

			var cancelledOrderIds = new List< int >();
			var ordersParts = ItemsService.ToChunks( refundedOrderIds, pageSize );

			ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : refundedOrderIds.ToJson() ) );
			foreach( var part in ordersParts )
			{
				var ordersFulfillment = AP.CreateQuery( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var results = this._fulfillmentServiceClient.GetOrderFulfillmentDetailList( this._fulfillmentServiceCredentials, this.AccountId, part.ToArray(), null );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					CheckCaSuccess( results );
					var resultData = results.ResultData ?? new FulfillmentService.OrderFulfillmentResponse[ 0 ];

					return resultData;
				} );

				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : ordersFulfillment.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : part.ToJson() ) );

				var cancelledOrderIdsPart = ordersFulfillment.Where( o => o.FulfillmentList.All( fulfillment => fulfillment.FulfillmentStatus == "Canceled" ) ).Select( o => o.OrderID );
				cancelledOrderIds.AddRange( cancelledOrderIdsPart );
			}

			ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : cancelledOrderIds.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : refundedOrderIds.ToJson() ) );

			CancelOrders( orders, cancelledOrderIds );
		}

		private async Task CheckFulfillmentStatusAsync< T >( List< T > orders, Mark mark = null ) where T : OrderResponseItem
		{
			var refundedOrderIds = GetRefundedOrderIds( orders );

			if( refundedOrderIds.Count == 0 )
				return;

			int pageSize;
			if( !this._pageSizes.TryGetValue( "High", out pageSize ) )
				pageSize = 50;

			var ordersParts = ItemsService.ToChunks( refundedOrderIds, pageSize );

			ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : refundedOrderIds.ToJson() ) );
			var cancelledOrderIds = ( await ordersParts.ProcessInBatchAsync( 3, async part =>
			{
				var ordersFulfillment = await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var results = await this._fulfillmentServiceClient.GetOrderFulfillmentDetailListAsync( this._fulfillmentServiceCredentials, this.AccountId, part.ToArray(), null ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					CheckCaSuccess( results.GetOrderFulfillmentDetailListResult );
					var resultData = results.GetOrderFulfillmentDetailListResult.ResultData ?? new FulfillmentService.OrderFulfillmentResponse[ 0 ];

					return resultData;
				} ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTrace( this.CreateMethodCallInfo( mark : mark, methodResult : ordersFulfillment.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : part.ToJson() ) );

				return ordersFulfillment.Where( o => o.FulfillmentList.All( fulfillment => fulfillment.FulfillmentStatus == "Canceled" ) ).Select( o => o.OrderID );
			} ).ConfigureAwait( false ) ).SelectMany( x => x ).ToArray();

			ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : cancelledOrderIds.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : refundedOrderIds.ToJson() ) );

			CancelOrders( orders, cancelledOrderIds );
		}

		private static void CancelOrders< T >( IEnumerable< T > orders, IEnumerable< int > cancelledOrderIds ) where T : OrderResponseItem
		{
			if( !cancelledOrderIds.Any() )
				return;

			foreach( var order in orders )
			{
				if( cancelledOrderIds.Contains( order.OrderID ) )
				{
					order.OrderState = "Cancelled";
				}
			}
		}

		private static List< int > GetRefundedOrderIds< T >( IEnumerable< T > orders ) where T : OrderResponseItem
		{
			var refundedOrders = new List< int >();
			foreach( var order in orders.Where( o => o.OrderState != "Cancelled" ) )
			{
				var typedOrder = order as OrderResponseDetailLow;
				if( typedOrder == null )
					continue;
				var refundStatus = typedOrder.OrderStatus.OrderRefundStatus;
				if( refundStatus == "OrderLevel" || refundStatus == "LineItemLevel" || refundStatus == "OrderAndLineItemLevel" )
					refundedOrders.Add( order.OrderID );
			}
			return refundedOrders;
		}
		#endregion

		public IEnumerable< OrderUpdateResponse > UpdateOrderList( OrderUpdateSubmit[] orderUpdates, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				return AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var results = this._client.UpdateOrderList( this._credentials, this.AccountId, orderUpdates );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( results );
					return results.ResultData;
				} );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< IEnumerable< OrderUpdateResponse > > UpdateOrderListAsync( OrderUpdateSubmit[] orderUpdates, CancellationToken token, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				return await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var results = await this._client.UpdateOrderListAsync( this._credentials, this.AccountId, orderUpdates ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( results.UpdateOrderListResult );
					return results.UpdateOrderListResult.ResultData;
				} ).ConfigureAwait( false );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		private static void CheckCaSuccess( APIResultOfArrayOfOrderResponseItem orderList )
		{
			if( orderList.Status != ResultStatus.Success )
			{
				if( orderList.MessageCode != 1 )
					throw new ChannelAdvisorException( orderList.MessageCode, orderList.Message );
			}
		}

		private static void CheckCaSuccess( FulfillmentService.APIResultOfArrayOfOrderFulfillmentResponse response )
		{
			if( response.Status != FulfillmentService.ResultStatus.Success )
				throw new ChannelAdvisorException( response.MessageCode, response.Message );
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

		#region IDisposable implementation

		public void Dispose()
		{
			Dispose( _client, true );
			GC.SuppressFinalize( this );
		}

		#endregion
	}
}