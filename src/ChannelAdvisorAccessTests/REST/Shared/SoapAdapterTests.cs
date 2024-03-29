﻿using System;
using ChannelAdvisorAccess.REST.Models;
using ChannelAdvisorAccess.REST.Shared;
using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.REST.Shared
{
	[ TestFixture ]
	public class SoapAdapterTests
	{
		private readonly Randomizer _randomizer = new Randomizer();

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
		public void ToQuantityInfoResponse_ReturnsQuantityInfoResponseWithZeroAvailableQuantity_WhenTotalAvailableQuantityNotSpecified()
		{
			// Arrange
			var product = new Product();

			// Act
			var quantityInfoResponse = product.ToQuantityInfoResponse();

			// Assert
			quantityInfoResponse.Available.Should().Be( 0 );
		}

		[ Test ]
		public void ToQuantityInfoResponse_ReturnsQuantityInfoResponseWithZeroAvailableQuantity_WhenTotalAvailableQuantityIsNull()
		{
			// Arrange
			var product = new Product
			{
				TotalAvailableQuantity = null
			};

			// Act
			var quantityInfoResponse = product.ToQuantityInfoResponse();

			// Assert
			quantityInfoResponse.Available.Should().Be( 0 );
		}

		[ Test ]
		public void ToQuantityInfoResponse_ReturnsQuantityInfoResponseWithCorrectAvailableQuantity_WhenTotalAvailableQuantityIsNotNull()
		{
			// Arrange
			var product = new Product
			{
				TotalAvailableQuantity = _randomizer.Next( 1, int.MaxValue )
			};

			// Act
			var quantityInfoResponse = product.ToQuantityInfoResponse();

			// Assert
			quantityInfoResponse.Available.Should().Be( product.TotalAvailableQuantity );
		}

		[ Test ]
		public void GetDistributionCenterCode_ReturnsNull_WhenOrderItemDoesNotExist()
		{
			// Arrange
			var orderItemId = this._randomizer.Next( 0, 100 );
			var fulfiilments = new[]
			{
				new Fulfillment
				{
					Items = new[]
					{
						new FulfillmentItem
						{
							OrderItemID = orderItemId + 1
						}
					}
				}
			};

			var dcCodes = Array.Empty< DistributionCenter >();

			// Act
			var dcCode = SoapAdapter.GetDistributionCenterCode( fulfiilments, orderItemId, dcCodes );

			// Assert
			dcCode.Should().BeNullOrEmpty();
		}

		[ Test ]
		public void GetDistributionCenterCode_ReturnsNull_WhenDistributionCenterIdIsNull()
		{
			// Arrange
			var orderItemId = this._randomizer.Next( 0, 100 );
			var distributionCenterId = this._randomizer.Next( 0, 100 );
			var distributionCenterCode = Guid.NewGuid().ToString();
			var fulfiilments = new[]
			{
				new Fulfillment
				{
					Items = new[]
					{
						new FulfillmentItem
						{
							OrderItemID = orderItemId
						}
					},
					DistributionCenterID = null
				}
			};

			var dcCodes = new[]
			{
				new DistributionCenter
				{
					ID = distributionCenterId,
					Code = distributionCenterCode
				}
			};

			// Act
			var dcCode = SoapAdapter.GetDistributionCenterCode( fulfiilments, orderItemId, dcCodes );

			// Assert
			dcCode.Should().BeNullOrEmpty();
		}

		[ Test ]
		public void GetDistributionCenterCode_ReturnsNull_WhenDistributionCenterDoesNotExist()
		{
			// Arrange
			var orderItemId = this._randomizer.Next( 0, 100 );
			var distributionCenterId = this._randomizer.Next( 0, 100 );
			var distributionCenterCode = Guid.NewGuid().ToString();
			var fulfiilments = new[]
			{
				new Fulfillment
				{
					Items = new[]
					{
						new FulfillmentItem
						{
							OrderItemID = orderItemId
						}
					},
					DistributionCenterID = distributionCenterId
				}
			};

			var dcCodes = new[]
			{
				new DistributionCenter
				{
					ID = distributionCenterId + 1,
					Code = distributionCenterCode
				}
			};

			// Act
			var dcCode = SoapAdapter.GetDistributionCenterCode( fulfiilments, orderItemId, dcCodes );

			// Assert
			dcCode.Should().BeNullOrEmpty();
		}

		[ Test ]
		public void GetDistributionCenterCode_ReturnsCode()
		{
			// Arrange
			var orderItemId = this._randomizer.Next( 0, 100 );
			var distributionCenterId = this._randomizer.Next( 0, 100 );
			var distributionCenterCode = Guid.NewGuid().ToString();
			var fulfiilments = new[]
			{
				new Fulfillment
				{
					Items = new[]
					{
						new FulfillmentItem
						{
							OrderItemID = orderItemId
						}
					},
					DistributionCenterID = distributionCenterId
				}
			};

			var dcCodes = new[]
			{
				new DistributionCenter
				{
					ID = distributionCenterId,
					Code = distributionCenterCode
				}
			};

			// Act
			var dcCode = SoapAdapter.GetDistributionCenterCode( fulfiilments, orderItemId, dcCodes );

			// Assert
			dcCode.Should().Be( distributionCenterCode );
		}
	}
}
