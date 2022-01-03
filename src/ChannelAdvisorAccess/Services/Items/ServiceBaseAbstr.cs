using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using ChannelAdvisorAccess.Misc;
using Newtonsoft.Json;

namespace ChannelAdvisorAccess.Services.Items
{
	public abstract class ServiceBaseAbstr
	{
		protected ServiceBaseAbstr()
		{
			this.LastNetworkActivityTime = DateTime.UtcNow;
			InitSecurityProtocol();
		}

		protected DateTime LastNetworkActivityTime { get; set; }

		protected string CreateMethodCallInfo( string methodParameters = "", string payload = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", string notes = "", [ CallerMemberName ] string memberName = "", string returnStatusCode = "", int? operationTimeout = null )
		{
			try
			{
				mark = mark ?? Mark.Blank();
				var connectionInfo = this.ToJson();
				var str = string.Format(
					"{{Mark:\"{3}\", MethodName:{0}, ConnectionInfo:{1}, MethodParameters: {2} {8}{4}{5}{6}{7}{9}{10}}}",
					memberName,
					connectionInfo,
					string.IsNullOrWhiteSpace( methodParameters ) ? PredefinedValues.EmptyJsonObject : methodParameters,
					mark,
					string.IsNullOrWhiteSpace( errors ) ? string.Empty : ", Errors:" + errors,
					string.IsNullOrWhiteSpace( methodResult ) ? string.Empty : ", Result:" + methodResult,
					string.IsNullOrWhiteSpace( notes ) ? string.Empty : ",Notes: " + notes,
					string.IsNullOrWhiteSpace( additionalInfo ) ? string.Empty : ", AdditionalInfo: " + additionalInfo,
					string.IsNullOrWhiteSpace( payload ) ? string.Empty : ", Body: " + payload,
					string.IsNullOrWhiteSpace( returnStatusCode ) ? string.Empty : ", ReturnStatus: \"" + returnStatusCode + "\"",
					operationTimeout == null ? string.Empty : ", Timeout: " + operationTimeout + " ms"
					);
				return str;
			}
			catch( Exception )
			{
				return PredefinedValues.EmptyJsonObject;
			}
		}
		
		/// <summary>
		///	This method is used to update service's last network activity time.
		///	It's called every time before making API request to server or after handling the response.
		/// </summary>
		protected void RefreshLastNetworkActivityTime()
		{
			this.LastNetworkActivityTime = DateTime.UtcNow;
		}

		private Func< string > _additionalLogInfo;

		[ JsonIgnore ]
		public Func< string > AdditionalLogInfo
		{
			get { return this._additionalLogInfo ?? ( () => string.Empty ); }
			set { this._additionalLogInfo = value; }
		}

		private void InitSecurityProtocol()
		{
			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
		}

		#region IDisposable

		internal bool disposedValue = false;

		public bool Disposed
		{
			get
			{
				if ( !this.disposedValue )
				{
					return this.disposedValue;
				}

				throw new ObjectDisposedException( "CanBeDisposed" );
			}
		}

		internal void Dispose< T >( ClientBase< T > client, bool disposing ) where T : class
		{
			if ( !disposedValue )
			{
				if ( disposing )
				{
					client.Close();
				}

				disposedValue = true;
			}
		}

        #endregion
    }
}