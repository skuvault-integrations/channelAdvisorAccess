using System;
using System.Threading.Tasks;
using ChannelAdvisorAccess.AdminService;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.Services.Items;

namespace ChannelAdvisorAccess.Services.Admin
{
	public class AdminService: ServiceBaseAbstr, IAdminService, IDisposable
	{
		private readonly APICredentials _credentials;
		private readonly AdminServiceSoapClient _client;
				
		public AdminService( APICredentials credentials )
		{
			this._credentials = credentials;
			this._client = new AdminServiceSoapClient();
		}

		#region Ping
		public void Ping( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();

			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				AP.CreateQuery( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var result = this._client.Ping( this._credentials );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( result );					
				} );

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task PingAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var result = await this._client.PingAsync( this._credentials ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( result.PingResult );					
				} ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}
		#endregion

		public AuthorizationResponse[] GetAuthorizationList( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				var authorizationResponses = this.GetAuthorizationList( string.Empty, mark );

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : authorizationResponses.ToJson(), additionalInfo : this.AdditionalLogInfo() ) );

				return authorizationResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< AuthorizationResponse[] > GetAuthorizationListAsync( Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );

				var authorizationResponses = await this.GetAuthorizationListAsync( string.Empty, mark ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : authorizationResponses.ToJson(), additionalInfo : this.AdditionalLogInfo() ) );

				return authorizationResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public AuthorizationResponse[] GetAuthorizationList( string localId, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : localId ) );

				var authorizationResponses = AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var result = this._client.GetAuthorizationList( this._credentials, localId );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( result );
					return result.ResultData;
				} );

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : authorizationResponses.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : localId ) );

				return authorizationResponses;
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< AuthorizationResponse[] > GetAuthorizationListAsync( string localId, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : localId ) );

				var authorizationResponses = await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var result = await this._client.GetAuthorizationListAsync( this._credentials, localId ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( result.GetAuthorizationListResult );
					return result.GetAuthorizationListResult.ResultData;
				} ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : authorizationResponses.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : localId ) );

				return authorizationResponses;
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public bool RequestAccess( int localId, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : localId.ToString() ) );

				var requestAccess = AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var result = this._client.RequestAccess( this._credentials, localId );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( result );
					return result.ResultData;
				} );
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : requestAccess.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : localId.ToString() ) );

				return requestAccess;
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		public async Task< bool > RequestAccessAsync( int localId, Mark mark = null )
		{
			if( mark.IsBlank() )
				mark = Mark.CreateNew();
			try
			{
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo(), methodParameters : localId.ToString() ) );

				var requestAsyncResult = await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					var result = await this._client.RequestAccessAsync( this._credentials, localId ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					ChannelAdvisorLogger.LogTraceRetryEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ) );
					this.CheckCaSuccess( result.RequestAccessResult );
					return result.RequestAccessResult.ResultData;
				} ).ConfigureAwait( false );
				;
				ChannelAdvisorLogger.LogEnd( this.CreateMethodCallInfo( mark : mark, methodResult : requestAsyncResult.ToJson(), additionalInfo : this.AdditionalLogInfo(), methodParameters : localId.ToString() ) );

				return requestAsyncResult;
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfo() ), exception );
				ChannelAdvisorLogger.LogTraceException( channelAdvisorException );
				throw channelAdvisorException;
			}
		}

		private void CheckCaSuccess( APIResultOfString results )
		{
			if( results.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( results.MessageCode, results.Message );
		}

		private void CheckCaSuccess( APIResultOfArrayOfAuthorizationResponse results )
		{
			if( results.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( results.MessageCode, results.Message );
		}

		private void CheckCaSuccess( APIResultOfBoolean results )
		{
			if( results.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( results.MessageCode, results.Message );
		}

		#region IDisposable implementation

		public void Dispose()
		{
			Dispose( _client, true );
			GC.SuppressFinalize( this );
		}

		#endregion
	}
}