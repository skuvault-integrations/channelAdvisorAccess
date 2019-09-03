using ChannelAdvisorAccess.Constants;
using ChannelAdvisorAccess.REST.Models.Configuration;
using ChannelAdvisorAccess.Services.Items;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ChannelAdvisorAccess.REST.Shared
{
	public class ItemsServiceUrlBuilder
	{
		public string GetProductWithIdOnlyBySkuUrl( string sku )
		{
			return this.GetProductBySkuUrl( sku, "ID,sku", null );
		}

		public string GetProductWithStoreInfoOnlyBySkuUrl( string sku )
		{
			return this.GetProductBySkuUrl( sku, "sku,StoreTitle,StoreDescription,IsDisplayInStore", null );
		}

		public string GetProductWithAttributesOnlyBySkuUrl( string sku )
		{
			return this.GetProductBySkuUrl( sku, "sku,Attributes", "Attributes" );
		}

		public string GetProductWithQuantityOnlyBySkuUrl( string sku )
		{
			return this.GetProductBySkuUrl( sku, "sku,TotalAvailableQuantity,DCQuantities", "DCQuantities" );
		}

		public string GetProductWithVariationInfoOnlyBySkuUrl( string sku )
		{
			return this.GetProductBySkuUrl( sku, "sku,IsInRelationship,IsParent,RelationshipName", "DCQuantities" );
		}

		public string GetProductBySkuUrl( string sku, string selectProperties, string expandProperties )
		{
			// following OData 4.0 specification single quote char should be represented as two single quotes
			var filter = string.Format( "sku eq '{0}'", Uri.EscapeDataString( sku.Replace( "'", "''" ) ) );

			return GetUrl( ChannelAdvisorEndPoint.ProductsUrl, filter, selectProperties, expandProperties );
		}

		public string GetProductsUrl( ItemsFilter itemsFilter, string selectProperties, string expandProperties )
		{
			var filterProperties = this.GetFilter( itemsFilter );
			return GetUrl( ChannelAdvisorEndPoint.ProductsUrl, filterProperties, selectProperties, expandProperties );
		}

		public string GetProductsUrl( string filterProperties, string selectProperties, string expandProperties )
		{
			return GetUrl( ChannelAdvisorEndPoint.ProductsUrl, filterProperties, selectProperties, expandProperties );
		}
		
		public string GetUpdateProductQuantityUrl( int productId )
		{
			return string.Format( "{0}({1})/UpdateQuantity", ChannelAdvisorEndPoint.ProductsUrl, productId );
		}

		public string GetUpdateProductUrl( int productId )
		{
			return string.Format( "{0}({1})", ChannelAdvisorEndPoint.ProductsUrl, productId );
		}

		/// <summary>
		///	Build url for specified endpoint
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="filterProperties"></param>
		/// <param name="selectProperties"></param>
		/// <param name="expandProperties"></param>
		/// <returns></returns>
		private string GetUrl( string baseUrl, string filterProperties, string selectProperties, string expandProperties )
		{
			var requestParams = new List< string >();

			// filter products by sku
			if ( !string.IsNullOrEmpty( filterProperties ) )
			{
				requestParams.Add( string.Format( "$filter={0}", filterProperties ) );
			}

			// expand product properties
			if ( !string.IsNullOrEmpty( expandProperties ) )
			{
				requestParams.Add( string.Format( "$expand={0}", expandProperties ) );
			}

			// select only specified properties
			if ( !string.IsNullOrEmpty( selectProperties ) )
			{
				requestParams.Add( string.Format( "$select={0}", selectProperties ) );
			}

			return baseUrl + "?" + string.Join( "&", requestParams.ToArray() );
		}

		/// <summary>
		///	Gets filter value for REST end point
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		private string GetFilter( ItemsFilter filter )
		{
			var filterClauses = new List< string >();

			if ( filter.Criteria != null )
			{
				var criteria = filter.Criteria;

				if ( !string.IsNullOrEmpty( criteria.ClassificationName ) )
					filterClauses.Add( String.Format( "Classification eq '{0}'", criteria.ClassificationName ) );

				if ( !string.IsNullOrEmpty( criteria.LabelName ))
					filterClauses.Add( String.Format( "Labels/Any (l: l/Name eq '{0}')", criteria.LabelName ) );

				if ( !string.IsNullOrEmpty( criteria.DateRangeField ))
				{
					string filterFieldName = null;

					if ( criteria.DateRangeField.Equals( TimeStampFields.CreateDate ) )
						filterFieldName = "CreateDateUtc";
					else if ( criteria.DateRangeField.Equals( TimeStampFields.LastUpdateDate ))
						filterFieldName = "UpdateDateUtc";

					if ( filterFieldName != null )
					{
						if ( criteria.DateRangeStartGMT.HasValue )
							filterClauses.Add( String.Format( "{0} ge {1} ", filterFieldName, this.ConvertDate( criteria.DateRangeStartGMT.Value ) ) );

						if ( criteria.DateRangeEndGMT.HasValue )
							filterClauses.Add( String.Format( "{0} le {1}", filterFieldName, this.ConvertDate( criteria.DateRangeEndGMT.Value ) ) );
					}
				}
			}

			return string.Join(" and ", filterClauses.ToArray() );
		}

		/// <summary>
		///	Convert date in format suitable for REST end point
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		protected string ConvertDate( DateTime date )
		{
			return date.ToString( "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture );
		}
	}
}
