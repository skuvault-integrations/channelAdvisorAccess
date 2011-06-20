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
			_criteria = new InventoryItemCriteria();
			_detailLevel = new InventoryItemDetailLevel();
		}

		public ItemsFilter( InventoryItemCriteria criteria, InventoryItemDetailLevel detailLevel ) 
		{
			this._criteria = criteria;
			this._detailLevel = detailLevel;
		}

		public ItemsFilter( InventoryItemCriteria criteria, InventoryItemDetailLevel detailLevel, InventoryItemSortField sortField, SortDirection sortDirection )
		{
			this._criteria = criteria;
			this._detailLevel = detailLevel;
			this._sortField = sortField;
			this._sortDirection = sortDirection;
		}

		private InventoryItemCriteria _criteria;
		private InventoryItemDetailLevel _detailLevel;
		private InventoryItemSortField? _sortField;
		private SortDirection? _sortDirection;

		public InventoryItemCriteria Criteria
		{
			get { return this._criteria; }
			set { this._criteria = value; }
		}

		public InventoryItemDetailLevel DetailLevel
		{
			get { return this._detailLevel; }
			set { this._detailLevel = value; }
		}

		public InventoryItemSortField? SortField
		{
			get { return this._sortField; }
			set { this._sortField = value; }
		}

		public SortDirection? SortDirection
		{
			get { return this._sortDirection; }
			set { this._sortDirection = value; }
		}
	}
}