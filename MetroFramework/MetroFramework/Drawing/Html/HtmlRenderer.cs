using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace MetroFramework.Drawing.Html
{
	public static class HtmlRenderer
	{
		private static List<Assembly> _references;

		public static List<Assembly> References
		{
			get
			{
				return HtmlRenderer._references;
			}
		}

		static HtmlRenderer()
		{
			HtmlRenderer._references = new List<Assembly>();
			HtmlRenderer.References.Add(Assembly.GetExecutingAssembly());
		}

		internal static void AddReference(Assembly assembly)
		{
			if (!HtmlRenderer.References.Contains(assembly))
			{
				HtmlRenderer.References.Add(assembly);
			}
		}

		public static void Render(Graphics g, string html, PointF location, float width)
		{
			HtmlRenderer.Render(g, html, new RectangleF(location, new SizeF(width, 0f)), false);
		}

		public static void Render(Graphics g, string html, RectangleF area, bool clip)
		{
			InitialContainer initialContainer = new InitialContainer(html);
			Region region = g.Clip;
			if (clip)
			{
				g.SetClip(area);
			}
			initialContainer.SetBounds(area);
			initialContainer.MeasureBounds(g);
			initialContainer.Paint(g);
			if (clip)
			{
				g.SetClip(region, CombineMode.Replace);
			}
		}
	}
}