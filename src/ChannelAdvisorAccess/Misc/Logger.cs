using System;
using System.Diagnostics;
using System.Reflection;
using Netco.Logging;

namespace ChannelAdvisorAccess.Misc
{
	internal class ChannelAdvisorLogger
	{
		static ChannelAdvisorLogger()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			var fvi = FileVersionInfo.GetVersionInfo( assembly.Location );
		}

		public static ILogger Log()
		{
			return NetcoLogger.GetLogger( "ChannelAdvisorLogger" );
		}

		public static void LogTraceException( Exception exception )
		{
			Log().Trace( exception, "[channelAdvisor] An exception occured." );
		}

		public static void LogTraceStarted( string info )
		{
			Log().Trace( "[channelAdvisor] Start call:{0}.", info );
		}

		public static void LogTraceEnded( string info )
		{
			Log().Trace( "[channelAdvisor] End call:{0}.", info );
		}

		public static void LogTrace( string info )
		{
			Log().Trace( "[channelAdvisor] Trace info:{0}.", info );
		}
	}
}