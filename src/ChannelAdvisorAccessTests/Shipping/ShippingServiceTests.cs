using System;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Shipping;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Shipping
{
	[ Explicit ]
	[TestFixture]
	public class ShippingServiceTests : TestsBase
	{
		[Test]
		public void ShippingService_IsDisposable()
		{
			var factory = new ChannelAdvisorServicesFactory( Credentials.DeveloperKey, Credentials.DeveloperPassword, null, null );
			ShippingService service;

			using ( service = ( ShippingService )factory.CreateShippingService( "test", Credentials.AccountId ) )
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