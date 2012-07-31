using System.Collections.Generic;
using ChannelAdvisorAccess.ListingService;

namespace ChannelAdvisorAccess.Services.Listing
{
	/// <summary>
	/// Service to work with CA listings.
	/// </summary>
	public interface IListingService
	{
		/// <summary>Ends the listings for the specified item.</summary>
		/// <param name="itemSkus">The item skus.</param>
		/// <param name="withdrawReason">The reason.</param>
		/// <remarks>Automatically ends listing for all account for the specified SKUs.</remarks>
		void WithdrawListing( IList< string > itemSkus, string withdrawReason ); 
	}
}