using System.Collections.Generic;
using ChannelAdvisorAccess.ListingService;
using ChannelAdvisorAccess.Misc;
using Netco.Extensions;

namespace ChannelAdvisorAccess.Services.Listing
{
	public class ListingService : IListingService
	{
		private readonly APICredentials _credentials;
		private readonly ListingServiceSoapClient _client;
		public string Name { get; private set; }
		public string AccountId{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ListingService"/> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="name">The account user-friendly name.</param>
		/// <param name="accountId">The account id.</param>
		public ListingService( APICredentials credentials, string name, string accountId )
		{
			this.Name = name;
			this.AccountId = accountId;
			this._credentials = credentials;
			this._client = new ListingServiceSoapClient();
		}

		public void WithdrawListing( IList< string > itemSkus, WithdrawReason reason )
		{
			if( itemSkus == null || itemSkus.Count == 0 )
				return;

			foreach( var skusSlice in itemSkus.Slice( 100 ))
			{
				string[] slice = skusSlice;
				ActionPolicies.CaSubmitPolicy.Do( () => _client.WithdrawListings( _credentials, AccountId, slice, null, reason ));
			}
		}
	}
}