using System;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Listing;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Listing
{
	[ Explicit ]
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
				Assert.That( service.Disposed, Is.False ); // not be disposed yet
			}

			try
			{
				Assert.That( service.Disposed, Is.True ); // expecting an exception.
			}
			catch ( Exception ex )
			{
				Assert.That( ex, Is.TypeOf<ObjectDisposedException>() );
			}
		}
	}
}