using ChannelAdvisorAccess.InventoryService;

namespace ChannelAdvisorAccess.Services.Items
{
	/// <summary>
	/// Collects all properties for item filtering together.
	/// </summary>
	public class ItemsFilter
	{
		public ItemsFilter()
		{
			this.Criteria = new InventoryItemCriteria();
			this.DetailLevel = new InventoryItemDetailLevel();
		}

		public ItemsFilter( InventoryItemCriteria criteria, InventoryItemDetailLevel detailLevel )
		{
			this.Criteria = criteria;
			this.DetailLevel = detailLevel;
		}

		public ItemsFilter( InventoryItemCriteria criteria, InventoryItemDetailLevel detailLevel, string sortField, string sortDirection )
		{
			this.Criteria = criteria;
			this.DetailLevel = detailLevel;
			this.SortField = sortField;
			this.SortDirection = sortDirection;
		}

		public InventoryItemCriteria Criteria{ get; set; }

		public InventoryItemDetailLevel DetailLevel{ get; set; }

		public string SortField{ get; set; }

		public string SortDirection{ get; set; }
	}
}