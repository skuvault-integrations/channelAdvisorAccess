using System.Threading.Tasks;
using ChannelAdvisorAccess.AdminService;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;

namespace ChannelAdvisorAccess.Services.Admin
{
	public class AdminService : IAdminService
	{
		private readonly APICredentials _credentials;
		private readonly AdminServiceSoapClient _client;

		public AdminService( APICredentials credentials )
		{
			this._credentials = credentials;
			this._client = new AdminServiceSoapClient();
		}

		#region Ping
		public void Ping()
		{
			AP.Query.Do( () =>
				{
					var result = this._client.Ping( this._credentials );
					CheckCaSuccess( result );
				} );
		}

		public async Task PingAsync()
		{
			await AP.QueryAsync.Do( async () =>
				{
					var result = await this._client.PingAsync( this._credentials );
					CheckCaSuccess( result.PingResult );
				} );
		}
		#endregion

		public AuthorizationResponse[] GetAuthorizationList()
		{
			return this.GetAuthorizationList( string.Empty );
		}

		public async Task< AuthorizationResponse[] > GetAuthorizationListAsync()
		{
			return await this.GetAuthorizationListAsync( string.Empty );
		}

		public AuthorizationResponse[] GetAuthorizationList( string localId )
		{
			return AP.Submit.Get( () =>
				{
					var result = this._client.GetAuthorizationList( this._credentials, localId );
					this.CheckCaSuccess( result );
					return result.ResultData;
				} );
		}

		public async Task< AuthorizationResponse[] > GetAuthorizationListAsync( string localId )
		{
			return await AP.SubmitAsync.Get( async () =>
				{
					var result = await this._client.GetAuthorizationListAsync( this._credentials, localId );
					this.CheckCaSuccess( result.GetAuthorizationListResult );
					return result.GetAuthorizationListResult.ResultData;
				} );
		}

		public bool RequestAccess( int localId )
		{
			return AP.Submit.Get( () =>
				{
					var result = this._client.RequestAccess( this._credentials, localId );
					this.CheckCaSuccess( result );
					return result.ResultData;
				} );
		}

		public async Task< bool > RequestAccessAsync( int localId )
		{
			return await AP.SubmitAsync.Get( async () =>
				{
					var result = await this._client.RequestAccessAsync( this._credentials, localId );
					this.CheckCaSuccess( result.RequestAccessResult );
					return result.RequestAccessResult.ResultData;
				} );
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