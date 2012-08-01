using ChannelAdvisorAccess.AdminService;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using APICredentials = ChannelAdvisorAccess.AdminService.APICredentials;

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

		public AuthorizationResponse[] GetAuthorizationList()
		{
			return this.GetAuthorizationList( string.Empty );
		}

		public AuthorizationResponse[] GetAuthorizationList( string localId )
		{
			return ActionPolicies.CaSubmitPolicy.Get( () =>
				{
					var result = this._client.GetAuthorizationList( this._credentials, localId );
					this.CheckCaSuccess( result );
					return result.ResultData;
				} );
		}

		public bool RequestAccess( int localId )
		{
			return ActionPolicies.CaSubmitPolicy.Get( () =>
				{
					var result = this._client.RequestAccess( this._credentials, localId );
					this.CheckCaSuccess( result );
					return result.ResultData;
				} );
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
