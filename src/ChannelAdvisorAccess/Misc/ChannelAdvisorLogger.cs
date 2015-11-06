using System;
using System.Diagnostics;
using System.Reflection;
using Netco.Logging;

namespace ChannelAdvisorAccess.Misc
{
	internal class ChannelAdvisorLogger
	{
		private static readonly string _versionInfo;

		static ChannelAdvisorLogger()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			_versionInfo = FileVersionInfo.GetVersionInfo( assembly.Location ).FileVersion;
		}

		public static ILogger Log()
		{
			return NetcoLogger.GetLogger( "ChannelAdvisorLogger" );
		}

		public static void LogTraceException( Exception exception )
		{
			Log().Trace( exception, "[channelAdvisor] An exception occured. ver:{0}", _versionInfo );
		}

		public static void LogTraceStarted( string info )
		{
			Log().Trace( "[channelAdvisor] Start call:{0}, ver:{1}.", info, _versionInfo );
		}

		public static void LogTraceEnd( string info )
		{
			Log().Trace( "[channelAdvisor] End call:{0}, ver:{1}.", info, _versionInfo );
		}

		public static void LogTrace( string info )
		{
			Log().Trace( "[channelAdvisor] Trace info:{0}, ver:{1}.", info, _versionInfo );
		}

		public static void LogTraceRetryStarted( string info )
		{
			Log().Trace( "[channelAdvisor] TraceRetryStarted info:{0}, ver:{1}.", info, _versionInfo );
		}

		public static void LogTraceRetryEnd( string info )
		{
			Log().Trace( "[channelAdvisor] TraceRetryEnd info:{0}, ver:{1}.", info, _versionInfo );
		}
	}
}