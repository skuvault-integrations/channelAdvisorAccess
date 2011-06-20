using System;

namespace ChannelAdvisorAccess.Exceptions
{
	/// <summary>
	/// Exception thrown if ChannelAdvisor webservice returns an error.
	/// </summary>
	public class ChannelAdvisorException : Exception
	{
		private readonly int _messageCode;
		private readonly string _errorMessage;

		public ChannelAdvisorException( int code, string message )
		{
			this._messageCode = code;
			this._errorMessage = message;
		}

		public int MessageCode
		{
			get { return this._messageCode; }
		}

		public string ErrorMessage
		{
			get { return this._errorMessage; }
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override string ToString()
		{
			return string.Format( "ErrorCode={0}, ErrorMessage={1}\n" +  base.ToString(), this.MessageCode, this.ErrorMessage );
		}
	}
}