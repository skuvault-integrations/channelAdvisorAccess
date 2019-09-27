using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace ChannelAdvisorAccess.REST.Shared
{
	public class MultiPartResponseParser
	{
		private const string PartDelimiter = "batchresponse";

		public IEnumerable< T > Parse< T >( string response, out int httpBatchStatusCode )
		{
			httpBatchStatusCode = (int)HttpStatusCode.OK;
			var result = new List< T >();

			string[] lines = response.Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );
			string partResponseRaw = null;
			var startPick = false;

			for( int i = 0; i < lines.Length; i++ )
			{
				var line = lines[ i ];
				if ( line.Contains( PartDelimiter ) )
				{
					if ( partResponseRaw != null )
					{
						try
						{
							result.Add( JsonConvert.DeserializeObject< T >( partResponseRaw ) );
						}
						catch { }
					}

					partResponseRaw = null;
					startPick = false;
				}
				else if ( line.Contains( "HTTP/1.1" ) )
				{
					var temp = line.Replace( "HTTP/1.1", "" ).Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

					if ( temp.Length > 0 )
					{
						int httpRequestStatusCode = 0;
						if ( int.TryParse( temp[0], out httpRequestStatusCode ) )
						{
							if ( httpRequestStatusCode >= 300 )
								httpBatchStatusCode = httpRequestStatusCode;
						}
					}
				}
				else if ( line.Contains( "Data-Version" ) )
				{
					startPick = true;
					continue;
				}

				if ( startPick )
					partResponseRaw += line;
			}

			return result;
		}
	}
}
