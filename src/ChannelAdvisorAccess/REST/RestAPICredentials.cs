using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.Misc
{
	public class RestAPICredentials
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; private set; }
		public string ApplicationID { get; private set; }
		public string SharedSecret { get; private set; }

		public RestAPICredentials( string applicationID, string sharedSecret, string accessToken, string refreshToken )
		{
			ApplicationID = applicationID;
			SharedSecret = sharedSecret;
			AccessToken = accessToken;
			RefreshToken = refreshToken;
		}
	}
}
