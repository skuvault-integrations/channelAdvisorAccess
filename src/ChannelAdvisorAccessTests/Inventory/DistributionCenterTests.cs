using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace ChannelAdvisorAccessTests.Inventory
{
	public class DistributionCenterTests : TestsBase
	{
		[ Test ]
		public void GetDistributionCenterList()
		{
			//------------ Arrange

			//------------ Act
			var dcList = this.ItemsService.GetDistributionCenterList( CancellationToken.None );

			//------------ Assert
			dcList.Length.Should().BeGreaterThan( 0 );
			dcList.Select( dc => dc.DistributionCenterCode ).Should().Contain( TestDistributionCenterCode );
			this.ItemsService.LastActivityTime.Should().NotBe( this.serviceLastActivityDateTime );
		}
	}
}