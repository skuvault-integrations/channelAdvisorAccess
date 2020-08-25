using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models.Infrastructure
{
	public sealed class ODataResponse< T >
	{
		[ JsonProperty( PropertyName = "@odata.context") ]
		public string Context { get; set; }
		public T[] Value { get; set; }
		[ JsonProperty( PropertyName = "@odata.nextLink" ) ]
		public string NextLink { get; set; }
		[ JsonProperty( PropertyName = "@odata.count" ) ]
		public int? Count { get; set; }
		[ JsonProperty( "error" ) ]
		public ResponseError Error { get; set; }
	}

	public class ResponseError
	{
		[ JsonProperty( "code" ) ]
		public string Code { get; set; }
	
		[ JsonProperty( "message" ) ]
		public string Message { get; set; }
	}
}
