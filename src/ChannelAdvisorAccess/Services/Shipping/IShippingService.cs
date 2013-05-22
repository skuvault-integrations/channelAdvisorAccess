using System;
using ChannelAdvisorAccess.ShippingService;

namespace ChannelAdvisorAccess.Services.Shipping
{
	public interface IShippingService
	{
		/// <summary>
		/// Marks the order shipped.
		/// </summary>
		/// <param name="orderId">The order ID.</param>
		/// <param name="carrierCode">The carrier code.</param>
		/// <param name="classCode">The class code.</param>
		/// <param name="trackingNumber">The tracking number.</param>
		/// <param name="dateShipped">The date shipped when order was shipped (will be converted to UTC).</param>
		void MarkOrderShipped( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped );

		/// <summary>
		/// Gets the account name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the account id.
		/// </summary>
		string AccountId{ get; }

		/// <summary>
		/// Gets the shipping carrier list.
		/// </summary>
		/// <returns>List of supported shipping carriers.</returns>
		ShippingCarrier[] GetShippingCarrierList();

		void MarkOrderShipped( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped );
		void MarkOrderShipped( int orderId, PartialShipmentContents partialShipmentContents );
		void MarkOrderShipped( string clientOrderId, PartialShipmentContents partialShipmentContents );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( string [] clientOrderIdentifierList );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, string [] clientOrderIdentifierList );
		void SubmitOrderShipmentList( OrderShipment[] orderShipments );
	}
}