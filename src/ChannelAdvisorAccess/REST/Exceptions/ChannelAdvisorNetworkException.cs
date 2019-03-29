using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Exceptions
{
	public class ChannelAdvisorNetworkException : Exception
	{
		public ChannelAdvisorNetworkException( string message, Exception exception ) : base ( message, exception ) { }
	}

	public class ChannelAdvisorUnauthorizedException : ChannelAdvisorNetworkException
	{
		public ChannelAdvisorUnauthorizedException( string message ) : base ( message, null ) { }
	}
}
