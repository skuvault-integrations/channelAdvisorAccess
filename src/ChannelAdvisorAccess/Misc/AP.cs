using System;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Logging;
using Netco.Utils;

namespace ChannelAdvisorAccess.Misc
{
	public static class AP
	{
		public static ActionPolicy Query
		{
			get { return _query; }
		}

		private static readonly ActionPolicy _query = ActionPolicy.Handle< Exception >().Retry( 10, ( ex, i ) =>
			{
				typeof( AP ).Log().Trace( ex, "Retrying CA API get call for the {0} time", i );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 0.5 + i ) );
			} );

		public static ActionPolicyAsync QueryAsync
		{
			get { return _queryAsync; }
		}

		private static readonly ActionPolicyAsync _queryAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( 10, async ( ex, i ) =>
			{
				typeof( AP ).Log().Trace( ex, "Retrying CA API get call for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) );
			} );

		public static ActionPolicy Submit
		{
			get { return _sumbit; }
		}

		private static readonly ActionPolicy _sumbit = ActionPolicy.Handle< Exception >().Retry( 10, ( ex, i ) =>
			{
				typeof( AP ).Log().Trace( ex, "Retrying CA API submit call for the {0} time", i );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 0.5 + i ) );
			} );

		public static ActionPolicyAsync SubmitAsync
		{
			get { return _sumbitAsync; }
		}

		private static readonly ActionPolicyAsync _sumbitAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( 10, async ( ex, i ) =>
			{
				typeof( AP ).Log().Trace( ex, "Retrying CA API submit call for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.5 + i ) );
			} );
	}
}