using System;

namespace MetroFramework.Drawing.Html
{
	public class CssPropertyAttribute : Attribute
	{
		private string _name;

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public CssPropertyAttribute(string name)
		{
			this.Name = name;
		}
	}
}