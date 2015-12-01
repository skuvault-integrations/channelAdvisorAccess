using System;

namespace ChannelAdvisorAccess.Services
{
	[ Flags ]
	public enum LogDetailsEnum
	{
		Undefined = 0x0,
		LogParametersAndResultForRetry = 0x1,
		LogParametersAndReturnsForTrace = 0x2,
	}
}