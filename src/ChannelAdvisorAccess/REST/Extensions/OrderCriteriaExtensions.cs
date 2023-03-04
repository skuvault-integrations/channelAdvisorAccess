namespace ChannelAdvisorAccess.REST.Extensions
{
	public static class OrderCriteriaExtensions
	{
		public const string OrderIdFieldName = "ID";

		/// <summary>
		///	Gets filter parameter for REST GET request
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <returns></returns>
		public static string ToRequestFilterString( this int orderId )
		{
			return $"{OrderIdFieldName} eq {orderId.ToString()} ";
		}
	}
}