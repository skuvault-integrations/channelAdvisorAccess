using System;
using System.Diagnostics;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Shipping;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Shipping
{
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