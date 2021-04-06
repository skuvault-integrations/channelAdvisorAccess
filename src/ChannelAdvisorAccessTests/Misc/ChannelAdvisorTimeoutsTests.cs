using ChannelAdvisorAccess.REST.Shared;
using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Misc
{
	[ TestFixture ]
	public class ChannelAdvisorTimeoutsTests
	{
		[ Test ]
		public void GivenSpecificTimeoutsAreNotSet_WhenGetTimeoutIsCalled_ThenDefaultTimeoutIsReturned()
		{
			var operationsTimeouts = new ChannelAdvisorTimeouts();

			operationsTimeouts[ ChannelAdvisorOperationEnum.ListOrdersRest ].Should().Be( operationsTimeouts.DefaultOperationTimeout.TimeoutInMs );
		}

		[ Test ]
		public void GivenOwnDefaultTimeoutValue_WhenGetTimeoutIsCalled_ThenOverridenDefaultTimeoutIsReturned()
		{
			var newDefaultTimeoutInMs = 10 * 60 * 1000;
			var operationsTimeouts = new ChannelAdvisorTimeouts( newDefaultTimeoutInMs );

			operationsTimeouts[ ChannelAdvisorOperationEnum.UpdateProductFieldsRest ].Should().Be( newDefaultTimeoutInMs );
		}

		[ Test ]
		public void GivenListOrdersTimeoutIsSet_WhenGetTimeoutIsCalled_ThenSpecificTimeoutIsReturned()
		{
			var operationsTimeouts = new ChannelAdvisorTimeouts();
			var specificTimeoutInMs = 10 * 60 * 1000;
			operationsTimeouts.Set( ChannelAdvisorOperationEnum.ListOrdersRest, new ChannelAdvisorOperationTimeout( specificTimeoutInMs ) );

			operationsTimeouts[ ChannelAdvisorOperationEnum.ListOrdersRest ].Should().Be( specificTimeoutInMs );
			operationsTimeouts[ ChannelAdvisorOperationEnum.UpdateProductFieldsRest ].Should().Be( operationsTimeouts.DefaultOperationTimeout.TimeoutInMs );
		}

		[ Test ]
		public void GivenListOrdersTimeoutIsSetTwice_WhenGetTimeoutIsCalled_ThenSpecificTimeoutIsReturned()
		{
			var operationsTimeouts = new ChannelAdvisorTimeouts();
			var specificTimeoutInMs = 10 * 60 * 1000;
			operationsTimeouts.Set( ChannelAdvisorOperationEnum.ListOrdersRest, new ChannelAdvisorOperationTimeout( specificTimeoutInMs ) );
			operationsTimeouts.Set( ChannelAdvisorOperationEnum.ListOrdersRest, new ChannelAdvisorOperationTimeout( specificTimeoutInMs * 2 ) );

			operationsTimeouts[ ChannelAdvisorOperationEnum.ListOrdersRest ].Should().Be( specificTimeoutInMs * 2 );
		}
	}
}