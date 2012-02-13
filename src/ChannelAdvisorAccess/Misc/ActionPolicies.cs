using System;
using FluentDateTime;
using Netco.ActionPolicyServices;
using Netco.Logging;
using Netco.Utils;

namespace ChannelAdvisorAccess.Misc
{
	public static class ActionPolicies
	{
		public static ActionPolicy CaGetPolicy
		{
			get { return _caGetPolicy; }
		}

		private static readonly ActionPolicy _caGetPolicy = ActionPolicy.Handle< Exception >().Retry(
			10, ( ex, i ) =>
				{
					typeof( ActionPolicies ).Log().Error( ex, "Retrying CA API get call for the {0} time", i );
					SystemUtil.Sleep( ( 0.5 + i ).Seconds() );
				} );

		public static ActionPolicy CaSubmitPolicy
		{
			get { return _caSumbitPolicy; }
		}

		private static readonly ActionPolicy _caSumbitPolicy = ActionPolicy.Handle< Exception >().Retry( 3, ( ex, i ) =>
				{
					typeof( ActionPolicies ).Log().Error( ex, "Retrying CA API submit call for the {0} time", i );
					SystemUtil.Sleep( ( 0.5 + i ).Seconds() );
				} );
	}
}