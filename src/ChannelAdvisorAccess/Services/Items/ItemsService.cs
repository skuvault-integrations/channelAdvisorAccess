using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Extensions;
using ChannelAdvisorAccess.InventoryService;
using ChannelAdvisorAccess.Misc;
using CuttingEdge.Conditions;
using Netco.Extensions;
using Netco.Logging;
using Newtonsoft.Json;

namespace ChannelAdvisorAccess.Services.Items
{
	public partial class ItemsService: IItemsService
	{
		private readonly APICredentials _credentials;
		private readonly InventoryServiceSoapClient _client;

		private readonly ObjectCache _cache;
		private readonly string _allItemsCacheKey;
		private readonly object _inventoryCacheLock = new Object();

		[ JsonIgnore ]
		public Func< string > AdditionalLogInfo{ get; set; }

		private string AdditionalLogInfoString
		{
			get
			{
				if( this.AdditionalLogInfo == null )
					return null;

				string res;
				try
				{
					res = this.AdditionalLogInfo();
				}
				catch
				{
					return null;
				}

				return res;
			}
		}
		public string Name{ get; private set; }
		public string AccountId{ get; private set; }
		public LogDetailsEnum LogDetailsEnum{ get; set; }
		public TimeSpan SlidingCacheExpiration{ get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ItemsService"/> class.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		/// <param name="name">The account user-friendly name.</param>
		/// <param name="accountId">The account id.</param>
		/// <param name="cache">The cache.</param>
		/// <remarks>If <paramref name="cache"/> is <c>null</c> no caching takes place.</remarks>
		public ItemsService( APICredentials credentials, string name, string accountId, ObjectCache cache = null, LogDetailsEnum logDetailsEnum = LogDetailsEnum.Undefined )
		{
			this._credentials = credentials;
			this.AccountId = accountId;
			this.LogDetailsEnum = logDetailsEnum;
			this._client = new InventoryServiceSoapClient();

			this.Name = name;
			this._cache = cache;
			this.SlidingCacheExpiration = ObjectCache.NoSlidingExpiration;
			this._allItemsCacheKey = string.Format( "caAllItems_ID_{0}", this.AccountId );
		}

		
		#region Check for Success
		/// <summary>
		/// Gets the result with success check.
		/// </summary>
		/// <typeparam name="T">Type of the result.</typeparam>
		/// <param name="apiResult">The API result.</param>
		/// <param name="resultData">The result data.</param>
		/// <returns>Returns result default value (typically <c>null</c>) if there was a problem
		/// with API call, otherwise returns result.</returns>
		private T GetResultWithSuccessCheck< T >( object apiResult, T resultData )
		{
			if (!this.IsRequestSuccessful(apiResult))
				return default( T );

			return resultData;
		}
		
		private bool IsRequestSuccessful(object obj)
		{
			var type = obj.GetType();

			var statusProp = type.GetProperty("Status");
			var status = (ResultStatus)statusProp.GetValue(obj, null);

			var messageCodeProp = type.GetProperty("MessageCode");
			var messageCode = (int)messageCodeProp.GetValue(obj, null);

			var message = (string)type.GetProperty("Message").GetValue(obj, null);

			return IsRequestSuccessfulCommon(status, message, messageCode);
		}

		private bool IsRequestSuccessfulImage(object obj)
		{
			GetInventoryItemImageListResponse apiResult = (GetInventoryItemImageListResponse)obj;

			var status = apiResult.GetInventoryItemImageListResult.Status;
			var message = apiResult.GetInventoryItemImageListResult.Message;
			var messageCode = apiResult.GetInventoryItemImageListResult.MessageCode;

			return IsRequestSuccessfulCommon(status, message, messageCode);
		}

		private bool IsRequestSuccessfulAttribute(object obj)
		{
			GetInventoryItemAttributeListResponse apiResult = (GetInventoryItemAttributeListResponse)obj;

			var status = apiResult.GetInventoryItemAttributeListResult.Status;
			var message = apiResult.GetInventoryItemAttributeListResult.Message;
			var messageCode = apiResult.GetInventoryItemAttributeListResult.MessageCode;

			return IsRequestSuccessfulCommon(status, message, messageCode);
		}

		/// <summary>
		/// Determines whether request was successful or not.
		/// </summary>
		/// <param name="apiResult">The API result.</param>
		/// <returns>
		/// 	<c>true</c> if request was successful; otherwise, <c>false</c>.
		/// </returns>
		private bool IsRequestSuccessfulCommon(ResultStatus status, string message, int messageCode)
		{
			var isRequestSuccessful = status == ResultStatus.Success && messageCode == 0;

			if (!isRequestSuccessful)
			{
				if (message.Contains("The specified SKU was not found") || message.Contains("All DoesSkuExist requests failed for the SKU list specified!"))
					this.Log().Trace("CA Api Request for '{0}' failed with message: {1}", AccountId, message);
				else
					this.Log().Error("CA Api Request for '{0}' failed with message: {1}", AccountId, message);
			}

			return isRequestSuccessful;
		}

		private static void CheckCaSuccess( APIResultOfBoolean apiResult )
		{
			if( apiResult.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( apiResult.MessageCode, apiResult.Message );
		}

		private static void CheckCaSuccess( APIResultOfArrayOfSynchInventoryItemResponse apiResult )
		{
			if( apiResult.Status != ResultStatus.Success )
			{
				// no sku exists, ignore
				if( apiResult.Message == "All Update requests failed for the SKU list specified!" )
					return;

				var msgs = new StringBuilder();

				foreach( var result in apiResult.ResultData )
				{
					if( !result.Result && !IsSkuMissing( result ) )
						msgs.AppendLine( result.ErrorMessage );
				}

				if( msgs.Length > 0 )
					throw new ChannelAdvisorException( apiResult.MessageCode, string.Concat( apiResult.Message, Environment.NewLine, msgs.ToString() ) );
			}
		}

		private static bool IsSkuMissing( SynchInventoryItemResponse r )
		{
			const string skuMissingMsg = "The specified Sku does not exist";
			return r.ErrorMessage.StartsWith( skuMissingMsg, StringComparison.InvariantCultureIgnoreCase );
		}

		private static void CheckCaSuccess( APIResultOfArrayOfUpdateInventoryItemResponse apiResult )
		{
			if( apiResult.Status != ResultStatus.Success )
			{
				// no sku exists, ignore
				if( apiResult.Message == "All Update requests failed for the SKU list specified!" )
					return;

				if( apiResult.ResultData == null || apiResult.ResultData.Length == 0 )
					throw new ChannelAdvisorException( apiResult.MessageCode, apiResult.Message );

				var msgs = new StringBuilder();

				foreach( var result in apiResult.ResultData )
				{
					if( !result.Result && !IsSkuMissing( result ) )
						msgs.AppendLine( result.ErrorMessage );
				}

				if( msgs.Length > 0 )
					throw new ChannelAdvisorException( apiResult.MessageCode, string.Concat( apiResult.Message, Environment.NewLine, msgs.ToString() ) );
			}
		}

		private static bool IsSkuMissing( UpdateInventoryItemResponse result )
		{
			const string skuMissingMsg = "The specified Sku does not exist";
			return result.ErrorMessage.StartsWith( skuMissingMsg, StringComparison.InvariantCultureIgnoreCase );
		}

		private static void CheckCaSuccess( APIResultOfInt32 result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		private static void CheckCaSuccess( APIResultOfArrayOfClassificationConfigurationInformation result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		private void CheckCaSuccess( APIResultOfString result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		private void CheckCaSuccess( APIResultOfArrayOfDistributionCenterResponse result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}
		#endregion

		private string CreateMethodCallInfo( string methodParameters = "", Mark mark = null, string errors = "", string methodResult = "", string additionalInfo = "", string notes = "", [ CallerMemberName ] string memberName = "" )
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
	}
}