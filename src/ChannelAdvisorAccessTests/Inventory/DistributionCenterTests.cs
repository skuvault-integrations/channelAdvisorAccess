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
			var dcList = this.ItemsService.GetDistributionCenterList();

			//------------ Assert
			dcList.Length.Should().BeGreaterThan( 0 );
			dcList[ 0 ].DistributionCenterCode.Should().Be( TestDistributionCenterCode );
		}
	}
}