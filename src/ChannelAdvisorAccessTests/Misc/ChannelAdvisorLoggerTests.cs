using FluentAssertions;
using NUnit.Framework;
using ChannelAdvisorAccess.Misc;

namespace ChannelAdvisorAccessTests.Misc
{
	[ TestFixture ]
	public class ChannelAdvisorLoggerTests
	{
		[ Test ]
		public void SanitizeToken_Long()
		{
			string token = "12345689012345";

			var result = ChannelAdvisorLogger.SanitizeToken( token );

			result.Should().Be( "1234****2345" );
		}

		[ Test ]
		public void SanitizeToken_Short()
		{
			string token = "123456";

			var result = ChannelAdvisorLogger.SanitizeToken( token );

			result.Should().Be( "1234****3456" );
		}
	}
}
