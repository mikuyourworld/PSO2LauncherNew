using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using System;
using System.Runtime.CompilerServices;

namespace MetroFramework.Interfaces
{
	public interface IMetroControl
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

		bool UseCustomBackColor
		{
			get;
			set;
		}

		bool UseCustomForeColor
		{
			get;
			set;
		}

		bool UseSelectable
		{
			get;
			set;
		}

		bool UseStyleColors
		{
			get;
			set;
		}

		event EventHandler<MetroPaintEventArgs> CustomPaint;

		event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

		event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
	}
}