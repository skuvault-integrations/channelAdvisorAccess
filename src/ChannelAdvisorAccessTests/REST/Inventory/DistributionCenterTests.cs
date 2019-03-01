using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccessTests.REST
{
	public class DistributionCenterTests : RestAPITestBase
	{
		protected const string TestDistributionCenterCode = "Louisville";

		[ Test ]
		public async void GetDistributionCenterList()
		{
			var result = await this.ItemsService.GetDistributionCenterListAsync(null);

			result.Should().HaveCount(4);
		}

		[ Test ]
		public async void CheckTestDistributionCenterExistence()
		{
			var result = await this.ItemsService.GetDistributionCenterListAsync(null);

			result.Select(dc => dc.DistributionCenterCode).Should().Contain(TestDistributionCenterCode);
		}
	}
}
