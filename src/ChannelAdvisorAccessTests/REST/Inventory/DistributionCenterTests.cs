using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccessTests.REST.Inventory
{
	public class DistributionCenterTests : RestAPITestBase
	{
		protected const string TestDistributionCenterCode = "Louisville";

		[ Test ]
		public void GetDistributionCenterList()
		{
			var result = this.ItemsService.GetDistributionCenterListAsync( null ).GetAwaiter().GetResult();

			result.Count().Should().BeGreaterThan( 4 );
		}

		[ Test ]
		public void CheckTestDistributionCenterExistence()
		{
			var result = this.ItemsService.GetDistributionCenterListAsync( null ).GetAwaiter().GetResult();

			result.Select( dc => dc.DistributionCenterCode ).Should().Contain( TestDistributionCenterCode );
		}
	}
}
