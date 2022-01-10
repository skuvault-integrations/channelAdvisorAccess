using ChannelAdvisorAccess.Misc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.Services.Listing
{
	/// <summary>
	/// Service to work with CA listings.
	/// </summary>
	public interface IListingService : IDisposable
	{
		void Ping( Mark mark );
		Task PingAsync( Mark mark );
		void WithdrawListing( IList< string > itemSkus, string withdrawReason, Mark mark );
		Task WithdrawListingAsync( IList< string > itemSkus, string withdrawReason, Mark mark );
	}
}