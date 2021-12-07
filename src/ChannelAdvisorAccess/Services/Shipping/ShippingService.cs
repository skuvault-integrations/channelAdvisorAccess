using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.ShippingService;
using Netco.Extensions;
using Netco.Logging;
using Newtonsoft.Json;

namespace ChannelAdvisorAccess.Services.Shipping
{
	public class ShippingService: ServiceBaseAbstr, IShippingService
	{
		private readonly APICredentials _credentials;
		private readonly ShippingServiceSoapClient _client;
		
		public ShippingService( APICredentials credentials, string accountId )
		{
			this._credentials = credentials;
			this.AccountId = accountId;
			this._client = new ShippingServiceSoapClient();
		}

		public ShippingService( APICredentials credentials, string name, string id ): this( credentials, id )
		{
			this.Name = name;
		}

		public string Name{ get; private set; }
		public string AccountId{ get; private set; }

		#region Ping
		public void Ping()
		{
			AP.CreateQuery( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
			{
				this.RefreshLastNetworkActivityTime();
				var result = this._client.Ping( this._credentials );
				this.RefreshLastNetworkActivityTime();
				this.CheckCaSuccess( result );
			} );
		}

		public async Task PingAsync()
		{
			await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
			{
				this.RefreshLastNetworkActivityTime();
				var result = await this._client.PingAsync( this._credentials ).ConfigureAwait( false );
				this.RefreshLastNetworkActivityTime();
				this.CheckCaSuccess( result.PingResult );
			} ).ConfigureAwait( false );
		}
		#endregion

		#region Mark Order Shipped
		public void MarkOrderShipped( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, CreateShipmentByOrderId( orderId, carrierCode, classCode, trackingNumber, dateShipped ) );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result );
				} );
			}
			catch( Exception e )
			{
				this.RefreshLastNetworkActivityTime();
				throw new MarkOrderShippedException( orderId.ToString( CultureInfo.InvariantCulture ), this.AccountId, carrierCode, classCode, trackingNumber, e );
			}
		}

		public async Task MarkOrderShippedAsync( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, CreateShipmentByOrderId( orderId, carrierCode, classCode, trackingNumber, dateShipped ) ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result.SubmitOrderShipmentListResult );
				} ).ConfigureAwait( false );
			}
			catch( Exception e )
			{
				this.RefreshLastNetworkActivityTime();
				throw new MarkOrderShippedException( orderId.ToString( CultureInfo.InvariantCulture ), this.AccountId, carrierCode, classCode, trackingNumber, e );
			}
		}

		private static OrderShipment[] CreateShipmentByOrderId( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			return new[]
			{
				new OrderShipment
				{
					OrderID = orderId,
					ShipmentType = "Full",
					FullShipment = new FullShipmentContents
					{
						CarrierCode = carrierCode,
						ClassCode = classCode,
						DateShippedGMT = dateShipped.ToUniversalTime(),
						TrackingNumber = trackingNumber
					}
				}
			};
		}

		public void MarkOrderShipped( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, CreateShipmentByClientId( clientOrderId, carrierCode, classCode, trackingNumber, dateShipped ) );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result );
				} );
			}
			catch( Exception e )
			{
				this.RefreshLastNetworkActivityTime();
				throw new MarkOrderShippedException( clientOrderId, this.AccountId, carrierCode, classCode, trackingNumber, e );
			}
		}

		public async Task MarkOrderShippedAsync( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, CreateShipmentByClientId( clientOrderId, carrierCode, classCode, trackingNumber, dateShipped ) ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result.SubmitOrderShipmentListResult );
				} ).ConfigureAwait( false );
			}
			catch( Exception e )
			{
				this.RefreshLastNetworkActivityTime();
				throw new MarkOrderShippedException( clientOrderId, this.AccountId, carrierCode, classCode, trackingNumber, e );
			}
		}

		private static OrderShipment[] CreateShipmentByClientId( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			return new[]
			{
				new OrderShipment
				{
					ClientOrderIdentifier = clientOrderId,
					ShipmentType = "Full",
					FullShipment = new FullShipmentContents
					{
						CarrierCode = carrierCode,
						ClassCode = classCode,
						DateShippedGMT = dateShipped.ToUniversalTime(),
						TrackingNumber = trackingNumber
					}
				}
			};
		}

		public void MarkOrderShipped( int orderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, CreatePartialShipmentByOrderId( orderId, partialShipmentContents ) );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result );
				} );
			}
			catch( Exception e )
			{
				this.RefreshLastNetworkActivityTime();
				throw new MarkOrderShippedException( orderId.ToString( CultureInfo.InvariantCulture ), this.AccountId, partialShipmentContents.CarrierCode, partialShipmentContents.ClassCode, partialShipmentContents.TrackingNumber, e );
			}
		}

		public async Task MarkOrderShippedAsync( int orderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, CreatePartialShipmentByOrderId( orderId, partialShipmentContents ) ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result.SubmitOrderShipmentListResult );
				} ).ConfigureAwait( false );
			}
			catch( Exception e )
			{
				this.RefreshLastNetworkActivityTime();
				throw new MarkOrderShippedException( orderId.ToString( CultureInfo.InvariantCulture ), this.AccountId, partialShipmentContents.CarrierCode, partialShipmentContents.ClassCode, partialShipmentContents.TrackingNumber, e );
			}
		}

		private static OrderShipment[] CreatePartialShipmentByOrderId( int orderId, PartialShipmentContents partialShipmentContents )
		{
			return new[]
			{
				new OrderShipment
				{
					OrderID = orderId,
					ShipmentType = "Partial",
					PartialShipment = partialShipmentContents
				}
			};
		}

		public void MarkOrderShipped( string clientOrderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, CreatePartialShipmentByClientId( clientOrderId, partialShipmentContents ) );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result );
				} );
			}
			catch( Exception e )
			{
				this.RefreshLastNetworkActivityTime();
				throw new MarkOrderShippedException( clientOrderId, this.AccountId, partialShipmentContents.CarrierCode, partialShipmentContents.ClassCode, partialShipmentContents.TrackingNumber, e );
			}
		}

		public async Task MarkOrderShippedAsync( string clientOrderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, CreatePartialShipmentByClientId( clientOrderId, partialShipmentContents ) ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result.SubmitOrderShipmentListResult );
				} ).ConfigureAwait( false );
			}
			catch( Exception e )
			{
				this.RefreshLastNetworkActivityTime();
				throw new MarkOrderShippedException( clientOrderId, this.AccountId, partialShipmentContents.CarrierCode, partialShipmentContents.ClassCode, partialShipmentContents.TrackingNumber, e );
			}
		}

		private static OrderShipment[] CreatePartialShipmentByClientId( string clientOrderId, PartialShipmentContents partialShipmentContents )
		{
			return new[]
			{
				new OrderShipment
				{
					ClientOrderIdentifier = clientOrderId,
					ShipmentType = "Partial",
					PartialShipment = partialShipmentContents
				}
			};
		}
		#endregion

		public void SubmitOrderShipmentList( IEnumerable< OrderShipment > orderShipments )
		{
			orderShipments.DoWithPages( 50, p => AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
			{
				this.RefreshLastNetworkActivityTime();
				var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, p.ToArray() );
				this.RefreshLastNetworkActivityTime();
				this.CheckCaSuccess( result );
			} ) );
		}

		public async Task SubmitOrderShipmentListAsync( IEnumerable< OrderShipment > orderShipments )
		{
			await orderShipments.DoWithPagesAsync( 50, async p => await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
			{
				this.RefreshLastNetworkActivityTime();
				var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, p.ToArray() ).ConfigureAwait( false );
				this.RefreshLastNetworkActivityTime();
				this.CheckCaSuccess( result.SubmitOrderShipmentListResult );
			} ).ConfigureAwait( false ) ).ConfigureAwait( false );
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList )
		{
			return this.GetOrderShipmentHistoryList( orderIdList, new string[] { } );
		}

		public async Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList )
		{
			return await this.GetOrderShipmentHistoryListAsync( orderIdList, new string[] { } ).ConfigureAwait( false );
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( string[] clientOrderIdentifierList )
		{
			return this.GetOrderShipmentHistoryList( new int[] { }, clientOrderIdentifierList );
		}

		public async Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( string[] clientOrderIdentifierList )
		{
			return await this.GetOrderShipmentHistoryListAsync( new int[] { }, clientOrderIdentifierList ).ConfigureAwait( false );
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, string[] clientOrderIdentifierList )
		{
			return AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
			{
				this.RefreshLastNetworkActivityTime();
				var result = this._client.GetOrderShipmentHistoryList( this._credentials, this.AccountId, orderIdList, clientOrderIdentifierList );
				this.RefreshLastNetworkActivityTime();
				this.CheckCaSuccess( result );
				return result.ResultData;
			} );
		}

		public async Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList, string[] clientOrderIdentifierList )
		{
			return await AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
			{
				this.RefreshLastNetworkActivityTime();
				var result = await this._client.GetOrderShipmentHistoryListAsync( this._credentials, this.AccountId, orderIdList, clientOrderIdentifierList ).ConfigureAwait( false );
				this.RefreshLastNetworkActivityTime();
				this.CheckCaSuccess( result.GetOrderShipmentHistoryListResult );
				return result.GetOrderShipmentHistoryListResult.ResultData;
			} ).ConfigureAwait( false );
		}

		public ShippingCarrier[] GetShippingCarrierList()
		{
			this.RefreshLastNetworkActivityTime();
			var result = this._client.GetShippingCarrierList( this._credentials, this.AccountId );
			this.RefreshLastNetworkActivityTime();
			this.CheckCaSuccess( result );
			return result.ResultData;
		}

		public async Task< ShippingCarrier[] > GetShippingCarrierListAsync()
		{
			this.RefreshLastNetworkActivityTime();
			var result = await this._client.GetShippingCarrierListAsync( this._credentials, this.AccountId ).ConfigureAwait( false );
			this.RefreshLastNetworkActivityTime();
			this.CheckCaSuccess( result.GetShippingCarrierListResult );
			return result.GetShippingCarrierListResult.ResultData;
		}

		private void CheckCaSuccess( APIResultOfString results )
		{
			if( results.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( results.MessageCode, results.Message );
		}

		private void CheckCaSuccess( APIResultOfArrayOfShipmentResponse result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
			ChannelAdvisorException exceptionToThrow = null;
			foreach( var shipmentResponse in result.ResultData )
			{
				if( !shipmentResponse.Success )
				{
					ChannelAdvisorLogger.LogTrace( string.Format( "Error encountered while marking order shipped: {0}", shipmentResponse.Message ) );
					if( exceptionToThrow == null )
						exceptionToThrow = new ChannelAdvisorException( shipmentResponse.Message );
				}
			}
			if( exceptionToThrow != null )
				throw exceptionToThrow;
		}

		private void CheckCaSuccess( APIResultOfArrayOfShippingCarrier result )
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