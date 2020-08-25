using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChannelAdvisorAccess.REST.Models.Infrastructure
{
	public class BatchResponses< T >
	{
		[ JsonProperty( "responses" ) ]
		public IEnumerable< BatchResponse< T > > Responses { get; set; }
	}

	public class BatchResponse< T >
	{
		[ JsonProperty( "id" ) ]
		public string Id { get; set; }

		[ JsonProperty( "status" ) ]
		public int Status { get; set; }

		[ JsonProperty( "headers" ) ]
		public object Headers { get; set; }

		[ JsonProperty( "body" ) ]
		public T Body { get; set; }
	}
}
