using Newtonsoft.Json;

namespace ChannelAdvisorAccess.REST.Models.Infrastructure
{
	public sealed class OAuthResponse
	{
		public string Error { get; set; }
		[ JsonProperty( PropertyName = "access_token" ) ]
		public string AccessToken { get; set; }
		[ JsonProperty( PropertyName = "expires_in" ) ]
		public int ExpiresIn { get; set; }
	}
}
