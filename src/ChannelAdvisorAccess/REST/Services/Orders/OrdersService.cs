using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.REST.Models;
using ChannelAdvisorAccess.REST.Misc;
using System.Net;
using System.Globalization;
using System.Web;
using ChannelAdvisorAccess.Exceptions;
using System.Runtime.Caching;

namespace ChannelAdvisorAccess.REST.Services
{
	/// <summary>
	/// Facade to work with ChannelAdvisor REST API
	/// </summary>
	public class OrdersService : RestServiceBaseAbstr, IOrdersService
	{
		private const string _apiUrl = "v1/orders";

		public string Name { get; private set; }

		/// <summary>
		///	REST authorization flow
		/// </summary>
		/// <param name="baseApiUrl"></param>
		/// <param name="accountName"></param>
		/// <param name="credentials"></param>
		public OrdersService( RestApplication application, string accountName, string accessToken, string refreshToken ) 
			: base( accountName, application, accessToken, refreshToken ) { }

		/// <summary>
		///	SOAP compatible authorization flow
		/// </summary>
		/// <param name="baseApiUrl">CA base API url</param>
		/// <param name="applicationID">applicationID recevied via developer console</param>
		/// <param name="sharedSecret">shared secret</param>
		/// <param name="scope">for example inventory, orders (multiply values should be delimited by space)</param>
		/// <param name="accountId">CA user account id (GUID)</param>
		/// <param name="accountName">CA user account friendly name</param>
		/// <param name="credentials">developer key and password</param>
		/// <param name="cache"></param>
		public OrdersService( RestApplication application, string accountId, string accountName, string developerKey, string developerPassword, ObjectCache cache = null ) 
			: base( application, accountName, accountId, developerKey, developerPassword, cache ) { }

		/// <summary>
		///	Gets orders by created date range
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IEnumerable< T > GetOrders< T >( DateTime startDate, DateTime endDate ) where T : OrderResponseItem
		{
			OrderCriteria criteria = new OrderCriteria()
			{
				StatusUpdateFilterBeginTimeGMT = startDate,
				StatusUpdateFilterEndTimeGMT = endDate,
			};

			return this.GetOrders< T >(criteria);
		}

		/// <summary>
		///	Gets orders by complex query
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="orderCriteria"></param>
		/// <returns></returns>
		public IEnumerable< T > GetOrders< T >( OrderCriteria orderCriteria ) where T : OrderResponseItem
		{
			return GetOrdersAsync< T >(orderCriteria).Result;
		}

		/// <summary>
		///	Gets orders and return it as list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IList< T > GetOrdersList< T >( DateTime startDate, DateTime endDate ) where T : OrderResponseItem
		{
			return this.GetOrders< T >( startDate, endDate ).ToList();
		}

		/// <summary>
		///	Gets orders asynchronously by created date range
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public async Task< IEnumerable < T > > GetOrdersAsync< T >( DateTime startDate, DateTime endDate ) where T : OrderResponseItem
		{
			OrderCriteria criteria = new OrderCriteria()
			{
				StatusUpdateFilterBeginTimeGMT = startDate,
				StatusUpdateFilterEndTimeGMT = endDate,
			};

			return await GetOrdersAsync< T >( criteria ).ConfigureAwait( false );
		}

		/// <summary>
		///	Gets orders asynchronously by complex query
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="orderCriteria"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< T > > GetOrdersAsync<T>( OrderCriteria orderCriteria, Mark mark = null ) where T : OrderResponseItem
		{
			string filterParam = GetRequestFilterString(orderCriteria);

			return await GetOrdersAsync< T >( filterParam, mark ).ConfigureAwait( false );
		}

		/// <summary>
		///	Gets orders asynchronously and returns it as list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter"></param>
		/// <returns></returns>
		public async Task< IEnumerable< T > > GetOrdersAsync < T >( string filter, Mark mark )
		{
			var orders = new List< T >();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			string url = _apiUrl;

			List< string > requestParams = new List< string >();
			requestParams.Add( "$expand=Items($expand=Promotions),Fulfillments,Adjustments,CustomFields" );

			if (!string.IsNullOrEmpty(filter))
				requestParams.Add( "$filter=" + filter );

			try
			{
				string nextLink = url;

				while ( nextLink != null )
				{
					if ( requestParams.Count > 0 )
						url = _apiUrl + "?" + string.Join("&", requestParams.ToArray());

					ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark: mark, additionalInfo: this.AdditionalLogInfo(), methodParameters: filter ) );

					var ordersFromRequest = await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
					{
						var response = await GetResponseAsync( url ).ConfigureAwait( false );

						return await response.Content.ReadAsAsync< ODataResponse< Models.Order > >();
					}).ConfigureAwait( false );

					nextLink = ordersFromRequest.NextLink;
					orders.AddRange( ordersFromRequest.Value.Select( order => order.ToOrderResponseDetailComplete() ).OfType< T >() );

					ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark: mark, methodResult: orders.ToJson(), additionalInfo: this.AdditionalLogInfo(), methodParameters: filter ) );

					if ( !string.IsNullOrEmpty( nextLink ) )
					{
						requestParams.Clear();
						requestParams.Add(nextLink.Split('?')[1]);
					}
				}
				
			}
			catch(Exception exception)
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark: mark, additionalInfo: this.AdditionalLogInfo(), methodParameters: filter ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}

			return orders;
		}

		public void Ping()
		{
			throw new NotImplementedException();
		}

		public Task PingAsync()
		{
			throw new NotImplementedException();
		}

		public int SubmitOrder(OrderSubmit orderSubmit)
		{
			throw new NotImplementedException();
		}

		public Task<int> SubmitOrderAsync(OrderSubmit orderSubmit)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<OrderUpdateResponse> UpdateOrderList(OrderUpdateSubmit[] orderUpdates)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<OrderUpdateResponse>> UpdateOrderListAsync(OrderUpdateSubmit[] orderUpdates)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///	Gets filtering parameter for REST GET request
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		private string GetRequestFilterString( OrderCriteria criteria )
		{
			List< string > clauses = new List< string >();
			
			if ( criteria.OrderCreationFilterBeginTimeGMT.HasValue )
				clauses.Add( $"CreatedDateUtc ge { ConvertDate( criteria.OrderCreationFilterBeginTimeGMT.Value ) } and " );

			if ( criteria.OrderCreationFilterEndTimeGMT.HasValue )
				clauses.Add( $"CreatedDateUtc le { ConvertDate( criteria.OrderCreationFilterEndTimeGMT.Value ) } and " );

			if (criteria.StatusUpdateFilterBeginTimeGMT.HasValue)
				clauses.Add( $"Fulfillments/any (f: f/UpdatedDateUtc ge { ConvertDate( criteria.StatusUpdateFilterBeginTimeGMT.Value ) }) and " );

			if (criteria.StatusUpdateFilterEndTimeGMT.HasValue)
				clauses.Add( $"Fulfillments/any (f: f/UpdatedDateUtc le { ConvertDate( criteria.StatusUpdateFilterEndTimeGMT.Value ) }) and " );

			if ( criteria.OrderIDList != null && criteria.OrderIDList.Length > 0)
			{
				clauses.Add( "(" );

				for (int i = 0; i < criteria.OrderIDList.Length; i++ )
				{
					clauses.Add( $"SiteOrderID eq '{ criteria.OrderIDList[i] }'" );

					if (i != criteria.OrderIDList.Length - 1)
						clauses.Add( " or " );
				}

				clauses.Add( ")" );
			}

			if ( clauses.Count > 0 )
				clauses[ clauses.Count - 1] = clauses.Last().Replace( " and ", string.Empty );

			return string.Join( " ", clauses );
		}
	}
}
