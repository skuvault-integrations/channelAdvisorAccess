using ChannelAdvisorAccess.REST.Shared;
using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.Misc
{
	public class ChannelAdvisorConfig
	{
		public string AccountName { get; set; }
		public ChannelAdvisorApiVersion ApiVersion { get; set; }
		public string AccountId { get; set; }
		public bool SoapCompatibilityAuth { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public ChannelAdvisorTimeouts Timeouts { get; private set; }

		public ChannelAdvisorConfig( string accountName, ChannelAdvisorTimeouts timeouts ) {
			Condition.Requires( accountName, "accountName" );

			this.AccountName = accountName;
			this.Timeouts = timeouts;
		}

		public ChannelAdvisorConfig( string accountName ) : this( accountName, new ChannelAdvisorTimeouts() ) { }
	}

	public enum ChannelAdvisorApiVersion { Soap, Rest }
}
