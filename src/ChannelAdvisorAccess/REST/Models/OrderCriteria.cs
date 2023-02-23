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
		public DateTime? ImportDateFilterBeginTimeGMT { get; set; }
		
		/// <summary>
		/// Orders imported into ChannelAdvisor before
		/// NOTE: Only the REST API supports this, not SOAP
		/// </summary>
		public DateTime? ImportDateFilterEndTimeGMT { get; set; }
		
		public DateTime? StatusUpdateFilterBeginTimeGMT { get; set; }
		
		public DateTime? StatusUpdateFilterEndTimeGMT { get; set; }
		
		public int[] OrderIDList { get; set; }
		
		public string DetailLevel{ get; set; }
	}
}