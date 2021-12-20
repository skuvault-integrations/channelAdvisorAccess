using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.ShippingService;
using Netco.Logging;

namespace ChannelAdvisorAccess.Services.Shipping
{
	public interface IShippingService
	{
		string Name { get; }
		string AccountId { get; }
		void Ping( Mark mark = null );
		Task PingAsync( Mark mark = null );
		void MarkOrderShipped( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped, Mark mark = null );
		Task MarkOrderShippedAsync( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped, Mark mark = null );
		void MarkOrderShipped( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped, Mark mark = null );
		Task MarkOrderShippedAsync( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped, Mark mark = null );
		void MarkOrderShipped( int orderId, PartialShipmentContents partialShipmentContents, Mark mark = null );
		Task MarkOrderShippedAsync( int orderId, PartialShipmentContents partialShipmentContents, Mark mark = null );
		void MarkOrderShipped( string clientOrderId, PartialShipmentContents partialShipmentContents, Mark mark = null );
		Task MarkOrderShippedAsync( string clientOrderId, PartialShipmentContents partialShipmentContents, Mark mark = null );
		void SubmitOrderShipmentList( IEnumerable< OrderShipment > orderShipments, Mark mark = null );
		Task SubmitOrderShipmentListAsync( IEnumerable< OrderShipment > orderShipments, Mark mark = null );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, Mark mark = null );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList, Mark mark = null );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( string[] clientOrderIdentifierList, Mark mark = null );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( string[] clientOrderIdentifierList, Mark mark = null );
		OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, string[] clientOrderIdentifierList, Mark mark = null );
		Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList, string[] clientOrderIdentifierList, Mark mark = null );
		ShippingCarrier[] GetShippingCarrierList( Mark mark = null );
		Task< ShippingCarrier[] > GetShippingCarrierListAsync( Mark mark = null );
	}
}