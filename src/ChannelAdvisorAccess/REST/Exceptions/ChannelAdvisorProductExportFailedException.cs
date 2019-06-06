using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Exceptions
{
	public class ChannelAdvisorProductExportFailedException : Exception
	{
		public ChannelAdvisorProductExportFailedException( string message ) : this( message, null ) { }
		public ChannelAdvisorProductExportFailedException( string message, Exception ex ) : base ( message, ex ) { }
	}
}
