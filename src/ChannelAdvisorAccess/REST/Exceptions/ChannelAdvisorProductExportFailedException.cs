using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Exceptions
{
	public class ChannelAdvisorProductExportUnavailableException : Exception
	{
		public ChannelAdvisorProductExportUnavailableException( string message ) : this( message, null ) { }
		public ChannelAdvisorProductExportUnavailableException( string message, Exception ex ) : base ( message, ex ) { }
	}
}
