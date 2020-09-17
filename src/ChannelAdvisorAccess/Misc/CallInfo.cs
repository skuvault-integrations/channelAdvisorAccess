using System;
using System.Runtime.CompilerServices;

namespace ChannelAdvisorAccess.Misc
{
	public class CallInfoBasic
	{
		public Mark Mark { get; }
		public string MemberName { get; }
		public string ConnectionInfo { get; }
		public string AdditionalInfo { get; }
		public string MethodParameters { get; }

		public CallInfoBasic( string connectionInfo, string additionalInfo, string methodParameters = "", Mark mark = null, [ CallerMemberName ] string memberName = "" )
		{
			this.Mark = mark ?? Mark.Blank();
			this.MemberName = memberName;
			this.ConnectionInfo = connectionInfo;
			this.AdditionalInfo = additionalInfo;
			this.MethodParameters = methodParameters;
		}
	}

	public class CallInfo : CallInfoBasic
	{
		public string Payload { get; }
		public string MethodResult { get; }
		public string Notes { get; }

		public CallInfo( string methodParameters = "", string payload = "", Mark mark = null, string methodResult = "", string additionalInfo = "", string notes = "", [ CallerMemberName ] string memberName = "", string connectionInfo = "" )
			: base( connectionInfo, additionalInfo, methodParameters, mark, memberName )
		{
			
			Payload = payload;
			MethodResult = methodResult;
			Notes = notes;
		}

		public string GetPayloadResponseText()
		{
			return $"Payload: \"{Payload}\", Response: \"{MethodResult}\"";
		}
	}

	public class RetryInfo : CallInfoBasic
	{
		public int RetryAttempt { get; }
		public double WaitDurationSecs { get; }

		public RetryInfo( int retryAttempt, double waitDurationSecs, [ CallerMemberName ] string memberName = "", string methodParameters = "" , string connectionInfo = "", string additionalInfo = "", Mark mark = null )
			: base( connectionInfo, additionalInfo, methodParameters, mark, memberName )
		{
			RetryAttempt = retryAttempt;
			WaitDurationSecs = waitDurationSecs;
		}
	}
}
