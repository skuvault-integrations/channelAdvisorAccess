using ChannelAdvisorAccess.OrderService;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.REST.Misc;
using ChannelAdvisorAccess.Services.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ChannelAdvisorAccess.REST.Services
{
	public abstract class RestServiceBaseAbstr : ServiceBaseAbstr
	{
		private readonly RestAPICredentials _credentials;
		private readonly APICredentials _SOAPCredentials;

		protected HttpClient client { get; private set; }

		public string AccountId { get; private set; }

		public RestServiceBaseAbstr( string apiUrl, RestAPICredentials credentials )
		{
			_credentials = credentials;

			SetupHttpClient(apiUrl);
		}

		public RestServiceBaseAbstr( APICredentials credentials, string accountID, string apiUrl, ObjectCache cache )
		{
			_SOAPCredentials = credentials;
			AccountId = accountID;

			SetupHttpClient( apiUrl );
		}

		protected void SetupHttpClient(string apiUrl)
		{
			client = new HttpClient();
			client.BaseAddress = new Uri( apiUrl );
			client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/json") );
			SetDefaultAuthorizationHeader( client );
		}

		/// <summary>
		///	Gets refresh token via developer console credentials
		/// </summary>
		/// <returns></returns>
		protected async Task RefreshAccessToken()
		{
			SetBasicAuthorizationHeader( client );

			var requestData = new Dictionary< string, string >();
			requestData.Add( "grant_type", "refresh_token" );
			requestData.Add( "refresh_token", _credentials.RefreshToken );
			var content = new FormUrlEncodedContent( requestData );

			try
			{
				HttpResponseMessage response = await client.PostAsync( "oauth2/token", content ).ConfigureAwait( false );
				var result = response.Content.ReadAsAsync< OAuthResponse >().Result;

				if ( !string.IsNullOrEmpty( result.Error ) )
					throw new ChannelAdvisorException( result.Error );

				_credentials.AccessToken = result.AccessToken;
			}
			catch( Exception ex )
			{
				var channelAdvisorException = new ChannelAdvisorException( ex.Message, ex );
				throw channelAdvisorException;
			}
			finally
			{
				SetDefaultAuthorizationHeader(client);
			}
		}

		/// <summary>
		///	Gets refresh token by SOAP credentials
		///	This is way how to obtain refresh token using existing credentials without involving partner
		/// </summary>
		/// <returns></returns>
		protected async Task RefreshAccessTokenBySOAPCredentials()
		{
			SetBasicAuthorizationHeader( client );
			var requestData = new Dictionary< string, string >();
			requestData.Add( "client_id", _credentials.ApplicationID );
			requestData.Add( "grant_type", "soap" );
			requestData.Add( "scope", "orders inventory" );
			requestData.Add( "developer_key", "" );

			var content = new FormUrlEncodedContent(requestData);

			try
			{
				HttpResponseMessage response = await client.PostAsync("oauth2/token", content).ConfigureAwait( false );
				var result = response.Content.ReadAsAsync< OAuthResponse >().Result;

				if (!string.IsNullOrEmpty(result.Error))
					throw new ChannelAdvisorException(result.Error);

				_credentials.AccessToken = result.AccessToken;
			}
			catch(Exception ex)
			{
				var channelAdvisorException = new ChannelAdvisorException(ex.Message, ex);
				throw channelAdvisorException;
			}
			finally
			{
				SetDefaultAuthorizationHeader(client);
			}

		}

		/// <summary>
		///	Gets response from REST Endpoint and tries to refresh token if necessary
		/// </summary>
		/// <param name="apiUrl"></param>
		/// <returns></returns>
		protected async Task< HttpResponseMessage > GetResponseAsync( string apiUrl )
		{
			var response = await client.GetAsync( apiUrl ).ConfigureAwait( false );

			if ( response.StatusCode == HttpStatusCode.Unauthorized )
			{
				// we should refresh access token and try again
				await RefreshAccessToken().ConfigureAwait( false );

				response = await client.GetAsync( apiUrl ).ConfigureAwait( false );
			}

			return response;
		}

		/// <summary>
		///	Post data to REST Endpoint
		/// </summary>
		/// <param name="apiUrl"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		protected async Task< HttpStatusCode > PostAsync< T >( string apiUrl, T data )
		{
			var response = await client.PostAsJsonAsync( apiUrl + "?access_token=" + _credentials.AccessToken, data ).ConfigureAwait( false );

			if ( response.StatusCode == HttpStatusCode.Unauthorized )
			{
				// we should refresh access token and try again
				await RefreshAccessToken().ConfigureAwait( false );

				response = await client.PostAsJsonAsync( apiUrl + "?access_token=" + _credentials.AccessToken, data );
			}

			return response.StatusCode;
		}

		/// <summary>
		///	Patch object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="apiUrl"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		protected async Task < HttpStatusCode > PutAsync< T > ( string apiUrl, T data )
		{
			var response = await client.PutAsJsonAsync( apiUrl + "?access_token=" + _credentials.AccessToken, data ).ConfigureAwait( false );

			if ( response.StatusCode == HttpStatusCode.Unauthorized )
			{
				// we should refresh access token and try again
				await RefreshAccessToken().ConfigureAwait( false );

				response = await client.PutAsJsonAsync( apiUrl + "?access_token=" + _credentials.AccessToken, data );
			}

			return response.StatusCode;
		}

		private void SetDefaultAuthorizationHeader( HttpClient client )
		{
			client.DefaultRequestHeaders.Remove("Authorization");
			client.DefaultRequestHeaders.Add( "Authorization", $"Bearer { _credentials.AccessToken }");
		}

		private void SetBasicAuthorizationHeader( HttpClient client )
		{
			client.DefaultRequestHeaders.Remove( "Authorization" );
			string authHeader = $"Basic { Convert.ToBase64String( Encoding.UTF8.GetBytes( _credentials.ApplicationID + ":" + _credentials.SharedSecret ) ) }";
			client.DefaultRequestHeaders.Add( "Authorization", authHeader );
		}
	
		/// <summary>
		///	Convert date in format suitable for REST end point
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		protected string ConvertDate( DateTime date )
		{
			return date.ToString( "yyyy-MM-ddThh:mm:ssZ", CultureInfo.InvariantCulture );
		}
	}
}
