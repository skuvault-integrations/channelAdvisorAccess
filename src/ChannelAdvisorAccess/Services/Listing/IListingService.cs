using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.Services.Listing
{
	/// <summary>
	/// Service to work with CA listings.
	/// </summary>
	public interface IListingService
	{
		void Ping();
		Task PingAsync();
		void WithdrawListing( IList< string > itemSkus, string withdrawReason );
		Task WithdrawListingAsync( IList< string > itemSkus, string withdrawReason );
	}
}