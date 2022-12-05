using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.REST.Models;
using ChannelAdvisorAccess.REST.Models.Infrastructure;
using ChannelAdvisorAccess.REST.Shared;
using ChannelAdvisorAccess.Services.Items;
using APICredentials = ChannelAdvisorAccess.OrderService.APICredentials;

namespace ChannelAdvisorAccess.REST.Services.Items
{
	public class ItemsPagingService: ItemsService, IItemsService
	{
		/// <summary>
		/// Rest items service with standard authorization flow
		/// </summary>
		/// <param name="credentials">Rest application credentials</param>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="accessToken">Tenant access token</param>
		/// <param name="refreshToken">Tenant refresh token</param>
		/// <param name="timeouts"></param>
		public ItemsPagingService( RestCredentials credentials, string accountName, string accessToken, string refreshToken, ChannelAdvisorTimeouts timeouts )
			: base( credentials, accountName, accessToken, refreshToken, timeouts )
		{
		}

		/// <summary>
		/// Rest items service with soap compatible authorization flow
		/// </summary>
		/// <param name="credentials">Rest application credentials</param>
		/// <param name="soapCredentials">Soap application credentials</param>
		/// <param name="accountId">Tenant account id</param>
		/// <param name="accountName">Tenant account name</param>
		/// <param name="timeouts"></param>
		public ItemsPagingService( RestCredentials credentials, APICredentials soapCredentials, string accountId, string accountName, ChannelAdvisorTimeouts timeouts )
			: base( credentials, soapCredentials, accountId, accountName, timeouts )
		{
		}

		/// <summary>
		/// Gets filtered items asynchronously with extra page information
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public new async Task< IEnumerable< InventoryItemResponse > > GetFilteredItemsAsync( ItemsFilter filter, Mark mark, CancellationToken token = default )
		{
			var url = new ItemsServiceUrlBuilder().GetProductsUrl( filter, null, null );

			var result = await Task.FromResult( this.GetProducts( url, mark, token : token ) );

			return ( from product in result.SelectMany( x => x ) select product.ToInventoryItemResponse() ).ToList();
		}

		/// <summary>
		/// Gets filtered skus asynchronously with extra page information
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="mark"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public new async Task< List< string > > GetFilteredSkusAsync( ItemsFilter filter, Mark mark, CancellationToken token = default )
		{
			var url = new ItemsServiceUrlBuilder().GetProductsUrl( filter, "ID,Sku", null );
			var result = await Task.FromResult( this.GetProducts( url, mark, this.Timeouts[ ChannelAdvisorOperationEnum.GetProductsByFilterWithIdOnlyRest ], token ) );

			return result.SelectMany( x => x ).Select( item => item.Sku ).ToList();
		}

		/// <summary>
		/// Gets products
		/// </summary>
		/// <param name="url"></param>
		/// <param name="mark"></param>
		/// <param name="operationTimeout"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		private IEnumerable< Product[] > GetProducts( string url, Mark mark, int? operationTimeout = null, CancellationToken token = default )
		{
			var nextLink = url;
			while( !string.IsNullOrEmpty( nextLink ) )
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo(
					mark : mark,
					additionalInfo : this.AdditionalLogInfo(),
					methodParameters : url ) );

				ODataResponse< Product > resultByPage;

				try
				{
					resultByPage = this.GetEntityAsync< ODataResponse< Product > >( nextLink, mark, operationTimeout, token ).GetAwaiter().GetResult();
					nextLink = resultByPage.NextLink;
				}
				catch( Exception exception )
				{
					var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
					ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
					throw channelAdvisorException;
				}

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo(
					mark : mark,
					methodParameters : url,
					methodResult : resultByPage.ToJson(),
					additionalInfo : this.AdditionalLogInfo() ) );

				yield return resultByPage.Value;
			}
		}
	}
}