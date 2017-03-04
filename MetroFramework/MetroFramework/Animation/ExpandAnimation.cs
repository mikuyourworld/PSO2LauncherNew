using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MetroFramework.Animation
{
	public sealed class ExpandAnimation : AnimationBase
	{
		public ExpandAnimation()
		{
		}

		private int DoExpandAnimation(int startSize, int targetSize)
		{
			float single = (float)this.counter - (float)this.startTime;
			float single1 = (float)startSize;
			float single2 = (float)targetSize - (float)startSize;
			float single3 = (float)this.targetTime - (float)this.startTime;
			return base.MakeTransition(single, single1, single3, single2);
		}

		public void Start(Control control, Size targetSize, TransitionType transitionType, int duration)
		{
			base.Start(control, transitionType, duration, () => {
				int num = this.DoExpandAnimation(control.Width, targetSize.Width);
				int num1 = this.DoExpandAnimation(control.Height, targetSize.Height);
				control.Size = new Size(num, num1);
			}, () => control.Size.Equals(targetSize));
		}
	}
}