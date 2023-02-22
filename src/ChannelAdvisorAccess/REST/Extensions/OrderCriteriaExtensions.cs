using System;
using System.Collections.Generic;
using System.Linq;
using ChannelAdvisorAccess.OrderService;

namespace ChannelAdvisorAccess.REST.Extensions
{
	public static class OrderCriteriaExtensions
	{
		public const string CreatedDateFieldName = "CreatedDateUtc";
		public const string CheckoutDateFieldName = "CheckoutDateUtc";
		public const string PaymentDateFieldName = "PaymentDateUtc";
		public const string ShippingDateFieldName = "ShippingDateUtc";
		public const string OrderIdFieldName = "ID";
		public const string ImportDateFieldName = "ImportDateUtc";

		/// <summary>
		///	Gets filtering parameter for REST GET request
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="orderId">Order id</param>
		/// <returns></returns>
		public static string ToRequestFilterString( this OrderCriteria criteria, int? orderId = null )
		{
			List< string > clauses = new List< string >();
			
			if ( criteria.OrderCreationFilterBeginTimeGMT.HasValue )
				clauses.Add( $"{CreatedDateFieldName} ge {criteria.OrderCreationFilterBeginTimeGMT.Value.ToDateTimeOffset()} and " );

			if ( criteria.OrderCreationFilterEndTimeGMT.HasValue )
				clauses.Add( $"{CreatedDateFieldName} le {criteria.OrderCreationFilterEndTimeGMT.Value.ToDateTimeOffset()} and " );

			var importDateFilter = "";
			if ( criteria.ImportDateFilterBeginTimeGMT.HasValue
				&& criteria.ImportDateFilterEndTimeGMT.HasValue )
			{
				importDateFilter = $"({ImportDateFieldName} ge {criteria.ImportDateFilterBeginTimeGMT.Value.ToDateTimeOffset()} " + 
					$"and {ImportDateFieldName} le {criteria.ImportDateFilterEndTimeGMT.Value.ToDateTimeOffset()})";
			}

			if ( criteria.StatusUpdateFilterBeginTimeGMT.HasValue
				&& criteria.StatusUpdateFilterEndTimeGMT.HasValue )
			{
				var statusUpdateBegin = criteria.StatusUpdateFilterBeginTimeGMT.Value.ToDateTimeOffset();
				var statusUpdateEnd = criteria.StatusUpdateFilterEndTimeGMT.Value.ToDateTimeOffset();
				clauses.Add( $"({CheckoutDateFieldName} ge {statusUpdateBegin} and {CheckoutDateFieldName} le {statusUpdateEnd}) " + 
					$"or ({PaymentDateFieldName} ge {statusUpdateBegin} and {PaymentDateFieldName} le {statusUpdateEnd}) " + 
					$"or ({ShippingDateFieldName} ge {statusUpdateBegin} and {ShippingDateFieldName} le {statusUpdateEnd}) " +
					$"{( !string.IsNullOrWhiteSpace( importDateFilter ) ? "or " + importDateFilter +" " : "" )}and " );
			} else {
				if ( !string.IsNullOrWhiteSpace( importDateFilter ) )
				{
					clauses.Add( $"{importDateFilter} and " );
				}
			}

			if ( orderId != null )
				clauses.Add( $"{OrderIdFieldName} eq {orderId.Value.ToString()} and " );

			if ( clauses.Count > 0 )
				clauses[ clauses.Count - 1] = clauses.Last().Substring( 0, clauses.Last().LastIndexOf( "and" ) );

			return string.Join( " ", clauses );
		}
	}
}