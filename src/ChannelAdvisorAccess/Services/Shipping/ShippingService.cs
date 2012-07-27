using System;
using System.Collections.Generic;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.ShippingService;
using Netco.Logging;
using APICredentials=ChannelAdvisorAccess.ShippingService.APICredentials;
using ResultStatus=ChannelAdvisorAccess.ShippingService.ResultStatus;
using System.Linq;

namespace ChannelAdvisorAccess.Services.Shipping
{
	public class ShippingService : IShippingService
	{
		private readonly APICredentials _credentials;
		private readonly ShippingServiceSoapClient _client;

		public ShippingService( APICredentials credentials, string accountId )
		{
			this._credentials = credentials;
			this.AccountId = accountId;
			this._client = new ShippingServiceSoapClient();
		}

		public ShippingService( APICredentials credentials, string name, string id ) : this( credentials, id )
		{
			this.Name = name;
		}

		public string Name { get; private set; }
		public string AccountId{ get; private set; }

		public void MarkOrderShippedOld( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				ActionPolicies.CaSubmitPolicy.Do( () =>
					{
						var result = _client.OrderShipped( _credentials, this.AccountId, orderId, dateShipped.ToUniversalTime(), carrierCode, classCode, trackingNumber, null );
						CheckCaSuccess( result );
					});
			}
			catch( Exception e )
			{
				this.Log().Error( e, "Failed to mark order '{0}' shipped for account '{1}' with carrier code '{2}' and class code '{3}'. Tracking number is '{4}'.",
				                  orderId, this.AccountId, carrierCode, classCode, trackingNumber );
				throw;
			}
		}

		public void MarkOrderShipped( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				ActionPolicies.CaSubmitPolicy.Do( () =>
					{
						var result = _client.SubmitOrderShipmentList( _credentials, this.AccountId, new OrderShipmentList{ ShipmentList = new[]{
							new OrderShipment{ 
								OrderId = orderId,
								ShipmentType = "Full",
								FullShipment = new FullShipmentContents{
									carrierCode = carrierCode,
									classCode = classCode,
									dateShippedGMT = dateShipped.ToUniversalTime(),
									trackingNumber = trackingNumber
								}
							}}});
						CheckCaSuccess( result );
					});
			}
			catch( Exception e )
			{
				this.Log().Error( e, "Failed to mark order '{0}' shipped for account '{1}' with carrier code '{2}' and class code '{3}'. Tracking number is '{4}'.",
					orderId, this.AccountId, carrierCode, classCode, trackingNumber );
				throw;
			}
		}

		public void MarkOrderShipped( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				ActionPolicies.CaSubmitPolicy.Do( () =>
					{
						var result = _client.SubmitOrderShipmentList( _credentials, this.AccountId, new OrderShipmentList{ ShipmentList = new[]{
							new OrderShipment{ 
								ClientOrderIdentifier = clientOrderId,
								ShipmentType = "Full",
								FullShipment = new FullShipmentContents{
									carrierCode = carrierCode,
									classCode = classCode,
									dateShippedGMT = dateShipped.ToUniversalTime(),
									trackingNumber = trackingNumber
								}
							}}});
						CheckCaSuccess( result );
					});
			}
			catch( Exception e )
			{
				this.Log().Error( e, "Failed to mark order '{0}' shipped for account '{1}' with carrier code '{2}' and class code '{3}'. Tracking number is '{4}'.",
					clientOrderId, this.AccountId, carrierCode, classCode, trackingNumber );
				throw;
			}
		}

		public void MarkOrderShipped( int orderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				ActionPolicies.CaSubmitPolicy.Do( () =>
					{
						var result = _client.SubmitOrderShipmentList( _credentials, this.AccountId, new OrderShipmentList{ ShipmentList = new[]{
							new OrderShipment{ 
								OrderId = orderId,
								ShipmentType = "Partial",
								PartialShipment = partialShipmentContents
							}}});
						CheckCaSuccess( result );
					});
			}
			catch( Exception e )
			{
				this.Log().Error( e, "Failed to mark order '{0}' shipped for account '{1}' with carrier code '{2}' and class code '{3}'. Tracking number is '{4}'.",
					orderId, this.AccountId, partialShipmentContents.carrierCode, partialShipmentContents.classCode, partialShipmentContents.trackingNumber );
				throw;
			}
		}

		public void MarkOrderShipped( string clientOrderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				ActionPolicies.CaSubmitPolicy.Do( () =>
					{
						var result = _client.SubmitOrderShipmentList( _credentials, this.AccountId, new OrderShipmentList{ ShipmentList = new[]{
							new OrderShipment{ 
								ClientOrderIdentifier = clientOrderId,
								ShipmentType = "Partial",
								PartialShipment = partialShipmentContents
							}}});
						CheckCaSuccess( result );
					});
			}
			catch( Exception e )
			{
				this.Log().Error( e, "Failed to mark order '{0}' shipped for account '{1}' with carrier code '{2}' and class code '{3}'. Tracking number is '{4}'.",
					clientOrderId, this.AccountId, partialShipmentContents.carrierCode, partialShipmentContents.classCode, partialShipmentContents.trackingNumber );
				throw;
			}
		}

		public void SubmitOrderShipmentList( OrderShipment[] orderShipments )
		{
			try
			{
				const int pageSize = 50;
				var pagesCount = orderShipments.Length / pageSize + (orderShipments.Length % pageSize > 0 ? 1 : 0);

				for( int pageNumber = 0; pageNumber < pagesCount; pageNumber++ )
				{
					var shipmentsToSubmit = orderShipments.Skip( pageNumber * pageSize ).Take( pageSize ).ToArray();

					ActionPolicies.CaSubmitPolicy.Do( () =>
				                                  	{
				                                  		var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId,
												new OrderShipmentList{ ShipmentList = shipmentsToSubmit });
											CheckCaSuccess( result );
				                                  	} );
				}
			}
			catch( Exception e )
			{
				this.Log().Error( e, "Failed to submit order shipment list" );
				throw;
			}
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList )
		{
			return GetOrderShipmentHistoryList( orderIdList, new string[]{} );
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( string [] clientOrderIdentifierList )
		{
			return GetOrderShipmentHistoryList( new int[]{}, clientOrderIdentifierList );
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, string [] clientOrderIdentifierList )
		{
			return ActionPolicies.CaSubmitPolicy.Get( () =>
					{
						var result = _client.GetOrderShipmentHistoryList( _credentials, this.AccountId, orderIdList, clientOrderIdentifierList );
						CheckCaSuccess( result );
						return result.ResultData;
					});
		}


		public ShippingCarrier[] GetShippingCarrierList()
		{
			var result = this._client.GetShippingCarrierList( this._credentials, this.AccountId );
			CheckCaSuccess( result ); 
			return result.ResultData;
		}

		/// <summary>Checks the ca success.</summary>
		/// <param name="result">The result.</param>
		private void CheckCaSuccess( APIResultOfArrayOfShipmentResponse result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
			ChannelAdvisorException exceptionToThrow = null;
			foreach( var shipmentResponse in result.ResultData )
			{
				if( !shipmentResponse.Success )
				{
					this.Log().Error( "Error encountered while marking order shipped: {0}", shipmentResponse.Message );
					if( exceptionToThrow == null )
						exceptionToThrow = new ChannelAdvisorException( shipmentResponse.Message );
				}
			}
			if( exceptionToThrow != null )
				throw exceptionToThrow;
		}

		/// <summary>
		/// Checks the ca success.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <exception cref="ChannelAdvisorException">Thrown if CA call failed.</exception>
		private static void CheckCaSuccess( APIResultOfBoolean result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		/// <summary>
		/// Checks the ca success.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <exception cref="ChannelAdvisorException">Thrown if CA call failed.</exception>
		private static void CheckCaSuccess( APIResultOfArrayOfShippingCarrier result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		private void CheckCaSuccess( APIResultOfArrayOfOrderShipmentHistoryResponse result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}
	}
}