using System;
using ChannelAdvisorAccess.InventoryService;

namespace ChannelAdvisorAccess.Misc
{
	public static class ItemExtensions
	{
		static ItemExtensions()
		{
			// Initialize default SKU detector
			ParentSkuDetector = sku => sku.EndsWith( " pt", StringComparison.InvariantCultureIgnoreCase );
		}

		public static Func< string, bool > ParentSkuDetector{ get; set; }

		public static bool IsParent( this InventoryItemResponse item )
		{
			return item.Sku.IsParentSku();
		}

		public static bool IsParentSku( this string sku )
		{
			return ParentSkuDetector( sku );
		}
	}
}