using FluentAssertions;
using NUnit.Framework;
using System.Linq;

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
			dcList.Select(dc => dc.DistributionCenterCode).Should().Contain(TestDistributionCenterCode);
		}
	}
}