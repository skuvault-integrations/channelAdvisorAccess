using System;

namespace ChannelAdvisorAccess.OrderService
{
	/// <summary>
	/// A partial created to add ImportDateFilter since the generated partial class OrderCriteria in Reference.cs is missing it, even if regenerate.
	/// </summary>
	public partial class OrderCriteria
	{
		/// <summary>
		/// Orders imported into ChannelAdvisor after
		/// </summary>
		public DateTime? ImportDateFilterBeginTimeGMT { get; set; }
		
		/// <summary>
		/// Orders imported into ChannelAdvisor before
		/// </summary>
		public DateTime? ImportDateFilterEndTimeGMT { get; set; }
	}
}