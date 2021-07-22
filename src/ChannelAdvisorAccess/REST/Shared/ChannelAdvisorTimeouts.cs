using CuttingEdge.Conditions;
using System.Collections.Generic;

namespace ChannelAdvisorAccess.REST.Shared
{
	public enum ChannelAdvisorOperationEnum
	{
		/// <summary>
		///	API: /v1/orders?$filter=CreatedDateUtc ge {0} and CreatedDateUtc
		/// </summary>
		ListOrdersRest,
		/// <summary>
		///	API: /v1/orders?$filter=ID eq {0}
		/// </summary>
		GetOrderRest,
		/// <summary>
		///  API: /v1/Products$count
		/// </summary>
		GetProductsCatalogSizeRest,
		/// <summary>
		///	API: /v1/Products$select=DCQuantities,Attributes,Images
		/// </summary>
		GetAllProductsRest,
		/// <summary>
		///	API: /v1/Products$select=ID,Sku
		/// </summary>
		GetAllProductsIdsRest,
		/// <summary>
		///	API: /v1/Products?$filter=sku eq {sku}
		/// </summary>
		GetProductBySkuUsingBatchRest,
		/// <summary>
		///	API: /v1/Products?$filter=sku eq {sku}&$select=ID,Sku
		/// </summary>
		GetProductIdBySkuRest,
		/// <summary>
		///	API: /v1/$batch
		///		with several /v1/Products?$filter=sku eq {sku}&$select=ID,Sku requests
		/// </summary>
		GetProductIdBySkuUsingBatchRest,
		/// <summary>
		///	API: /v1/Products(id)
		/// </summary>
		GetProductByIdRest,
		/// <summary>
		///	API: /v1/Products?$filter=sku eq {sku}&$select=sku,StoreTitle,StoreDescription,IsDisplayInStore
		/// </summary>
		GetProductBySkuWithStoreInfoRest,
		/// <summary>
		///  API: /v1/Products?$filter=sku eq {sku}&$select=sku,Attributes&$expand=Attributes
		/// </summary>
		GetProductBySkuWithAttributesOnlyRest,
		/// <summary>
		/// API: /v1/Products?$filter=sku eq {sku}&$select=sku,IsInRelationship,IsParent,RelationshipName&$expand=DCQuantities
		/// </summary>
		GetProductBySkuWithVariationInfoOnlyRest,
		/// <summary>
		/// API: /v1/Products?$filter=sku eq {sku}&$select=sku,TotalAvailableQuantity,DCQuantities&$expand=DCQuantities
		/// </summary>
		GetProductBySkuWithQuantitiesOnlyRest,
		/// <summary>
		///	API: /v1/$batch
		///		with /v1/Products?$filter=sku eq {sku}&$select=sku,TotalAvailableQuantity,DCQuantities&$expand=DCQuantities
		/// </summary>
		GetProductBySkuWithQuantitiesOnlyUsingBatchRest,
		/// <summary>
		///	API: /v1/Products?$filter=sku eq {sku}&$select=ID,Sku,Images&$expand=Images
		/// </summary>
		GetProductBySkuWithImagesOnlyRest,
		/// <summary>
		///	API: /v1/Products?$filter=Classification eq {classification}&$select=ID,Sku
		///		or
		///		/v1/Products?$filter=Labels/Any (l: l/Name eq '{label}')&$select=ID,Sku
		///		or
		///		/v1/Products?$filter=CreateDateUtc ge {createDateUtc} and CreateDateUtc le {createDateUtc}$select=ID,Sku
		///		or
		///		/v1/Products?$filter=UpdateDateUtc ge {createDateUtc} and UpdateDateUtc le {createDateUtc}$select=ID,Sku
		/// </summary>
		GetProductsByFilterWithIdOnlyRest,
		/// <summary>
		///	API: /v1/ProductExport
		/// </summary>
		ExportProductsRest,
		/// <summary>
		///	API: /v1/Products(Id}/UpdateQuantity
		/// </summary>
		UpdateProductQuantityRest,
		/// <summary>
		///	API: /v1/$batch
		///		with /v1/Products(Id}/UpdateQuantity
		/// </summary>
		UpdateProductQuantityUsingBatchRest,
		/// <summary>
		///	API: /v1/Products(id}
		/// </summary>
		UpdateProductFieldsRest,
		/// <summary>
		///	API: /v1/$batch
		///		with /v1/Products(id}
		/// </summary>
		UpdateProductFieldsUsingBatchRest,
		/// <summary>
		///	API: oauth2/token
		/// </summary>
		RefreshAccessTokenByRestCredentials,
		/// <summary>
		///	API: oauth2/token
		/// </summary>
		RefreshAccessTokenBySoapCredentials,
		/// <summary>
		///	API: /v1/DistributionCenters
		/// </summary>
		GetDistributionCentersRest
	}

	public class ChannelAdvisorOperationTimeout
	{
		public int TimeoutInMs { get; private set; }

		public ChannelAdvisorOperationTimeout( int timeoutInMs )
		{
			Condition.Requires( timeoutInMs, "timeoutInMs" ).IsGreaterThan( 0 );
			this.TimeoutInMs = timeoutInMs;
		}
	}

	public class ChannelAdvisorTimeouts
	{
		public const int DefaultTimeoutInMs = 10 * 60 * 1000;
		private Dictionary< ChannelAdvisorOperationEnum, ChannelAdvisorOperationTimeout > _timeouts;

		/// <summary>
		///	This timeout value will be used if specific timeout for operation is not provided. Default value can be changed through constructor.
		/// </summary>
		public ChannelAdvisorOperationTimeout DefaultOperationTimeout { get; private set; }

		public int this[ ChannelAdvisorOperationEnum operation ]
		{
			get
			{
				ChannelAdvisorOperationTimeout timeout;
				if ( _timeouts.TryGetValue( operation, out timeout ) )
					return timeout.TimeoutInMs;

				return DefaultOperationTimeout.TimeoutInMs;
			}
		}

		public void Set( ChannelAdvisorOperationEnum operation, ChannelAdvisorOperationTimeout timeout )
		{
			if ( _timeouts.ContainsKey( operation ) )
			{
				_timeouts.Remove( operation );
			}

			_timeouts.Add( operation, timeout );
		}

		public ChannelAdvisorTimeouts( int defaultTimeoutInMs )
		{
			_timeouts = new Dictionary< ChannelAdvisorOperationEnum, ChannelAdvisorOperationTimeout >();
			this.DefaultOperationTimeout = new ChannelAdvisorOperationTimeout( defaultTimeoutInMs );
		}

		public ChannelAdvisorTimeouts() : this( DefaultTimeoutInMs ) { }
	}
}