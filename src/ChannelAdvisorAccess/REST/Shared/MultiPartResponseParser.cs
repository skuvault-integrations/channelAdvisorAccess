using ChannelAdvisorAccess.REST.Models.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ChannelAdvisorAccess.REST.Shared
{
	public static class MultiPartResponseParser
	{
		public static IEnumerable< T > Parse< T >( string response, out int httpBatchStatusCode )
		{
			var batchResponses = JsonConvert.DeserializeObject< BatchResponses< T > >( response );
			var batchResponsesList = ( batchResponses != null && batchResponses.Responses != null ) ? 
				batchResponses.Responses.ToList() : new List< BatchResponse< T > >();
			var lastResponseItem = batchResponsesList.LastOrDefault();
			httpBatchStatusCode = ( lastResponseItem != null ) ? lastResponseItem.Status : ( int ) HttpStatusCode.OK;
			return batchResponsesList.Select( x => x.Body ) ?? new List< T >();
		}
	}
}
