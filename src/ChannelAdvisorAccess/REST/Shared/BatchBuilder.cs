using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ChannelAdvisorAccess.REST.Shared
{
	public class BatchBuilder
	{
		private List< RequestPart > _requestParts;
		private string _baseUrl;

		public BatchBuilder( string baseUrl )
		{
			if( string.IsNullOrWhiteSpace( baseUrl ) )
			{
				throw new ArgumentException( "baseUrl must not be null or whitespace", nameof(baseUrl) );
			}

			_requestParts = new List< RequestPart >();
			_baseUrl = baseUrl;
		}

		private BatchBuilder( string baseUrl, IEnumerable< RequestPart > parts )
		{
			if( string.IsNullOrWhiteSpace( baseUrl ) )
			{
				throw new ArgumentException( "baseUrl must not be null or whitespace", nameof(baseUrl) );
			}

			if( parts == null || !parts.Any() )
			{
				throw new ArgumentException( "parts must not be empty", nameof(parts) );
			}

			_requestParts = parts.ToList();
			_baseUrl = baseUrl;
		}


		public void AddGetRequest( string url)
		{
			_requestParts.Add( new RequestPart( _requestParts.Count + 1, HttpMethod.Get, _baseUrl + url ) );
		}

		public void AddGetRequests( IEnumerable< string > urls )
		{
			foreach( string url in urls )
			{
				this.AddGetRequest( url );
			}
		}

		public void AddPostRequest( string url, string payload )
		{
			_requestParts.Add( new RequestPart( _requestParts.Count + 1, HttpMethod.Post, _baseUrl + url, payload ) );
		}

		public void AddPutRequest( string url, string payload )
		{
			_requestParts.Add( new RequestPart( _requestParts.Count + 1, HttpMethod.Put, _baseUrl + url, payload ) );
		}
		
		public BatchBuilder[] Split( int batchSize )
		{
			var batches = new List< BatchBuilder >();

			if ( this._requestParts.Count == batchSize )
			{
				batches.Add( this );
				return batches.ToArray();
			}

			var chunks = this.ToChunks( this._requestParts, batchSize );

			foreach( var chunk in chunks )
			{
				batches.Add( new BatchBuilder( this._baseUrl, chunk ) );
			}

			return batches.ToArray();
		}

		public MultipartContent Build()
		{
			var multiPartContent = new MultipartContent( "mixed", "changeset" );

			foreach( var part in this._requestParts )
			{
				var requestContent = string.Format( "\r\n{0} {1} HTTP/1.1", part.Method.ToString().ToUpper(), part.Url );

				if ( part.Method == HttpMethod.Post || part.Method == HttpMethod.Put )
				{
					requestContent += "\r\nContent-Type: application/json \r\n\r\n";
					requestContent += part.Payload;
				}
				else
					requestContent += "\r\n";

				var content = new StreamContent( new MemoryStream( Encoding.UTF8.GetBytes( requestContent ) ) );
				content.Headers.Add( "Content-Type", part.ContentType );
				content.Headers.Add( "Content-Transfer-Encoding", part.ContentTransferEncoding );
				content.Headers.Add( "Content-ID", part.Id.ToString() );
				multiPartContent.Add( content );
			}

			return multiPartContent;
		}

		public override string ToString()
		{
			var bodyBuilder = new StringBuilder();
			bodyBuilder.AppendLine( "--batch" );
			bodyBuilder.AppendLine( "Content-Type: multipart/mixed; boundary=changeset" );

			foreach( var part in _requestParts )
			{
				bodyBuilder.AppendLine( part.ToString() );
			}

			bodyBuilder.AppendLine( "--changeset--" );
			bodyBuilder.AppendLine( "--batch--" );

			return bodyBuilder.ToString();
		}

		private IEnumerable< List< T > > ToChunks< T >( IEnumerable< T > items, int chunkSize )
		{
			var chunk = new List< T >( chunkSize );
			foreach( var item in items )
			{
				chunk.Add( item );
				if( chunk.Count == chunkSize )
				{
					yield return chunk;
					chunk = new List< T >( chunkSize );
				}
			}
			if( chunk.Any() )
				yield return chunk;
		}
	}

	class RequestPart
	{
		public int Id { get; private set; }
		public string Boundary { get; private set; }
		public string ContentType { get; private set; }
		public string ContentTransferEncoding { get; private set; }
		public string Url { get; private set; }
		public HttpMethod Method { get; private set; }
		public string Payload { get; private set; }

		public RequestPart( int id, HttpMethod method, string url, string payload = null )
		{
			if( id <= 0 )
			{
				throw new ArgumentOutOfRangeException( nameof(id), id, "Id must be greater than 0" );
			}

			this.Id = id;
			this.Boundary = "--changeset";
			this.ContentType = "application/http";
			this.ContentTransferEncoding = "binary";
			this.Url = url;
			this.Method = method;
			this.Payload = payload;
		}

		public override string ToString()
		{
			var requestPart = new StringBuilder();
			requestPart.AppendLine( this.Boundary );
			requestPart.AppendLine( string.Format( "Content-Type: {0}", this.ContentType ) );
			requestPart.AppendLine( string.Format( "Content-Transfer-Encoding: {0}", this.ContentTransferEncoding ) );
			requestPart.AppendLine( string.Format( "Content-ID: {0}", this.Id ) );
			requestPart.AppendLine();
			requestPart.AppendLine( string.Format( "{0} {1} HTTP/1.1", this.Method, this.Url ) );

			if ( this.Method == HttpMethod.Post || this.Method == HttpMethod.Put )
			{
				requestPart.AppendLine( "Content-Type: application/json" );
				requestPart.AppendLine();
				requestPart.AppendLine( this.Payload );
			}
			else
				requestPart.AppendLine();

			return requestPart.ToString();
		}
	}
}
