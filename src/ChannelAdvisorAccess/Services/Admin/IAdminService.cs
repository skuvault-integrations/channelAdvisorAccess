using System;
using System.Threading.Tasks;
using ChannelAdvisorAccess.AdminService;
using ChannelAdvisorAccess.Misc;

namespace ChannelAdvisorAccess.Services.Admin
{
	public interface IAdminService : IDisposable
	{
		/// <summary>
		/// Gets all authorization records
		/// </summary>
		/// <param name="mark"></param>
		/// <returns>List of authorization records</returns>
		AuthorizationResponse[] GetAuthorizationList( Mark mark = null );

		/// <summary>
		/// Gets authorization record which corresponds to the specified ChannelAdvisor Account
		/// </summary>
		/// <param name="localId">A number which uniquely identifies a ChannelAdvisor Account</param>
		/// <param name="mark"></param>
		/// <returns>List with a single authorization record</returns>
		AuthorizationResponse[] GetAuthorizationList( string localId, Mark mark = null );

		/// <summary>
		/// Allows to request access to a specific ChannelAdvisor Account
		/// </summary>
		/// <param name="localId">A number which uniquely identifies a ChannelAdvisor Account</param>
		/// <param name="mark"></param>
		/// <returns>True if the request was created. False otherwise</returns>
		bool RequestAccess( int localId, Mark mark = null );

		void Ping( Mark mark = null );
		Task PingAsync( Mark mark = null );
		Task< AuthorizationResponse[] > GetAuthorizationListAsync( Mark mark = null );
		Task< AuthorizationResponse[] > GetAuthorizationListAsync( string localId, Mark mark = null );
		Task< bool > RequestAccessAsync( int localId, Mark mark = null );
	}
}
