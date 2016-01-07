using System;
using System.Runtime.CompilerServices;
using ChannelAdvisorAccess.Misc;
using Newtonsoft.Json;

namespace ChannelAdvisorAccess.Services.Items
{
	public abstract class ServiceBaseAbstr
	{
		protected string CreateMethodCallInfo( string methodParameters = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", string notes = "", [ CallerMemberName ] string memberName = "" )
		{
			try
			{
				mark = mark ?? Mark.Blank();
				var connectionInfo = this.ToJson();
				var str = string.Format(
					"{{Mark:\"{3}\", MethodName:{0}, ConnectionInfo:{1}, MethodParameters:{2} {4}{5}{6}{7}}}",
					memberName,
					connectionInfo,
					string.IsNullOrWhiteSpace( methodParameters ) ? PredefinedValues.EmptyJsonObject : methodParameters,
					mark,
					string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors,
					string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult,
					string.IsNullOrWhiteSpace( notes ) ? string.Empty : ",Notes: " + notes,
					string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", " + additionalInfo
					);
				return str;
			}
			catch( Exception exception )
			{
				return PredefinedValues.EmptyJsonObject;
			}
		}

		[ JsonIgnore ]
		public Func< string > AdditionalLogInfo{ get; set; }

		protected string AdditionalLogInfoString
		{
			get
			{
				if( this.AdditionalLogInfo == null )
					return string.Empty;

				string res;
				try
				{
					res = this.AdditionalLogInfo();
				}
				catch
				{
					return string.Empty;
				}

				return res;
			}
		}
	}
}