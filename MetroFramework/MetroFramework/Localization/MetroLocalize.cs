using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MetroFramework.Localization
{
	internal class MetroLocalize
	{
		private DataSet languageDataset;

		public MetroLocalize(string ctrlName)
		{
			this.importManifestResource(ctrlName);
		}

		public MetroLocalize(Control ctrl)
		{
			this.importManifestResource(ctrl.Name);
		}

		private string convertVar(object var)
		{
			if (var == null)
			{
				return "";
			}
			return var.ToString();
		}

		public string CurrentLanguage()
		{
			string twoLetterISOLanguageName = Application.CurrentCulture.TwoLetterISOLanguageName;
			if (twoLetterISOLanguageName.Length == 0)
			{
				twoLetterISOLanguageName = this.DefaultLanguage();
			}
			return twoLetterISOLanguageName.ToLower();
		}

		public string DefaultLanguage()
		{
			return "en";
		}

		public string getValue(string key, object var1, object var2, object var3)
		{
			string str = this.translate(key);
			str = str.Replace("#1", this.convertVar(var1));
			str = str.Replace("#2", this.convertVar(var2));
			return str.Replace("#3", this.convertVar(var3));
		}

		public string getValue(string key, object var1, object var2, object var3, object var4)
		{
			string str = this.translate(key);
			str = str.Replace("#1", this.convertVar(var1));
			str = str.Replace("#2", this.convertVar(var2));
			str = str.Replace("#3", this.convertVar(var3));
			return str.Replace("#4", this.convertVar(var4));
		}

		public string getValue(string key, object var1, object var2, object var3, object var4, object var5)
		{
			string str = this.translate(key);
			str = str.Replace("#1", this.convertVar(var1));
			str = str.Replace("#2", this.convertVar(var2));
			str = str.Replace("#3", this.convertVar(var3));
			str = str.Replace("#4", this.convertVar(var4));
			return str.Replace("#5", this.convertVar(var5));
		}

		private void importManifestResource(string ctrlName)
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			Stream manifestResourceStream = null;
			if (entryAssembly != null)
			{
				string[] name = new string[] { entryAssembly.GetName().Name, ".Localization.", this.CurrentLanguage(), ".", ctrlName, ".xml" };
				manifestResourceStream = entryAssembly.GetManifestResourceStream(string.Concat(name));
			}
			if (manifestResourceStream == null)
			{
				entryAssembly = Assembly.GetCallingAssembly();
				string[] strArrays = new string[] { entryAssembly.GetName().Name, ".Localization.", this.CurrentLanguage(), ".", ctrlName, ".xml" };
				manifestResourceStream = entryAssembly.GetManifestResourceStream(string.Concat(strArrays));
				if (manifestResourceStream == null)
				{
					string[] name1 = new string[] { entryAssembly.GetName().Name, ".Localization.", this.DefaultLanguage(), ".", ctrlName, ".xml" };
					manifestResourceStream = entryAssembly.GetManifestResourceStream(string.Concat(name1));
				}
			}
			if (this.languageDataset == null)
			{
				this.languageDataset = new DataSet();
			}
			if (manifestResourceStream != null)
			{
				DataSet dataSet = new DataSet();
				dataSet.ReadXml(manifestResourceStream);
				this.languageDataset.Merge(dataSet);
				manifestResourceStream.Close();
			}
		}

		public string translate(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return "";
			}
			if (this.languageDataset == null)
			{
				return string.Concat("&", key);
			}
			if (this.languageDataset.Tables["Localization"] == null)
			{
				return string.Concat("&", key);
			}
			DataRow[] dataRowArray = this.languageDataset.Tables["Localization"].Select(string.Concat("Key='", key, "'"));
			if ((int)dataRowArray.Length <= 0)
			{
				return string.Concat("~", key);
			}
			return dataRowArray[0]["Value"].ToString();
		}

		public string translate(string key, object var1)
		{
			string str = this.translate(key);
			return str.Replace("#1", this.convertVar(var1));
		}

		public string translate(string key, object var1, object var2)
		{
			string str = this.translate(key);
			str = str.Replace("#1", this.convertVar(var1));
			return str.Replace("#2", this.convertVar(var2));
		}
	}
}