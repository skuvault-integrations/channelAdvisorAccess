using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using ChannelAdvisorAccess.REST.Models.Configuration;
using ChannelAdvisorAccess.REST.Shared;

namespace ChannelAdvisorAccess.REST.Services.Orders
{
	/// <summary>
	/// Facade to work with ChannelAdvisor REST API
	/// </summary>
	public class OrdersService : RestServiceBaseAbstr, IOrdersService
	{
		/// <summary>
		///	Rest service with standard authorization flow
		/// </summary>
		/// <param name="credentials">Rest application credentials</param>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="accessToken">Tenant access token</param>
		/// <param name="refreshToken">Tenant refresh token</param>
		public OrdersService( RestCredentials credentials, string accountName, string accessToken, string refreshToken ) 
			: base( credentials, accountName, accessToken, refreshToken ) { }

		/// <summary>
		///	Rest service with soap compatible authorization flow
		/// </summary>
		/// <param name="credentials">Rest application credentials</param>
		/// <param name="soapCredentials">Soap application credentials</param>
		/// <param name="accountId">Tenant account id</param>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="cache"></param>
		public OrdersService( RestCredentials credentials, APICredentials soapCredentials, string accountId, string accountName ) 
			: base( credentials, soapCredentials, accountId, accountName ) { }

		/// <summary>
		///	Gets orders by created date range
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IEnumerable< T > GetOrders< T >( DateTime startDate, DateTime endDate ) where T : OrderResponseItem
		{
			var criteria = new OrderCriteria()
			{
				StatusUpdateFilterBeginTimeGMT = startDate,
				StatusUpdateFilterEndTimeGMT = endDate,
			};

			return this.GetOrders< T >( criteria );
		}

		/// <summary>
		///	Gets orders by complex query
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="orderCriteria"></param>
		/// <returns></returns>
		public IEnumerable< T > GetOrders< T >( OrderCriteria orderCriteria ) where T : OrderResponseItem
		{
			return this.GetOrdersAsync< T >( orderCriteria ).GetAwaiter().GetResult();
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
		public Task< IEnumerable < T > > GetOrdersAsync< T >( DateTime startDate, DateTime endDate ) where T : OrderResponseItem
		{
			OrderCriteria criteria = new OrderCriteria()
			{
				StatusUpdateFilterBeginTimeGMT = startDate,
				StatusUpdateFilterEndTimeGMT = endDate,
			};

			return this.GetOrdersAsync< T >( criteria );
		}

		/// <summary>
		///	Gets orders asynchronously by complex query
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="orderCriteria"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public Task< IEnumerable< T > > GetOrdersAsync< T >( OrderCriteria orderCriteria, Mark mark = null ) where T : OrderResponseItem
		{
			var filterParam = this.GetRequestFilterString( orderCriteria );

			return this.GetOrdersAsync< T >( filterParam, mark );
		}

		/// <summary>
		///	Gets orders asynchronously and returns it as list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< T > > GetOrdersAsync < T >( string filter, Mark mark )
		{
			var orders = new List< T >();

			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			var url = ChannelAdvisorEndPoint.OrdersUrl + "?$expand=Items($expand=Promotions),Fulfillments,Adjustments,CustomFields";

			if ( !string.IsNullOrEmpty( filter )) 
				url += "&$filter=" + filter;

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark: mark, additionalInfo: this.AdditionalLogInfo(), methodParameters: filter ) );

				var ordersFromRequest = await this.GetResponseAsync< Models.Order >( url, mark );

				orders.AddRange( ordersFromRequest.Select( order => order.ToOrderResponseDetailComplete() ).OfType< T >() );

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark: mark, methodResult: orders.ToJson(), additionalInfo: this.AdditionalLogInfo(), methodParameters: filter ) );
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark: mark, additionalInfo: this.AdditionalLogInfo(), methodParameters: filter ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}

			return orders;
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
				clauses.Add( String.Format( "CreatedDateUtc ge {0} and ", base.ConvertDate( criteria.OrderCreationFilterBeginTimeGMT.Value ) ) );

			if ( criteria.OrderCreationFilterEndTimeGMT.HasValue )
				clauses.Add( String.Format( "CreatedDateUtc le {0} and ", base.ConvertDate( criteria.OrderCreationFilterEndTimeGMT.Value ) ) );

			if (criteria.StatusUpdateFilterBeginTimeGMT.HasValue)
				clauses.Add( String.Format( "Fulfillments/any (f: f/UpdatedDateUtc ge {0}) and ", base.ConvertDate( criteria.StatusUpdateFilterBeginTimeGMT.Value ) ) );

			if (criteria.StatusUpdateFilterEndTimeGMT.HasValue)
				clauses.Add( String.Format( "Fulfillments/any (f: f/UpdatedDateUtc le {0}) and ", base.ConvertDate( criteria.StatusUpdateFilterEndTimeGMT.Value ) ) );

			if ( criteria.OrderIDList != null && criteria.OrderIDList.Length > 0)
			{
				clauses.Add( "(" );

				for (int i = 0; i < criteria.OrderIDList.Length; i++ )
				{
					clauses.Add( String.Format( "SiteOrderID eq '{0}'", criteria.OrderIDList[i] ) );

					if (i != criteria.OrderIDList.Length - 1)
						clauses.Add( " or " );
				}

				clauses.Add( ")" );
			}

			if ( clauses.Count > 0 )
				clauses[ clauses.Count - 1] = clauses.Last().Replace( " and ", string.Empty );

			return string.Join( " ", clauses );
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
	}
}
