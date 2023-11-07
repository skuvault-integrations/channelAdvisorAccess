﻿using ChannelAdvisorAccess.REST.Models.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ChannelAdvisorAccess.REST.Shared
{
	public static class MultiPartResponseParser
	{
		public static bool TryParse< T >( string response, out int httpBatchStatusCode, out IEnumerable< T > result, out string parseErrorMessage )
		{
			result = new T[] { };
			httpBatchStatusCode = 0;
			parseErrorMessage = "";
			var isResponseValid = true;

			if ( string.IsNullOrWhiteSpace( response ) )
				return false;

			var deserializeErrorMessage = "";
			var batchResponses = JsonConvert.DeserializeObject< BatchResponses< T > >( response, new JsonSerializerSettings() {
				Error = ( s, args ) =>
				{
					args.ErrorContext.Handled = true;
					isResponseValid = false;
					deserializeErrorMessage = args?.ErrorContext?.Error?.Message;
				}
			} );

			if ( isResponseValid )
			{
				var batchResponsesList = ( batchResponses != null && batchResponses.Responses != null ) ? 
				batchResponses.Responses.ToList() : new List< BatchResponse< T > >();
				var lastItemWithStatus300AndUp = batchResponsesList.LastOrDefault( x => x.Status >= 300 );
				httpBatchStatusCode = ( lastItemWithStatus300AndUp != null ) ? lastItemWithStatus300AndUp.Status : ( int ) HttpStatusCode.OK;
				result = batchResponsesList.Select( x => x.Body );
				return true;
			}

			parseErrorMessage = deserializeErrorMessage;
			return false;
		}
	}
}
