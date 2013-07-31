using System;
using System.Runtime.Serialization;

namespace ChannelAdvisorAccess.Services.Shipping
{
	[ Serializable ]
	public class MarkOrderShippedException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public MarkOrderShippedException()
		{
		}

		public MarkOrderShippedException( string message ) : base( message )
		{
		}

		public MarkOrderShippedException( string message, Exception inner ) : base( message, inner )
		{
		}

		protected MarkOrderShippedException(
			SerializationInfo info,
			StreamingContext context ) : base( info, context )
		{
		}

		public MarkOrderShippedException( string orderId, string accountId, string carrier, string @class, string trackingNumber ) :
			base( GenerateMessage( orderId, accountId, carrier, @class, trackingNumber ) )
		{
			this.OrderId = orderId;
			this.AccountId = accountId;
			this.Carrier = carrier;
			this.Class = @class;
			this.TrackingNumber = trackingNumber;
		}

		public MarkOrderShippedException( string orderId, string accountId, string carrier, string @class, string trackingNumber, Exception inner ) :
			base( GenerateMessage( orderId, accountId, carrier, @class, trackingNumber ), inner )
		{
			this.OrderId = orderId;
			this.AccountId = accountId;
			this.Carrier = carrier;
			this.Class = @class;
			this.TrackingNumber = trackingNumber;
		}

		public string OrderId { get; private set; }
		public string AccountId{ get; private set; }
		public string Carrier{ get; private set; }
		public string Class { get; private set; }
		public string TrackingNumber { get; private set; }

		private static string GenerateMessage( string id, string accountId, string carrier, string @class, string trackingNumber )
		{
			return string.Concat( "Id: ", id, ", AccountId: ", accountId, ", Carrier: ", carrier ?? string.Empty, ", Class: ", @class ?? string.Empty, ", Tracking Number: ", trackingNumber ?? string.Empty ); 
		}
	}
}