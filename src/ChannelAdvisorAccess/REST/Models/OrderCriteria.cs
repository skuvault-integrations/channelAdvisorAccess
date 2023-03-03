using System;

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
		public DateTime? ImportDateFilterBegin { get; set; }
		
		/// <summary>
		/// Get only orders imported into ChannelAdvisor before this time (synced from other channels)
		/// </summary>
		public DateTime? ImportDateFilterEnd { get; set; }
		
		/// <summary>
		/// Order status update filter starting date/time. In the API request, this is split into multiple statuses
		/// </summary>
		public DateTime? StatusUpdateFilterBegin { get; set; }
		
		/// <summary>
		/// Order status update filter ending date/time. In the API request, this is split into multiple statuses
		/// </summary>
		public DateTime? StatusUpdateFilterEnd { get; set; }
		
		public int[] OrderIDList { get; set; }
	}
}