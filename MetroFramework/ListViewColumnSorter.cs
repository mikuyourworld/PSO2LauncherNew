using System;
using System.Collections;
using System.Windows.Forms;

public class ListViewColumnSorter : IComparer
{
	public int ColumnToSort;

	public SortOrder OrderOfSort;

	private CaseInsensitiveComparer ObjectCompare;

	private ListViewColumnSorter.SortModifiers mySortModifier = ListViewColumnSorter.SortModifiers.SortByText;

	public ListViewColumnSorter.SortModifiers _SortModifier
	{
		get
		{
			return this.mySortModifier;
		}
		set
		{
			this.mySortModifier = value;
		}
	}

	public SortOrder Order
	{
		get
		{
			return this.OrderOfSort;
		}
		set
		{
			this.OrderOfSort = value;
		}
	}

	public int SortColumn
	{
		get
		{
			return this.ColumnToSort;
		}
		set
		{
			this.ColumnToSort = value;
		}
	}

	public ListViewColumnSorter()
	{
		this.ColumnToSort = 0;
		this.ObjectCompare = new CaseInsensitiveComparer();
	}

	public int Compare(object x, object y)
	{
		DateTime dateTime;
		DateTime dateTime1;
		int num = 0;
		ListViewItem listViewItem = (ListViewItem)x;
		ListViewItem listViewItem1 = (ListViewItem)y;
		num = (!DateTime.TryParse(listViewItem.SubItems[this.ColumnToSort].Text, out dateTime) || !DateTime.TryParse(listViewItem1.SubItems[this.ColumnToSort].Text, out dateTime1) ? this.ObjectCompare.Compare(listViewItem.SubItems[this.ColumnToSort].Text, listViewItem1.SubItems[this.ColumnToSort].Text) : this.ObjectCompare.Compare(dateTime, dateTime1));
		if (this.OrderOfSort == SortOrder.Ascending)
		{
			return num;
		}
		if (this.OrderOfSort != SortOrder.Descending)
		{
			return 0;
		}
		return -num;
	}

	public enum SortModifiers
	{
		SortByImage,
		SortByCheckbox,
		SortByText
	}
}