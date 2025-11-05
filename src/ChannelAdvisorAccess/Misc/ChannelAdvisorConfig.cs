using System;

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

		public ChannelAdvisorConfig( string accountName ) {
			if( accountName == null )
			{
				throw new ArgumentNullException( nameof(accountName), "accountName must not be null" );
			}

			this.AccountName = accountName;
		}
	}

	public enum ChannelAdvisorApiVersion { Soap, Rest }
}
