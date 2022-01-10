using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.ShippingService;

namespace ChannelAdvisorAccess.Services.Shipping
{
	public interface IShippingService : IDisposable
	{
		string Name { get; }
		string AccountId { get; }
		void Ping( Mark mark );
		Task PingAsync( Mark mark );
		void MarkOrderShipped( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped, Mark mark );
		Task MarkOrderShippedAsync( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped, Mark mark );
		void MarkOrderShipped( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped, Mark mark );
		Task MarkOrderShippedAsync( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped, Mark mark );
		void MarkOrderShipped( int orderId, PartialShipmentContents partialShipmentContents, Mark mark );
		Task MarkOrderShippedAsync( int orderId, PartialShipmentContents partialShipmentContents, Mark mark );
		void MarkOrderShipped( string clientOrderId, PartialShipmentContents partialShipmentContents, Mark mark );
		Task MarkOrderShippedAsync( string clientOrderId, PartialShipmentContents partialShipmentContents, Mark mark );
		void SubmitOrderShipmentList( IEnumerable< OrderShipment > orderShipments, Mark mark );
		Task SubmitOrderShipmentListAsync( IEnumerable< OrderShipment > orderShipments, Mark mark );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, Mark mark );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList, Mark mark );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( string[] clientOrderIdentifierList, Mark mark );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( string[] clientOrderIdentifierList, Mark mark );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, string[] clientOrderIdentifierList, Mark mark );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList, string[] clientOrderIdentifierList, Mark mark );
		ShippingCarrier[] GetShippingCarrierList( Mark mark );
		Task< ShippingCarrier[] > GetShippingCarrierListAsync( Mark mark );
	}
}