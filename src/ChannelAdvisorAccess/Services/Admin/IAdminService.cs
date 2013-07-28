using System.Threading.Tasks;
using ChannelAdvisorAccess.AdminService;

namespace ChannelAdvisorAccess.Services.Admin
{
	public interface IAdminService
	{
		/// <summary>
		/// Gets all authorization records
		/// </summary>
		/// <returns>List of authorization records</returns>
		AuthorizationResponse[] GetAuthorizationList();

		/// <summary>
		/// Gets authorization record which corresponds to the specified ChannelAdvisor Account
		/// </summary>
		/// <param name="localId">A number which uniquely identifies a ChannelAdvisor Account</param>
		/// <returns>List with a single authorization record</returns>
		AuthorizationResponse[] GetAuthorizationList( string localId );

		/// <summary>
		/// Allows to request access to a specific ChannelAdvisor Account
		/// </summary>
		/// <param name="localId">A number which uniquely identifies a ChannelAdvisor Account</param>
		/// <returns>True if the request was created. False otherwise</returns>
		bool RequestAccess( int localId );

		void Ping();
		Task PingAsync();
		Task< AuthorizationResponse[] > GetAuthorizationListAsync();
		Task< AuthorizationResponse[] > GetAuthorizationListAsync( string localId );
		Task< bool > RequestAccessAsync( int localId );
	}
}
