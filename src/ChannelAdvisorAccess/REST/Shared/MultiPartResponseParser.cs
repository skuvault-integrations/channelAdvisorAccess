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
			var batchResponses = JsonConvert.DeserializeObject< BatchResponses< T > >( response )?.Responses;
			httpBatchStatusCode = batchResponses?.ToList().Last().Status ?? ( int ) HttpStatusCode.OK;
			return batchResponses?.ToList().Select( x => x.Body ) ?? new List<T>();
		}
	}
}
