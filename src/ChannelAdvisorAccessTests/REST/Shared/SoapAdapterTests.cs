using ChannelAdvisorAccess.REST.Models;
using ChannelAdvisorAccess.REST.Shared;
using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.REST.Shared
{
	[ TestFixture ]
	public class SoapAdapterTests
	{
		[ Test ]
		public void Fulfillments_ToShipmentList()
		{
			string carrierCode = "Carrier";
			string classCode = "33rd";
			string trackingNumber = "alksfj2ijr2oi";
			var fulfillments = new []
			{ 
				new Fulfillment
				{
					ShippingCarrier = carrierCode,
					ShippingClass = classCode,
					TrackingNumber = trackingNumber
				}
			};

			var result = fulfillments.ToShipmentList();

			result[ 0 ].ShippingCarrier.Should().Be( carrierCode );
			result[ 0 ].ShippingClass.Should().Be( classCode );
			result[ 0 ].TrackingNumber.Should().Be( trackingNumber );
		}
	}
}
