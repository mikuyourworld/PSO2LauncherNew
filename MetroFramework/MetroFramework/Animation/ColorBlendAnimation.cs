using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Animation
{
	public sealed class ColorBlendAnimation : AnimationBase
	{
		private double percent = 1;

		public ColorBlendAnimation()
		{
		}

		private Color DoColorBlend(Color startColor, Color targetColor, double ratio)
		{
			ColorBlendAnimation colorBlendAnimation = this;
			colorBlendAnimation.percent = colorBlendAnimation.percent + 0.2;
			int num = (int)Math.Round((double)startColor.A * (1 - ratio) + (double)targetColor.A * ratio);
			int num1 = (int)Math.Round((double)startColor.R * (1 - ratio) + (double)targetColor.R * ratio);
			int num2 = (int)Math.Round((double)startColor.G * (1 - ratio) + (double)targetColor.G * ratio);
			int num3 = (int)Math.Round((double)startColor.B * (1 - ratio) + (double)targetColor.B * ratio);
			return Color.FromArgb(num, num1, num2, num3);
		}

		private Color GetPropertyValue(string pName, Control control)
		{
			Type type = control.GetType();
			return (Color)type.InvokeMember(pName, BindingFlags.GetProperty, null, control, null);
		}

		public void Start(Control control, string property, Color targetColor, int duration)
		{
			if (duration == 0)
			{
				duration = 1;
			}
			base.Start(control, this.transitionType, 2 * duration, () => {
				Color propertyValue = this.GetPropertyValue(property, control);
				Color color = this.DoColorBlend(propertyValue, targetColor, 0.1 * (this.percent / 2));
				control.GetType().GetProperty(property).GetSetMethod(true).Invoke(control, new object[] { color });
			}, () => {
				Color propertyValue = this.GetPropertyValue(property, control);
				if (propertyValue.A.Equals(targetColor.A) && propertyValue.R.Equals(targetColor.R) && propertyValue.G.Equals(targetColor.G) && propertyValue.B.Equals(targetColor.B))
				{
					return true;
				}
				return false;
			});
		}
	}
}