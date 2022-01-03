using System;
using System.Diagnostics;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Admin;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Admin
{
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