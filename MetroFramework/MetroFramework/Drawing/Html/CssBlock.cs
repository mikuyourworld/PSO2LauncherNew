using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MetroFramework.Drawing.Html
{
	[CLSCompliant(false)]
	public class CssBlock
	{
		private string _block;

		private Dictionary<PropertyInfo, string> _propertyValues;

		private Dictionary<string, string> _properties;

		public string BlockSource
		{
			get
			{
				return this._block;
			}
		}

		public Dictionary<string, string> Properties
		{
			get
			{
				return this._properties;
			}
		}

		public Dictionary<PropertyInfo, string> PropertyValues
		{
			get
			{
				return this._propertyValues;
			}
		}

		private CssBlock()
		{
			this._propertyValues = new Dictionary<PropertyInfo, string>();
			this._properties = new Dictionary<string, string>();
		}

		public CssBlock(string blockSource) : this()
		{
			this._block = blockSource;
			foreach (Match match in Parser.Match(";?[^;\\s]*:[^\\{\\}:;]*(\\}|;)?", blockSource))
			{
				string[] strArrays = match.Value.Split(new char[] { ':' });
				if ((int)strArrays.Length != 2)
				{
					continue;
				}
				string str = strArrays[0].Trim();
				string str1 = strArrays[1].Trim();
				if (str1.EndsWith(";"))
				{
					str1 = str1.Substring(0, str1.Length - 1).Trim();
				}
				this.Properties.Add(str, str1);
				if (!CssBox._properties.ContainsKey(str))
				{
					continue;
				}
				this.PropertyValues.Add(CssBox._properties[str], str1);
			}
		}

		public void AssignTo(CssBox b)
		{
			foreach (PropertyInfo key in this.PropertyValues.Keys)
			{
				string item = this.PropertyValues[key];
				if (item == "inherit" && b.ParentBox != null)
				{
					item = Convert.ToString(key.GetValue(b.ParentBox, null));
				}
				key.SetValue(b, item, null);
			}
		}

		internal void UpdatePropertyValues()
		{
			this.PropertyValues.Clear();
			foreach (string key in this.Properties.Keys)
			{
				if (!CssBox._properties.ContainsKey(key))
				{
					continue;
				}
				this.PropertyValues.Add(CssBox._properties[key], this.Properties[key]);
			}
		}
	}
}