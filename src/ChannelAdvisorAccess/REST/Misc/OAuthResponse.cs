using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Misc
{
	public class OAuthResponse
	{
		public string Error { get; set; }
		[JsonProperty(PropertyName = "access_token")]
		public string AccessToken { get; set; }
		[JsonProperty(PropertyName = "expires_in")]
		public int ExpiresIn { get; set; }
	}
}
