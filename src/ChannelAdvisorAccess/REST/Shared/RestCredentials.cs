using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace ChannelAdvisorAccess.REST.Shared
{
	public sealed class RestCredentials
	{
		public string ApplicationId { get; private set; }
		public string SharedSecret { get; private set; }

		public RestCredentials( string applicationId, string sharedSecret )
		{
			Condition.Requires( applicationId ).IsNotNullOrEmpty();
			Condition.Requires( sharedSecret ).IsNotNullOrEmpty();

			this.ApplicationId = applicationId;
			this.SharedSecret = sharedSecret;
		}
	}
}
