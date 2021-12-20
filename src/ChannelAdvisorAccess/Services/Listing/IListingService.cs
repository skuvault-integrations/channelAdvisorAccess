using ChannelAdvisorAccess.Misc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.Services.Listing
{
	/// <summary>
	/// Service to work with CA listings.
	/// </summary>
	public interface IListingService
	{
		void Ping( Mark mark = null );
		Task PingAsync( Mark mark = null );
		void WithdrawListing( IList< string > itemSkus, string withdrawReason, Mark mark = null );
		Task WithdrawListingAsync( IList< string > itemSkus, string withdrawReason, Mark mark = null );
	}
}