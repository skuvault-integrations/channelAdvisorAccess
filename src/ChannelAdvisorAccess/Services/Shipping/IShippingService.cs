using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChannelAdvisorAccess.ShippingService;

namespace ChannelAdvisorAccess.Services.Shipping
{
	public interface IShippingService
	{
		string Name { get; }
		string AccountId { get; }
		void Ping();
		Task PingAsync();
		void MarkOrderShipped( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped );
		Task MarkOrderShippedAsync( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped );
		void MarkOrderShipped( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped );
		Task MarkOrderShippedAsync( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped );
		void MarkOrderShipped( int orderId, PartialShipmentContents partialShipmentContents );
		Task MarkOrderShippedAsync( int orderId, PartialShipmentContents partialShipmentContents );
		void MarkOrderShipped( string clientOrderId, PartialShipmentContents partialShipmentContents );
		Task MarkOrderShippedAsync( string clientOrderId, PartialShipmentContents partialShipmentContents );
		void SubmitOrderShipmentList( IEnumerable< OrderShipment > orderShipments );
		Task SubmitOrderShipmentListAsync( IEnumerable< OrderShipment > orderShipments );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( string[] clientOrderIdentifierList );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( string[] clientOrderIdentifierList );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, string[] clientOrderIdentifierList );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList, string[] clientOrderIdentifierList );
		ShippingCarrier[] GetShippingCarrierList();
		Task< ShippingCarrier[] > GetShippingCarrierListAsync();
	}
}