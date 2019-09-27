using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	/// <summary>
	///	Discounts
	/// </summary>
	public class Promotion
	{
		/// <summary>
		/// Uniquely identifies the promotion within the ChannelAdvisor account
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// 	The code submitted to enable the discount
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// The discount to the item cost. Will be negative
		/// </summary>
		public decimal Amount { get; set; }
		/// <summary>
		/// The discount to the shipping cost. Will be negative.
		/// </summary>
		public decimal ShippingAmount { get; set; }
		/// <summary>
		/// Reference to the order item to which the promotion belongs
		/// </summary>
		public OrderItem OrderItem { get; set; }
	}
}
