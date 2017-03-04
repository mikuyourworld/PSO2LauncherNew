using MetroFramework;
using MetroFramework.Components;
using System;

namespace MetroFramework.Interfaces
{
	public interface IMetroForm
	{
		MetroColorStyle Style
		{
			get;
			set;
		}

		MetroStyleManager StyleManager
		{
			get;
			set;
		}

		MetroThemeStyle Theme
		{
			get;
			set;
		}
	}
}