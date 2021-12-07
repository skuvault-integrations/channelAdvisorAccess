﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChannelAdvisorAccess.AdminService;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.Services.Items;
using Newtonsoft.Json;

namespace ChannelAdvisorAccess.Services.Admin
{
	public class AdminService: ServiceBaseAbstr, IAdminService
	{
		private readonly APICredentials _credentials;
		private readonly AdminServiceSoapClient _client;
		
		private string AdditionalLogInfoString
		{
			get
			{
				if( this.AdditionalLogInfo == null )
					return string.Empty;

				string res;
				try
				{
					res = this.AdditionalLogInfo();
				}
				catch
				{
					return string.Empty;
				}

				return res;
			}
		}

		private string CreateMethodCallInfo( string methodParameters = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", string notes = "", [ CallerMemberName ] string memberName = "" )
		{
			try
			{
				mark = mark ?? Mark.Blank();
				var connectionInfo = this.ToJson();
				var str = string.Format(
					"{{Mark:\"{3}\", MethodName:{0}, ConnectionInfo:{1}, MethodParameters:{2} {4}{5}{6}{7}}}",
					memberName,
					connectionInfo,
					string.IsNullOrWhiteSpace( methodParameters ) ? PredefinedValues.EmptyJsonObject : methodParameters,
					mark,
					string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors,
					string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult,
					string.IsNullOrWhiteSpace( notes ) ? string.Empty : ",Notes: " + notes,
					string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", " + additionalInfo
					);
				return str;
			}
			catch( Exception )
			{
				return PredefinedValues.EmptyJsonObject;
			}
		}

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
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				AP.CreateQuery( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = this._client.Ping( this._credentials );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result );
				} );

				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
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
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				await AP.CreateQueryAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Do( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = await this._client.PingAsync( this._credentials ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result.PingResult );
				} ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
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
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				var authorizationResponses = this.GetAuthorizationList( string.Empty, mark );

				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : authorizationResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );

				return authorizationResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
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
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ) );

				var authorizationResponses = await this.GetAuthorizationListAsync( string.Empty, mark ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : authorizationResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString ) );

				return authorizationResponses;
			}
			catch( Exception exception )
			{
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
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
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : localId ) );

				var authorizationResponses = AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = this._client.GetAuthorizationList( this._credentials, localId );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result );
					return result.ResultData;
				} );

				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : authorizationResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : localId ) );

				return authorizationResponses;
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
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
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : localId ) );

				var authorizationResponses = await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = await this._client.GetAuthorizationListAsync( this._credentials, localId ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result.GetAuthorizationListResult );
					return result.GetAuthorizationListResult.ResultData;
				} ).ConfigureAwait( false );

				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : authorizationResponses.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : localId ) );

				return authorizationResponses;
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
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
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : localId.ToString() ) );

				var requestAccess = AP.CreateSubmit( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = this._client.RequestAccess( this._credentials, localId );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result );
					return result.ResultData;
				} );
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : requestAccess.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : localId.ToString() ) );

				return requestAccess;
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
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
				ChannelAdvisorLogger.LogStarted( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString, methodParameters : localId.ToString() ) );

				var requestAsyncResult = await AP.CreateSubmitAsync( ExtensionsInternal.CreateMethodCallInfo( this.AdditionalLogInfo ) ).Get( async () =>
				{
					this.RefreshLastNetworkActivityTime();
					var result = await this._client.RequestAccessAsync( this._credentials, localId ).ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					this.CheckCaSuccess( result.RequestAccessResult );
					return result.RequestAccessResult.ResultData;
				} ).ConfigureAwait( false );
				;
				ChannelAdvisorLogger.LogTraceEnd( this.CreateMethodCallInfo( mark : mark, methodResult : requestAsyncResult.ToJson(), additionalInfo : this.AdditionalLogInfoString, methodParameters : localId.ToString() ) );

				return requestAsyncResult;
			}
			catch( Exception exception )
			{
				this.RefreshLastNetworkActivityTime();
				var channelAdvisorException = new ChannelAdvisorException( this.CreateMethodCallInfo( mark : mark, additionalInfo : this.AdditionalLogInfoString ), exception );
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
	}
}