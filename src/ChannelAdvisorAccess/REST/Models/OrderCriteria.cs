using System;
using System.Collections.Generic;
using System.Linq;
using ChannelAdvisorAccess.REST.Extensions;

namespace ChannelAdvisorAccess.REST.Models
{
	/// <summary>
	///	Order filter criteria 
	/// </summary>
	public class OrderCriteria
	{
		/// <summary>
		/// Get only orders imported into ChannelAdvisor after this time (synced from other channels)
		/// </summary>
		private DateTime? ImportDateFilterBegin;
		
		/// <summary>
		/// Get only orders imported into ChannelAdvisor before this time (synced from other channels)
		/// </summary>
		private DateTime? ImportDateFilterEnd;
		
		/// <summary>
		/// Order status update filter starting date/time. In the API request, this is split into multiple statuses
		/// </summary>
		private DateTime? StatusUpdateFilterBegin;
		
		/// <summary>
		/// Order status update filter ending date/time. In the API request, this is split into multiple statuses
		/// </summary>
		private DateTime? StatusUpdateFilterEnd;
		
		internal int[] OrderIDList { get; }

		/// <summary>
		/// Filter by orderIds
		/// </summary>
		/// <param name="orderIds"></param>
		public OrderCriteria( int[] orderIds )
		{
			this.OrderIDList = orderIds;
		}

		/// <summary>
		/// Filter by order status and import start/end dates
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		public OrderCriteria( DateTime startDate, DateTime endDate )
		{
			this.StatusUpdateFilterBegin = startDate;
			this.StatusUpdateFilterEnd = endDate;
			this.ImportDateFilterBegin = startDate;
			this.ImportDateFilterEnd = endDate;
		}

		public const string CheckoutDateFieldName = "CheckoutDateUtc";
		public const string PaymentDateFieldName = "PaymentDateUtc";
		public const string ShippingDateFieldName = "ShippingDateUtc";
		public const string ImportDateFieldName = "ImportDateUtc";

		/// <summary>
		///	Gets filtering parameter for REST GET request
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		public override string ToString()
		{
			List< string > clauses = new List< string >();

			var importDateFilter = "";
			if ( this.ImportDateFilterBegin.HasValue
				&& this.ImportDateFilterEnd.HasValue )
			{
				importDateFilter = $"({ImportDateFieldName} ge {this.ImportDateFilterBegin.Value.ToDateTimeOffset()} " + 
					$"and {ImportDateFieldName} le {this.ImportDateFilterEnd.Value.ToDateTimeOffset()})";
			}

			if ( this.StatusUpdateFilterBegin.HasValue
				&& this.StatusUpdateFilterEnd.HasValue )
			{
				var statusUpdateBegin = this.StatusUpdateFilterBegin.Value.ToDateTimeOffset();
				var statusUpdateEnd = this.StatusUpdateFilterEnd.Value.ToDateTimeOffset();
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

			if ( clauses.Count > 0 )
				clauses[ clauses.Count - 1] = clauses.Last().Substring( 0, clauses.Last().LastIndexOf( "and" ) );

			return string.Join( " ", clauses );
		}
	}
}