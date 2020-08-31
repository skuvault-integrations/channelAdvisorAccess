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
			var lastItemWithStatus300AndUp = batchResponsesList.LastOrDefault( x => x.Status >= 300 );
			httpBatchStatusCode = ( lastItemWithStatus300AndUp != null ) ? lastItemWithStatus300AndUp.Status : ( int ) HttpStatusCode.OK;
			return batchResponsesList.Select( x => x.Body ) ?? new List< T >();
		}
	}
}
