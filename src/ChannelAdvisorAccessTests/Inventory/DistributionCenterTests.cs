using FluentAssertions;
using NUnit.Framework;

namespace ChannelAdvisorAccessTests.Inventory
{
	public class DistributionCenterTests : TestsBase
	{
		[ Test ]
		public void GetDistributionCenterList()
		{
			//------------ Arrange

			//------------ Act
			var dcList = ItemsService.GetDistributionCenterList();

			//------------ Assert
			dcList.Should().HaveCount( 1 );
			dcList[ 0 ].DistributionCenterCode.Should().Be( TestDistributionCenterCode );
		}
	}
}