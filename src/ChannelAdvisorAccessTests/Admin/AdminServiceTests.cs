using System;
using System.Diagnostics;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Admin;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Admin
{
	[ Explicit ]
	[ TestFixture ]
	public class AdminServiceTests : TestsBase
	{
		[ Test ]
		public void AdminService_IsDisposable()
		{
			var factory = new ChannelAdvisorServicesFactory( Credentials.DeveloperKey, Credentials.DeveloperPassword, null, null );
			AdminService service;

			using ( service = (AdminService)factory.CreateAdminService() )
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