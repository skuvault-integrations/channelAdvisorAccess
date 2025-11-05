using System;

namespace ChannelAdvisorAccess.REST.Shared
{
	public sealed class RestCredentials
	{
		public string ApplicationId { get; private set; }
		public string SharedSecret { get; private set; }

		public RestCredentials( string applicationId, string sharedSecret )
		{
			if( string.IsNullOrEmpty( applicationId ) )
			{
				throw new ArgumentException( "applicationId must not be null or empty", nameof(applicationId) );
			}

			if( string.IsNullOrEmpty( sharedSecret ) )
			{
				throw new ArgumentException( "sharedSecret must not be null or empty", nameof(sharedSecret) );
			}

			this.ApplicationId = applicationId;
			this.SharedSecret = sharedSecret;
		}
	}
}
