using System;
using ChannelAdvisorAccess.Constants;
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
		
		[ Test ]
		public void ToSoapOrderCriteria_WhenDateFiltersSpecified_ThenMapsDateFieldValues()
		{
			var statusUpdateBegin = new DateTime( 2023, 3, 3, 3, 3, 3 );
			var statusUpdateEnd = new DateTime( 2024, 4, 4, 4, 4, 4 );
			var detailLevel = DetailLevelTypes.Complete;
			var orderCriteria = new OrderCriteria
			{
				StatusUpdateFilterBegin = statusUpdateBegin,
				StatusUpdateFilterEnd = statusUpdateEnd,
				DetailLevel = detailLevel
			};
			
			var result = orderCriteria.ToSoapOrderCriteria();
			
			Assert.That( result.StatusUpdateFilterBeginTimeGMT.Value, Is.EqualTo( statusUpdateBegin ) );
			Assert.That( result.StatusUpdateFilterEndTimeGMT.Value, Is.EqualTo( statusUpdateEnd ) );
			Assert.That( result.DetailLevel, Is.EqualTo( detailLevel ) );
		}

		[ Test ]
		public void ToSoapOrderCriteria_WhenOrderIdsFilterSpecified_ThenMapsOrderIds()
		{
			var orderIdList = new [] { 1, 2, 3 };
			var detailLevel = DetailLevelTypes.Complete;
			var orderCriteria = new OrderCriteria
			{
				OrderIDList = orderIdList,
				DetailLevel = detailLevel
			};
			
			var result = orderCriteria.ToSoapOrderCriteria();
			
			Assert.That( result.OrderIDList, Is.EqualTo( orderIdList ) );
			Assert.That( result.DetailLevel, Is.EqualTo( detailLevel ) );
		}
		
		[ Test ]
		public void ToSoapOrderCriteria_WhenImportDateIsNotNull_ShouldThrow()
		{
			var orderCriteria = new OrderCriteria
			{
				ImportDateFilterBegin = new DateTime( 2023, 3, 3, 3, 3, 3 ),
				ImportDateFilterEnd = new DateTime( 2023, 3, 3, 3, 3, 3 )
			};
			
			var ex = Assert.Throws< Exception >( () => orderCriteria.ToSoapOrderCriteria() );
			Assert.That( ex.Message, Is.EqualTo( "SOAP API doesn't support ImportDate filters" ) );
		}
	}
}
