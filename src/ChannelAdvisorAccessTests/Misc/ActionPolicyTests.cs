using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.REST.Shared;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ChannelAdvisorAccessTests.Misc
{
	[ TestFixture ]
	public class ActionPolicyTests
	{
		[ Test ]
		public void WaitRetryTimeWhenRestApiIsUsedShouldntExceedTenMinutes()
		{
			var actionPolicy = new ActionPolicy( 10 );
			var retryWaitTime = actionPolicy.GetDelayBeforeNextAttempt( 10 );

			retryWaitTime.Should().Be( TimeSpan.FromMinutes( 10 ) );
		}

		[ Test ]
		public void WaitRetryTimeWhenSoapApiIsUsedShouldntExceedTenMinutes()
		{
			var attemptNumber = 10;
			var retryWaitTime = AP.GetDelayFor429Exception( attemptNumber );

			retryWaitTime.Should().Be( TimeSpan.FromMinutes( 10 ) );
		}
	}
}