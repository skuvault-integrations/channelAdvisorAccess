using System;

namespace ChannelAdvisorAccess.REST.Models
{
	/// <summary>
	///	Order criteria passed from v1, both REST and SOAP.
	/// Created since the previously used auto-generated SOAP OrderCriteria is missing some fields REST has 
	/// </summary>
	public class OrderCriteria
	{
		/// <summary>
		/// Orders imported into ChannelAdvisor after
		/// NOTE: Only the REST API supports this, not SOAP
		/// </summary>
		public DateTime? ImportDateFilterBegin { get; set; }
		
		/// <summary>
		/// Orders imported into ChannelAdvisor before
		/// NOTE: Only the REST API supports this, not SOAP
		/// </summary>
		public DateTime? ImportDateFilterEnd { get; set; }
		
		/// <summary>
		/// Order status update filter starting date/time
		/// SOAP API - Passed as-is
		/// REST API - Split into different statuses
		/// </summary>
		public DateTime? StatusUpdateFilterBegin { get; set; }
		
		/// <summary>
		/// Order status update filter ending date/time
		/// SOAP API - Passed as-is
		/// REST API - Split into different statuses
		/// </summary>
		public DateTime? StatusUpdateFilterEnd { get; set; }
		
		public int[] OrderIDList { get; set; }
		
		/// <summary>
		/// Order details to return. Valid options are in class DetailLevelTypes
		/// NOTE: SOAP API only
		/// </summary>
		public string DetailLevel { get; set; }
	}
}