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

		public ItemsFilter( InventoryItemCriteria criteria, InventoryItemDetailLevel detailLevel, string sortField, string sortDirection )
		{
			this._criteria = criteria;
			this._detailLevel = detailLevel;
			this._sortField = sortField;
			this._sortDirection = sortDirection;
		}

		private InventoryItemCriteria _criteria;
		private InventoryItemDetailLevel _detailLevel;
		private string _sortField;
		private string _sortDirection;

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

		public string SortField
		{
			get { return this._sortField; }
			set { this._sortField = value; }
		}

		public string SortDirection
		{
			get { return this._sortDirection; }
			set { this._sortDirection = value; }
		}
	}
}