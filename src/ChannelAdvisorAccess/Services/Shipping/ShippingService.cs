using System;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.ShippingService;
using Netco.Logging;
using APICredentials=ChannelAdvisorAccess.ShippingService.APICredentials;
using ResultStatus=ChannelAdvisorAccess.ShippingService.ResultStatus;

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

		/// <summary>Markes the order shipped old.</summary>
		/// <param name="orderId">The order id.</param>
		/// <param name="carrierCode">The carrier code.</param>
		/// <param name="classCode">The class code.</param>
		/// <param name="trackingNumber">The tracking number.</param>
		/// <param name="dateShipped">The date shipped.</param>
		/// <remarks>Uses old OrderShipped CA API method to mark the order shipped.</remarks>
		/// <seealso href="http://developer.channeladvisor.com/display/cadn/OrderShipped"/>
		public void MarkeOrderShippedOld( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				ActionPolicies.CaSubmitPolicy.Do( () =>
					{
						var result = _client.OrderShipped( _credentials, this.AccountId, orderId, dateShipped, carrierCode, classCode, trackingNumber, null );
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
								ShipmentType = ShipmentTypeEnum.Full,
								FullShipment = new FullShipmentContents{
									carrierCode = carrierCode,
									classCode = classCode,
									dateShippedGMT = dateShipped,
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
								ShipmentType = ShipmentTypeEnum.Full,
								FullShipment = new FullShipmentContents{
									carrierCode = carrierCode,
									classCode = classCode,
									dateShippedGMT = dateShipped,
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
								ShipmentType = ShipmentTypeEnum.Partial,
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
								ShipmentType = ShipmentTypeEnum.Partial,
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