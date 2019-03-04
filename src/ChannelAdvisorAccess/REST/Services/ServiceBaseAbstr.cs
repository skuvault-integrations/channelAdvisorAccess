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
		public const string baseApiUrl = "https://api.channeladvisor.com";

		private readonly RestApplication _application;
		private readonly string _developerKey;
		private readonly string _developerPassword;
		private readonly string[] _scope = new string[] { "orders", "inventory" };
		private string _accessToken;
		private string _refreshToken;

		protected HttpClient client { get; private set; }

		public string AccountId { get; private set; }
		public string AccountName { get; private set; }

		/// <summary>
		///	Standard authorization flow for REST service
		/// </summary>
		/// <param name="apiUrl"></param>
		/// <param name="credentials"></param>
		public RestServiceBaseAbstr( string accountName, RestApplication application, string accessToken, string refreshToken )
		{
			_application = application;
			_accessToken = accessToken;
			_refreshToken = refreshToken;

			AccountName = accountName;

			SetupHttpClient();
		}

		/// <summary>
		///  SOAP compatible authorization flow for REST service
		/// </summary>
		/// <param name="credentials"></param>
		/// <param name="name"></param>
		/// <param name="accountID"></param>
		/// <param name="apiUrl"></param>
		/// <param name="cache"></param>
		public RestServiceBaseAbstr( RestApplication application, string accountName, string accountID, string developerKey, string developerPassword, ObjectCache cache )
		{
			AccountId = accountID;
			AccountName = accountName;

			_application = application;
			_developerKey = developerKey;
			_developerPassword = developerPassword;

			SetupHttpClient();
		}

		/// <summary>
		///	
		/// </summary>
		/// <param name="apiUrl"></param>
		protected void SetupHttpClient()
		{
			client = new HttpClient();
			client.BaseAddress = new Uri( baseApiUrl );
			client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/json") );
			SetDefaultAuthorizationHeader( client );
		}

		/// <summary>
		///	Gets refresh token via developer console credentials
		/// </summary>
		/// <returns></returns>
		protected async Task RefreshAccessToken()
		{
			if ( IsSOAPCredentialsFlowUsed() )
				await RefreshAccessTokenBySOAPCredentials().ConfigureAwait( false );
			else
				await RefreshAccessTokenByRestCredentials().ConfigureAwait( false );
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
			requestData.Add( "client_id", _application.ApplicationID );
			requestData.Add( "grant_type", "soap" );
			requestData.Add( "scope", string.Join( " ", _scope ) );
			requestData.Add( "developer_key", _developerKey );
			requestData.Add( "password", _developerPassword );
			requestData.Add( "account_id", AccountId );

			var content = new FormUrlEncodedContent( requestData );

			try
			{
				HttpResponseMessage response = await client.PostAsync("oauth2/token", content).ConfigureAwait( false );
				var result = response.Content.ReadAsAsync< OAuthResponse >().Result;

				if (!string.IsNullOrEmpty(result.Error))
					throw new ChannelAdvisorException(result.Error);

				_accessToken = result.AccessToken;
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
		///	Gets refresh token by REST credentials
		/// </summary>
		/// <returns></returns>
		protected async Task RefreshAccessTokenByRestCredentials()
		{
			SetBasicAuthorizationHeader( client );

			var requestData = new Dictionary< string, string >();
			requestData.Add( "grant_type", "refresh_token" );
			requestData.Add( "refresh_token", _refreshToken );
			var content = new FormUrlEncodedContent( requestData );

			try
			{
				HttpResponseMessage response = await client.PostAsync( "oauth2/token", content ).ConfigureAwait( false );
				var result = response.Content.ReadAsAsync< OAuthResponse >().Result;

				if ( !string.IsNullOrEmpty( result.Error ) )
					throw new ChannelAdvisorException( result.Error );

				_accessToken = result.AccessToken;
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
			var response = await client.PostAsJsonAsync( apiUrl + "?access_token=" + _accessToken, data ).ConfigureAwait( false );

			if ( response.StatusCode == HttpStatusCode.Unauthorized )
			{
				// we should refresh access token and try again
				await RefreshAccessToken().ConfigureAwait( false );

				response = await client.PostAsJsonAsync( apiUrl + "?access_token=" + _accessToken, data );
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
			var response = await client.PutAsJsonAsync( apiUrl + "?access_token=" + _accessToken, data ).ConfigureAwait( false );

			if ( response.StatusCode == HttpStatusCode.Unauthorized )
			{
				// we should refresh access token and try again
				await RefreshAccessToken().ConfigureAwait( false );

				response = await client.PutAsJsonAsync( apiUrl + "?access_token=" + _accessToken, data );
			}

			return response.StatusCode;
		}

		/// <summary>
		///  Setup HTTP client for REST authorization flow
		/// </summary>
		/// <param name="client"></param>
		private void SetDefaultAuthorizationHeader( HttpClient client )
		{
			client.DefaultRequestHeaders.Remove("Authorization");
			client.DefaultRequestHeaders.Add( "Authorization", $"Bearer { _accessToken }");
		}

		/// <summary>
		///  Setup HTTP client for SOAP compatible authorization flow
		/// </summary>
		/// <param name="client"></param>
		private void SetBasicAuthorizationHeader( HttpClient client )
		{
			client.DefaultRequestHeaders.Remove( "Authorization" );
			string authHeader = $"Basic { Convert.ToBase64String( Encoding.UTF8.GetBytes( _application.ApplicationID + ":" + _application.SharedSecret ) ) }";
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
	
		private bool IsSOAPCredentialsFlowUsed()
		{
			return !string.IsNullOrEmpty( _developerKey );
		}
	}
}
