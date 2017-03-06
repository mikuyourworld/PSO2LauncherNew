using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace MetroFramework.Fonts
{
    public class FontResolver : MetroFonts.IMetroFontResolver
	{
		private const string OPEN_SANS_REGULAR = "Open Sans";

		private readonly PrivateFontCollection fontCollection = new PrivateFontCollection();

		private FontFamily GetFontFamily(string familyName)
		{
			FontFamily families;
			lock (this.fontCollection)
			{
				FontFamily[] fontFamilyArray = this.fontCollection.Families;
				int num = 0;
				while (num < (int)fontFamilyArray.Length)
				{
					FontFamily fontFamily = fontFamilyArray[num];
					if (fontFamily.Name != familyName)
                        num++;
                    else
					{
						families = fontFamily;
						return families;
					}
				}
				string str = string.Concat(this.GetType().Namespace, ".Resources.", familyName.Replace(' ', '\u005F'), ".ttf");
				Stream manifestResourceStream = null;
				IntPtr zero = IntPtr.Zero;
				try
				{
					manifestResourceStream = this.GetType().Assembly.GetManifestResourceStream(str);
					int length = (int)manifestResourceStream.Length;
					zero = Marshal.AllocCoTaskMem(length);
					byte[] numArray = new byte[length];
					manifestResourceStream.Read(numArray, 0, length);
					Marshal.Copy(numArray, 0, zero, length);
					this.fontCollection.AddMemoryFont(zero, length);
					families = this.fontCollection.Families[(int)this.fontCollection.Families.Length - 1];
				}
				finally
				{
					if (manifestResourceStream != null)
                        manifestResourceStream.Dispose();
                    if (zero != IntPtr.Zero)
                        Marshal.FreeCoTaskMem(zero);
                }
			}
			return families;
		}

		public Font ResolveFont(string familyName, float emSize, FontStyle fontStyle, GraphicsUnit unit)
		{
			Font font = new Font(familyName, emSize, fontStyle, unit);
			if (font.Name == familyName || !FontResolver.TryResolve(ref familyName, ref fontStyle))
			{
				return font;
			}
            if (font != null)
                font.Dispose();
			return new Font(this.GetFontFamily(familyName), emSize, fontStyle, unit);
		}

		private static bool TryResolve(ref string familyName, ref FontStyle fontStyle)
		{
			if (familyName == "Segoe UI Light")
			{
				familyName = OPEN_SANS_REGULAR;
				if (fontStyle != FontStyle.Bold)
                    fontStyle = FontStyle.Regular;
                return true;
			}
			if (familyName != "Segoe UI")
			{
				return false;
			}
			if (fontStyle == FontStyle.Bold)
			{
                familyName = OPEN_SANS_REGULAR;
				return true;
			}
			familyName = OPEN_SANS_REGULAR;
			return true;
		}
	}
}