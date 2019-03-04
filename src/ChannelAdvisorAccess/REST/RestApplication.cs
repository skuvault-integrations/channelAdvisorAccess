using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.Misc
{
	public class RestApplication
	{
		public string ApplicationID { get; private set; }
		public string SharedSecret { get; private set; }

		public RestApplication( string applicationID, string sharedSecret )
		{
			ApplicationID = applicationID;
			SharedSecret = sharedSecret;
		}
	}
}
