using System;
using System.Diagnostics;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Listing;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Listing
{
	[TestFixture]
	public class ListingServiceTests : TestsBase
	{
		[Test]
		public void ListingService_IsDisposable()
		{
			var factory = new ChannelAdvisorServicesFactory( Credentials.DeveloperKey, Credentials.DeveloperPassword, null, null );
			ListingService service;

			using ( service = ( ListingService )factory.CreateListingService( "test", Credentials.AccountId ) )
			{
				Debug.Assert( !service.Disposed ); // not be disposed yet
			}

			try
			{
				Debug.Assert( service.Disposed ); // expecting an exception.
			}
			catch ( Exception ex )
			{
				Debug.Assert( ex is ObjectDisposedException );
			}
		}
	}
}