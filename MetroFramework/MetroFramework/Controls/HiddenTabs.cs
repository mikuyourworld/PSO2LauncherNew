using System;

namespace MetroFramework.Controls
{
	public class HiddenTabs
	{
		private int _index;

		private string _tabpage;

		public int index
		{
			get
			{
				return this._index;
			}
		}

		public string tabpage
		{
			get
			{
				return this._tabpage;
			}
		}

		public HiddenTabs(int id, string page)
		{
			this._index = id;
			this._tabpage = page;
		}
	}
}