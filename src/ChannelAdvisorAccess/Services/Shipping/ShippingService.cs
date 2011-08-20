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
	}
}