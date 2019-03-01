using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	public class CustomField
	{
		/// <summary>
		/// Identifies the custom field
		/// </summary>
		public int FieldID { get; set; }
		/// <summary>
		/// Uniquely identifies the order within the ChannelAdvisor system
		/// </summary>
		public int OrderID { get; set; }
		/// <summary>
		/// Uniquely identifies the ChannelAdvisor account
		/// </summary>
		public int ProfileID { get; set; }
		/// <summary>
		/// The value of the custom field
		/// </summary>
		public string Value { get; set; }
		/// <summary>
		/// Reference to the order to which the custom field belongs
		/// </summary>
		public Order Order { get; set; }
	}
}
